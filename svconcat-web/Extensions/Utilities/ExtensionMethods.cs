using svconcat_web.Extensions.Models.Custom;
using svconcat_web.Extensions.ViewModels;
using svconcat_web.Extensions.ViewModels.Common;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace svconcat_web.Extensions.Utilities;

public static class ExtensionMethods
{

    #region IPublishedContent

    public static Website Website(this IPublishedContent content)
    {
        return content.AncestorOrSelf(1) as Website;
    }

    #endregion
}