using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class CtaBlockViewComponent(IViewmodelFactory viewmodelFactory): ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new CtaBlockViewModel();

        if (source?.Content is not CtaBlock cta) return View(vm);

        vm = viewmodelFactory.CreateViewModel<CtaBlock, CtaBlockViewModel>(cta);

        return View(vm);
    }
}