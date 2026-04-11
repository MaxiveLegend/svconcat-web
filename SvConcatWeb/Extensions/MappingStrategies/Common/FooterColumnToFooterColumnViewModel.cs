using SvConcatWeb.Extensions.ViewModels.Common.Footer;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Common;

public class FooterColumnToFooterColumnViewModel(IViewmodelFactory viewmodelFactory) : IMappingStrategy<FooterColumn, FooterColumnViewModel>
{
    public FooterColumnViewModel Execute(FooterColumn source)
    {
        var vm = new FooterColumnViewModel();

        if (source == null) return vm;

        vm.ColumnName = source.ColumnName ?? string.Empty;
        vm.items = source.Items
            .Select(item => item?.Content as FooterItem)
            .Select(viewmodelFactory.CreateViewModel<FooterItem, FooterColumnItemViewModel>);

        return vm;
    }
}