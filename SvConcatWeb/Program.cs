using SvConcatWeb.Extensions.Controllers;
using Umbraco.Cms.Web.Website.Controllers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

// Configure Umbraco Render Controller Type
builder.Services.Configure<UmbracoRenderingDefaultsOptions>(options => options.DefaultControllerType = typeof(DefaultController));

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

// Security policy
var policyCollection = new HeaderPolicyCollection()
    .AddFrameOptionsSameOrigin() // Fallback for legacy browsers (CSP handles modern)
    .AddContentTypeOptionsNoSniff() // Prevents MIME-type sniffing
    .AddReferrerPolicyStrictOriginWhenCrossOrigin() // Protects privacy when linking out
    .AddStrictTransportSecurityMaxAgeIncludeSubDomainsAndPreload() // HSTS 1 year
    .AddPermissionsPolicy(builder => // Browser functionality permissions
    {
        builder.AddAccelerometer().None();
        builder.AddAutoplay().Self(); // Allow self-hosted video/audio if needed
        builder.AddCamera().None();
        // builder.AddEncryptedMedia().None();
        builder.AddFullscreen().All(); // Good for event photos/videos
        builder.AddGeolocation().None();
        builder.AddGyroscope().None();
        builder.AddMagnetometer().None();
        builder.AddMicrophone().None();
        builder.AddMidi().None();
        builder.AddPayment().None();
        builder.AddPictureInPicture().None();
        builder.AddUsb().None();
        builder.AddXrSpatialTracking().None();
    })
    .AddCustomHeader("Cross-Origin-Opener-Policy", "same-origin")
    .AddCustomHeader("Cross-Origin-Embedder-Policy", "require-corp")
    .AddCustomHeader("Cross-Origin-Resource-Policy", "same-origin");

// Apply to all requests except the Umbraco Backoffice, to avoid breaking the CMS
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/umbraco"), appBuilder =>
{
    appBuilder.UseSecurityHeaders(policyCollection);
});

await app.RunAsync();
