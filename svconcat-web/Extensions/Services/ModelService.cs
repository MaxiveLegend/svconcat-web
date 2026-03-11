using System.Globalization;
using svconcat_web.Extensions.Models;
using svconcat_web.Extensions.Services.Interfaces;
using svconcat_web.Extensions.Utilities;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace svconcat_web.Extensions.Services;

public class ModelService(IVariationContextAccessor variationContextAccessor) : IModelService
{
    public IMasterModel CreateMasterModel(IPublishedContent content, Type contentType = null)
    {
        var model = CreateMagicModel(typeof(MasterModel<>), content, contentType) as IMasterModel;

        model.CurrentCulture = new CultureInfo(variationContextAccessor.VariationContext.Culture);
        model.CurrentPage = content;

        var website = content.Website();
        model.Website = website;
        
        var seoPage = content as Seo;

        if (seoPage != null)
        {
            model.SeoTitle = seoPage.SeoTitle ?? string.Empty;
            model.SeoDescription = seoPage.SeoDescription ?? string.Empty;
            model.SeoSocialImage = seoPage.SeoImage?.Url(mode: UrlMode.Absolute) ?? string.Empty;
            model.SeoSiteName = website.SiteName ??  string.Empty;
            model.SeoCanonical = content.Url(mode: UrlMode.Absolute);
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
}