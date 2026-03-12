using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;

namespace SvConcatWeb.Extensions.MappingStrategies.Common;

public class LinkToLinkViewModel : IMappingStrategy<Link, LinkViewModel>
{
    public LinkViewModel Execute(Link source)
    {
        if (source == null) return null;
        
        return new LinkViewModel()
        {
            Url = source?.Url ?? string.Empty,
            Name = source?.Name ??  string.Empty,
            Target = source?.Target ?? string.Empty
        };
    }
}