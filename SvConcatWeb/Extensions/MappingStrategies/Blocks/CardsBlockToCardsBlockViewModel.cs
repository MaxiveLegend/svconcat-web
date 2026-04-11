using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class CardsBlockToCardsBlockViewModel(IViewmodelFactory viewmodelFactory) : IMappingStrategy<CardsBlock, CardsBlockViewModel>
{
    public CardsBlockViewModel Execute(CardsBlock source)
    {
        var vm = new CardsBlockViewModel();

        if (source == null) return vm;

        vm.Title = source.Title ?? string.Empty;
        vm.Text = source.Text ?? string.Empty;
        vm.Cards = source.Cards.OfType<ICard>().Select(viewmodelFactory.CreateViewModel<ICard, CardViewModel>);
        vm.Cta = viewmodelFactory.CreateViewModel<Link, LinkViewModel>(source.Cta);

        return vm;
    }
}