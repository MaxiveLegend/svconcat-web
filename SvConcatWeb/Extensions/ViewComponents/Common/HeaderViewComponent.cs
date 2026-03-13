using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.Models;
using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace SvConcatWeb.Extensions.ViewComponents.Common;

public class HeaderViewComponent(IVariationContextAccessor variationContextAccessor, IViewmodelFactory viewModelFactory) : ViewComponent
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
        SetHeaderLogo(vm, model);

        return View(vm);
    }

    private void SetHeaderLogo(HeaderViewModel vm, IMasterModel model)
    {
        var mainCrops = new MainCropItem
        {
            Width = 195,
            Height = 52
        };

        var culture = variationContextAccessor.VariationContext.Culture;
        vm.WebsiteLogo = model.Website.SiteLogo.CreateImageItem(mainCrops, culture);
    }
}