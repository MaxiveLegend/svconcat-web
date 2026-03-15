using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class CardsBlockToCardsBlockViewModel(IViewmodelFactory viewmodelFactory) : IMappingStrategy<BlockListItem, CardsBlockViewModel>
{
    public CardsBlockViewModel Execute(BlockListItem source)
    {
        var vm = new CardsBlockViewModel();

        if (source?.Content is not CardsBlock sourceContent) return vm;

        vm.Title = sourceContent.Title ?? string.Empty;
        vm.Text = sourceContent.Text ?? string.Empty;
        vm.Cards = sourceContent.Cards.Select(viewmodelFactory.CreateViewModel<IPublishedContent, CardViewModel>);
        vm.Cta = viewmodelFactory.CreateViewModel<Link, LinkViewModel>(sourceContent.Cta);

        return vm;
    }
}