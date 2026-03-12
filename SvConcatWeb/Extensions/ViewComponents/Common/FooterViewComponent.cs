using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.Models;
using SvConcatWeb.Extensions.ViewModels.Common.Footer;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.Blocks;

namespace SvConcatWeb.Extensions.ViewComponents.Common;

public class FooterViewComponent(IViewmodelFactory viewmodelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(IMasterModel model)
    {
        var vm = new FooterViewModel();

        if (model?.Website == null) return View(vm);

        vm.Columns = model.Website.Columns.Select(viewmodelFactory.CreateViewModel<BlockListItem, FooterColumnViewModel>);

        return View(vm);
    }
}