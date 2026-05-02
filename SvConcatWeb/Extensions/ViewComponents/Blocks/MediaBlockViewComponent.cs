using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class MediaBlockViewComponent(IViewmodelFactory viewmodelFactory): ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new MediaBlockViewModel();

        if (source?.Content is not MediaBlock mediaBlock) return View(vm);

        vm = viewmodelFactory.CreateViewModel<MediaBlock, MediaBlockViewModel>(mediaBlock);

        return View(vm);
    }
}