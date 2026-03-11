using svconcat_web.Extensions.ViewModels;
using svconcat_web.Extensions.ViewModels.Common;
using svconcat_web.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;

namespace svconcat_web.Extensions.MappingStrategies.Common;

public class LinkToLinkViewModel : IMappingStrategy<Link, LinkViewModel>
{
    public LinkViewModel Execute(Link source)
    {
        return new LinkViewModel()
        {
            Url = source?.Url ?? string.Empty,
            Name = source?.Name ??  string.Empty,
            Target = source?.Target ?? string.Empty
        };
    }
}