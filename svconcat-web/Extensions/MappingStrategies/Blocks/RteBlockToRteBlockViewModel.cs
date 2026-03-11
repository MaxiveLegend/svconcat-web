using svconcat_web.Extensions.ViewModels;
using svconcat_web.Extensions.ViewModels.Blocks;
using svconcat_web.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace svconcat_web.Extensions.MappingStrategies.Blocks;

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