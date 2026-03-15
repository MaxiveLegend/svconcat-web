using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Common;

public class CardToCardViewModel(IVariationContextAccessor variationContextAccessor, IViewmodelFactory viewmodelFactory) : IMappingStrategy<IPublishedContent, CardViewModel>
{
    public CardViewModel Execute(IPublishedContent source)
    {
        var vm = new CardViewModel();

        if (source is not ICard card) return vm;

        vm.Title = card.CardTitle ?? string.Empty;
        vm.Subtitle = card.CardSubtitle ?? string.Empty;
        vm.Description = card.CardDescription ?? string.Empty;
        vm.Cta = viewmodelFactory.CreateViewModel<Link, LinkViewModel>(card.CardLink);
        SetImage(vm, card);

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