using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.ViewComponents.Blocks;

public class ImageBlockViewComponent(IVariationContextAccessor variationContextAccessor) : ViewComponent
{
    public IViewComponentResult Invoke(BlockListItem source)
    {
        var vm = new ImageBlockViewModel();

        if (source?.Content is not ImageBlock sourceContent) return View(vm);

        SetImage(vm, sourceContent);
        
        return View(vm);
    }

    private void SetImage(ImageBlockViewModel vm, ImageBlock sourceContent)
    {
        var mainCrops = new MainCropItem
        {
            Width = 1312
        };

        var culture = variationContextAccessor.VariationContext.Culture;

        vm.Image = sourceContent.Image.CreateImageItem(mainCrops, culture);
    }
}