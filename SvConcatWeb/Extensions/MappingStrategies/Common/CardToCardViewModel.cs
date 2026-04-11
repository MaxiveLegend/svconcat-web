using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Common;

public class CardToCardViewModel(IVariationContextAccessor variationContextAccessor, IViewmodelFactory viewmodelFactory) : IMappingStrategy<ICard, CardViewModel>
{
    public CardViewModel Execute(ICard source)
    {
        var vm = new CardViewModel();

        if (source == null) return vm;

        vm.Title = source.CardTitle ?? string.Empty;
        vm.Subtitle = source.CardSubtitle ?? string.Empty;
        vm.Description = source.CardDescription ?? string.Empty;
        vm.Cta = viewmodelFactory.CreateViewModel<Link, LinkViewModel>(source.CardLink);
        SetImage(vm, source);

        return vm;
    }

    private void SetImage(CardViewModel vm, ICard card)
    {
        var mainCrops = new MainCropItem
        {
            Width = 350,
            Height = 207
        };

        var culture = variationContextAccessor.VariationContext.Culture;
        
        vm.Image = card.CardImage.CreateImageItem(mainCrops, culture);
    }
}