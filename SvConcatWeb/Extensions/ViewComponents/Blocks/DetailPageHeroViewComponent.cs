using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class DetailPageHeroViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new DetailPageHeroViewModel();

        if (source?.Content is not DetailPageHero hero) return View(vm);

        vm = viewmodelFactory.CreateViewModel<DetailPageHero, DetailPageHeroViewModel>(hero);
        
        return View(vm);
    }
}