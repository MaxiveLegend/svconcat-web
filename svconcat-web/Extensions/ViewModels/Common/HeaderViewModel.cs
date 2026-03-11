using Umbraco.Cms.Core.Models.PublishedContent;

namespace svconcat_web.Extensions.ViewModels.Common;

public class HeaderViewModel
{
    public IPublishedContent CurrentPage { get; set; }
    public IEnumerable<LinkViewModel> MainNavItems { get; set; }
    public IEnumerable<LinkViewModel> ExternalLinks { get; set; }
    public string WebsiteUrl { get; set; }
}