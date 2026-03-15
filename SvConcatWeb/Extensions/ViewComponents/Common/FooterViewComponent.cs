using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.Models;
using SvConcatWeb.Extensions.ViewModels.Common.Footer;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;

namespace SvConcatWeb.Extensions.ViewComponents.Common;

public class FooterViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(IMasterModel source)
    {
        var vm = new FooterViewModel();

        if (source?.Website == null) return View(vm);

        vm.Columns = source.Website.Columns.Select(viewmodelFactory.CreateViewModel<BlockListItem, FooterColumnViewModel>);

        return View(vm);
    }
}