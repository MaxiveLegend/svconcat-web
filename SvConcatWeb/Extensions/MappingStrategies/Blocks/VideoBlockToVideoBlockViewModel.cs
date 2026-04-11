using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.Utilities;
using SvConcatWeb.Extensions.ViewModels.Blocks;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.MappingStrategies.Blocks;

public class VideoBlockToVideoBlockViewModel(IVariationContextAccessor variationContextAccessor) : IMappingStrategy<VideoBlock, VideoBlockViewModel>
{
    public VideoBlockViewModel Execute(VideoBlock source)
    {
        var vm = new VideoBlockViewModel();
        
        vm.VideoUrl = source.Video?.MediaUrl() ?? string.Empty;
        vm.StartTime = source.StartTime;
        vm.Duration = source.Duration;

        if (source.Thumbnail != null)
        {
            SetThumbnail(vm, source);
        }

        return vm;
    }

    public void SetThumbnail(VideoBlockViewModel vm, VideoBlock videoBlock)
    {
        var mainCrops = new MainCropItem
        {
            Width = 1600,
            Height = 900
        };
        
        var sources = new List<PictureSourceItem>
        {
            new ()
            {
                Width = 900,
                MediaQuery = "(max-width: 980px)",
                ResolutionScale = 1
            }
        };

        var culture = variationContextAccessor.VariationContext.Culture;

        vm.Thumbnail = videoBlock.Thumbnail.CreateImageItem(mainCrops, culture, sources);
    }
}