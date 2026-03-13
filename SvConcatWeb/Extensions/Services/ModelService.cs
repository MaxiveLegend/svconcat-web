using System.Globalization;
using SvConcatWeb.Extensions.Models;
using SvConcatWeb.Extensions.Services.Interfaces;
using SvConcatWeb.Extensions.Utilities;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.Services;

public class ModelService(IUmbracoContextAccessor umbracoContextAccessor, IVariationContextAccessor variationContextAccessor) : IModelService
{
    public IMasterModel CreateMasterModel(IPublishedContent content, Type contentType = null)
    {
        var model = CreateMagicModel(typeof(MasterModel<>), content, contentType) as IMasterModel;

        model.CurrentCulture = new CultureInfo(variationContextAccessor.VariationContext.Culture);
        model.CurrentPage = content;

        var website = content.Website();
        model.Website = website;

        var isHome = content.IsHomeNode(website);

        if (content is ISeo seoPage)
        {
            model.SeoTitle = seoPage.SeoTitle ?? string.Empty;
            model.SeoDescription = seoPage.SeoDescription ?? string.Empty;
            model.SeoSocialImage = seoPage.SeoImage?.Url(mode: UrlMode.Absolute) ?? string.Empty;
            model.SeoSiteName = website.SiteName ?? string.Empty;
    
            model.SeoCanonical = isHome 
                ? website.Url(mode: UrlMode.Absolute) 
                : content.Url(mode: UrlMode.Absolute);

            model.SeoRobots = seoPage.DisableIndexing ? "noindex, nofollow" : "index, follow";
        }

        return model;
    }

    private object CreateMagicModel(Type genericType, IPublishedContent content, Type contentType = null)
    {
        if (contentType == null) contentType = content.GetType();

        var modelType = genericType.MakeGenericType(contentType);
        var model = Activator.CreateInstance(modelType, content);

        return model;
    }
    
    // private bool isHomeNode(IPublishedContent content, Website website)
    // {
    //     var redirectValue = website.GetProperty("umbracoInternalRedirectId")?.GetValue();
    //     int? redirectId = null;
    //
    //     var contentCache = umbracoContextAccessor.GetRequiredUmbracoContext().Content;
    //
    //     redirectId = redirectValue switch
    //     {
    //         Guid guid => contentCache.GetById(guid)?.Id,
    //         GuidUdi guidUdi => contentCache.GetById(guidUdi.Guid)?.Id,
    //         _ => redirectId
    //     };
    //
    //     return content.Id == redirectId || content.Id == website.Id;
    // }
}