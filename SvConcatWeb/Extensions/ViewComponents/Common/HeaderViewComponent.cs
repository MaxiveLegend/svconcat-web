using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.Models;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;

namespace SvConcatWeb.Extensions.ViewComponents.Common;

public class HeaderViewComponent(IViewmodelFactory viewModelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(IMasterModel model)
    {
        var vm = new HeaderViewModel();

        if (model?.Website == null) return View(vm);
        
        vm.CurrentPage = model.CurrentPage;
        vm.MainNavItems =
            model.Website.MainNavItems.Select(viewModelFactory.CreateViewModel<Link, LinkViewModel>);
        vm.ExternalLinks = model.Website.ExternalLinks.Select(viewModelFactory.CreateViewModel<Link, LinkViewModel>);
        vm.WebsiteUrl = model.Website.Url();

        return View(vm);
    }
}