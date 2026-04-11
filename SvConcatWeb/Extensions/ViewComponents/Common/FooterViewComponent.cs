using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.Models;
using SvConcatWeb.Extensions.ViewModels.Common.Footer;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Common;

public class FooterViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(IMasterModel source)
    {
        var vm = new FooterViewModel();

        if (source?.Website == null) return View(vm);

        vm.Columns = source.Website.Columns
            .Select(column => column?.Content as FooterColumn)
            .Select(viewmodelFactory.CreateViewModel<FooterColumn, FooterColumnViewModel>);

        return View(vm);
    }
}