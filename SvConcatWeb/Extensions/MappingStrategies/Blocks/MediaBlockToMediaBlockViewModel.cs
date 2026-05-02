using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class MediaBlockToMediaBlockViewModel(IViewmodelFactory viewmodelFactory, IVariationContextAccessor variationContextAccessor): IMappingStrategy<MediaBlock, MediaBlockViewModel>
{
    public MediaBlockViewModel Execute(MediaBlock source)
    {
        var vm = new MediaBlockViewModel();
        
        if (source == null) return vm;

        vm.Title = source.Title ?? string.Empty;
        vm.Text = source.Text ?? string.Empty;
        vm.Cta = viewmodelFactory.CreateViewModel<Link, LinkViewModel>(source.Cta);
        SetMedia(vm, source);

        return vm;
    }

    private void SetMedia(MediaBlockViewModel vm, MediaBlock source)
    {
        var mainCrops = new MainCropItem
        {
            Width = 1600,
            Height = 900
        };

        var culture = variationContextAccessor.VariationContext.Culture;

        vm.Media = source.Media.CreateImageItem(mainCrops, culture);
    }
}