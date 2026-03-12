using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class HeroToHeroViewModel(IVariationContextAccessor variationContextAccessor) : IMappingStrategy<BlockListItem, HeroViewModel>
{
    public HeroViewModel Execute(BlockListItem source)
    {
        var sourceContent = source?.Content as Hero;
        var vm = new HeroViewModel();
        
        if (sourceContent == null) return vm;

        vm.Title = sourceContent.Title ?? string.Empty;
        vm.Subtitle = sourceContent.Subtitle ?? string.Empty;
        
        SetHeroImage(vm, sourceContent);

        return vm;
    }

    private void SetHeroImage(HeroViewModel vm, Hero sourceContent)
    {
        var mainCrop = new MainCropItem
        {
            Width = 1920,
            Height = 500
        };

        var culture = variationContextAccessor.VariationContext.Culture;
        vm.Image = sourceContent.Image.CreateImageItem(mainCrop, culture);
    }
}