using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;

namespace SvConcatWeb.Extensions.MappingStrategies.Common;

public class LinkToLinkViewModel : IMappingStrategy<Link, LinkViewModel>
{
    public LinkViewModel Execute(Link source)
    {
        var vm = new LinkViewModel();
        
        if (source == null) return vm;
        
        vm.Url = source.Url ?? string.Empty;
        vm.Target = source.Target ?? string.Empty;
        vm.Name = source.Name ?? string.Empty;

        return vm;
    }
}