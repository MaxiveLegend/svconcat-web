using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class HeroViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new HeroViewModel();

        if (source?.Content is not Hero hero) return View(vm);

        vm = viewmodelFactory.CreateViewModel<Hero, HeroViewModel>(hero);
        
        return View(vm);
    }
}