using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class ImageBlockToImageBlockViewModel(IVariationContextAccessor variationContextAccessor): IMappingStrategy<ImageBlock, ImageBlockViewModel>
{
    public ImageBlockViewModel Execute(ImageBlock source)
    {
        var vm = new ImageBlockViewModel();
        
        if (source?.Image == null) return vm;
        
        SetImage(vm, source);

        return vm;
    }
    
    private void SetImage(ImageBlockViewModel vm, ImageBlock source)
    {
        var mainCrops = new MainCropItem
        {
            Width = 1312
        };

        var culture = variationContextAccessor.VariationContext.Culture;

        vm.Image = source.Image.CreateImageItem(mainCrops, culture);
    }
}