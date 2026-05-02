using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class ImageBlockViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new ImageBlockViewModel();

        if (source?.Content is not ImageBlock sourceContent) return View(vm);

        vm = viewmodelFactory.CreateViewModel<ImageBlock, ImageBlockViewModel>(sourceContent);
        
        return View(vm);
    }
}