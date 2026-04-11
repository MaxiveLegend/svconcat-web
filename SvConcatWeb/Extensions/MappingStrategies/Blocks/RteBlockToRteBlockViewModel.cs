using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class RteBlockToRteBlockViewModel : IMappingStrategy<RteBlock, RteBlockViewModel>
{
    public RteBlockViewModel Execute(RteBlock source)
    {
        var vm = new RteBlockViewModel();

        if (source == null) return vm;
        
        vm.Content = source.Content;

        return vm;
    }
}