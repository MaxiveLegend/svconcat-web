using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModels.Common.Footer;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Common;

public class FooterColumnItemToFooterColumnItemViewModel(IVariationContextAccessor variationContextAccessor, IViewmodelFactory viewmodelFactory) : IMappingStrategy<BlockListItem, FooterColumnItemViewModel>
{
    public FooterColumnItemViewModel Execute(BlockListItem source)
    {
        var vm = new FooterColumnItemViewModel();

        if (source?.Content is not FooterItem sourceItem) return vm;
        
        vm.Link = viewmodelFactory.CreateViewModel<Link, LinkViewModel>(sourceItem.Link);
        vm.Text = sourceItem.Text ?? string.Empty;
        SetIcon(vm, sourceItem);

        return vm;
    }

    private void SetIcon(FooterColumnItemViewModel vm, FooterItem sourceItem)
    {
        var mainCrop = new MainCropItem
        {
            Width = 32,
            Height = 32
        };

        var culture = variationContextAccessor.VariationContext.Culture;
        vm.Icon = sourceItem.Icon.CreateImageItem(mainCrop, culture);
    }
}