using Microsoft.AspNetCore.Mvc;
using svconcat_web.Extensions.ViewModels;
using svconcat_web.Extensions.ViewModels.Blocks;
using svconcat_web.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;

namespace svconcat_web.Extensions.ViewComponents.Blocks;

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