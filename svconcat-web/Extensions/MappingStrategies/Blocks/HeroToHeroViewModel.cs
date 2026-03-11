using svconcat_web.Extensions.Models.Custom;
using svconcat_web.Extensions.Utilities;
using svconcat_web.Extensions.ViewModels;
using svconcat_web.Extensions.ViewModels.Blocks;
using svconcat_web.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace svconcat_web.Extensions.MappingStrategies.Blocks;

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