using SvConcatWeb.Extensions.ViewModels.Common.Footer;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Common;

public class FooterColumnToFooterColumnViewModel(IViewmodelFactory viewmodelFactory) : IMappingStrategy<BlockListItem, FooterColumnViewModel>
{
    public FooterColumnViewModel Execute(BlockListItem source)
    {
        var vm = new FooterColumnViewModel();

        if (source?.Content is not FooterColumn sourceColumn) return vm;

        vm.ColumnName = sourceColumn.ColumnName ?? string.Empty;
        vm.items = sourceColumn.Items.Select(viewmodelFactory
            .CreateViewModel<BlockListItem, FooterColumnItemViewModel>);

        return vm;
    }
}