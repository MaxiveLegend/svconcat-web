using System.Globalization;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace svconcat_web.Extensions.Models;

public interface IMasterModel
{
    IPublishedContent CurrentPage { get; set; }
    CultureInfo CurrentCulture { get; set; }
    Website Website { get; set; }
    
    // SEO
    string SeoTitle { get; set; }
    string SeoDescription { get; set; }
    string SeoCanonical { get; set; }
    string SeoSocialImage { get; set; }
    string SeoSiteName { get; set; }
    string SeoRobots { get; set; }
}

public class MasterModel<T> : ContentModel<T>, IMasterModel
    where T : class, IPublishedContent
{
    public MasterModel(T content) : base(content) {}

    public IPublishedContent CurrentPage { get; set; }
    public CultureInfo CurrentCulture { get; set; }
    public Website Website { get; set; }
    
    // SEO
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string SeoCanonical { get; set; }
    public string SeoSocialImage { get; set; }
    public string SeoSiteName { get; set; }
    public string SeoRobots { get; set; }
}