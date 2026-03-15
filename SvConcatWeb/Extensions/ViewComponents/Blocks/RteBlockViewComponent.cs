using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class RteBlockViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new RteBlockViewModel();

        if (source?.Content is not RteBlock) return View(vm);

        vm = viewmodelFactory.CreateViewModel<BlockListItem, RteBlockViewModel>(source);

        return vm.HasContent ? View(vm) : Content(string.Empty);
    }
}