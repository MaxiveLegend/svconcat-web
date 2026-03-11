using Microsoft.AspNetCore.Mvc;
using svconcat_web.Extensions.Models;
using svconcat_web.Extensions.ViewModels;
using svconcat_web.Extensions.ViewModels.Common;
using svconcat_web.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;

namespace svconcat_web.Extensions.ViewComponents.Common;

public class HeaderViewComponent(IViewmodelFactory viewModelFactory) : ViewComponent
{
    public IViewComponentResult Invoke(IMasterModel source)
    {
        var vm = new HeaderViewModel();

        if (source?.Website == null) return View(vm);

        vm.CurrentPage = source.CurrentPage;
        vm.MainNavItems =
            source.Website.MainNavItems.Select(viewModelFactory.CreateViewModel<Link, LinkViewModel>);
        vm.ExternalLinks = source.Website.ExternalLinks.Select(viewModelFactory.CreateViewModel<Link, LinkViewModel>);
        vm.WebsiteUrl = source.Website.Url();

        return View(vm);
    }
}