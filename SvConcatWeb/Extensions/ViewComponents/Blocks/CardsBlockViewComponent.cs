using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class CardsBlockViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new CardsBlockViewModel();

        if (source?.Content is not CardsBlock cardsBlock) return View(vm);

        vm = viewmodelFactory.CreateViewModel<CardsBlock, CardsBlockViewModel>(cardsBlock);
        
        return View(vm);
    }
}