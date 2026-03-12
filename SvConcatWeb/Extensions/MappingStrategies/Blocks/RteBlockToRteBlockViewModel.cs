using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class RteBlockToRteBlockViewModel : IMappingStrategy<BlockListItem, RteBlockViewModel>
{
    public RteBlockViewModel Execute(BlockListItem source)
    {
        var sourceContent = source?.Content as RteBlock;
        var vm = new RteBlockViewModel();

        if (sourceContent == null) return vm;
        
        vm.Content = sourceContent.Content;

        return vm;
    }
}