using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class DetailPageHeroToDetailPageHeroViewModel(IViewmodelFactory viewmodelFactory, IVariationContextAccessor variationContextAccessor) : IMappingStrategy<DetailPageHero, DetailPageHeroViewModel>
{
    public DetailPageHeroViewModel Execute(DetailPageHero source)
    {
        var vm = new DetailPageHeroViewModel();
        
        if (source == null) return vm;

        vm.Title = source.Title ?? string.Empty;
        vm.Subtitle = source.Subtitle ?? string.Empty;
        vm.Cta = viewmodelFactory.CreateViewModel<Link, LinkViewModel>(source.Cta);
        
        SetHeroImage(vm, source);

        return vm;
    }

    private void SetHeroImage(DetailPageHeroViewModel vm, DetailPageHero sourceContent)
    {
        var mainCrop = new MainCropItem
        {
            Width = 1920,
            Height = 500
        };

        var sources = new List<PictureSourceItem>
        {
            new ()
            {
                Width = 768,
                MediaQuery = "(max-width: 768px)",
                ResolutionScale = 1
            },
            new ()
            {
                Width = 1440,
                MediaQuery = "(max-width: 1440px)",
                ResolutionScale = 1
            }
        };

        var culture = variationContextAccessor.VariationContext.Culture;
        vm.Image = sourceContent.Image.CreateImageItem(mainCrop, culture, sources);
    }
}