using Umbraco.Cms.Core.Models.PublishedContent;

namespace SvConcatWeb.Extensions.ViewModels.Common;

public class HeaderViewModel
{
    public IPublishedContent CurrentPage { get; set; }
    public IEnumerable<LinkViewModel> MainNavItems { get; set; }
    public IEnumerable<LinkViewModel> ExternalLinks { get; set; }
    public ImageItemViewModel WebsiteLogo { get; set; }
    public string WebsiteUrl { get; set; }
}