using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class HeroViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem model)
    {
        var vm = new HeroViewModel();

        if (model == null) return View(vm);

        vm = viewmodelFactory.CreateViewModel<BlockListItem, HeroViewModel>(model);
        
        return View(vm);
    }
}