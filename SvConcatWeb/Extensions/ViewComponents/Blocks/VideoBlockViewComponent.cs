using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class VideoBlockViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new VideoBlockViewModel();

        if (source?.Content is not VideoBlock videoBlock) return View(vm);

        vm = viewmodelFactory.CreateViewModel<VideoBlock, VideoBlockViewModel>(videoBlock);
        
        return View(vm);
    }
}