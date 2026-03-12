using SvConcatWeb.Extensions.Models.Custom;
using SvConcatWeb.Extensions.ViewModels.Common;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace SvConcatWeb.Extensions.Utilities;

public static class ExtensionMethods
{

    #region IPublishedContent

    public static Website Website(this IPublishedContent content)
    {
        return content.AncestorOrSelf(1) as Website;
    }

    #endregion

    #region MediaWithCrops

    private static string AltText(this MediaWithCrops mediaItem)
    {
        var altText = string.Empty;
        if (mediaItem?.Content is AltTaggable altTaggable)
        {
            altText = altTaggable.AltTag ?? string.Empty;
        }

        return altText;
    }

    private static string TryGetCropUrl(this MediaWithCrops mediaItem, int? width = null, int? height = null,
        ImageCropMode? imageCropMode = ImageCropMode.Crop, bool preferFocalPoint = true, string resolutionScale = "")
    {
        var url = string.Empty;

        if (mediaItem == null) return url;

        try
        {
            var options = mediaItem.ContentType.Alias.Equals(Image.ModelTypeAlias) ? "&format=webp" : string.Empty;

            var cropUrl = mediaItem.GetCropUrl(width, height, preferFocalPoint: preferFocalPoint,
                imageCropMode: imageCropMode, furtherOptions: options);

            url = !string.IsNullOrEmpty(cropUrl) ? cropUrl + resolutionScale : string.Empty;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.ToString());
        }

        return url;
    }

    public static ImageItemViewModel CreateImageItem(this MediaWithCrops mediaItem, MainCropItem mainCrop, string culture,
        IEnumerable<PictureSourceItem> sources = null)
    {
        var vm = new ImageItemViewModel();
        if (mediaItem == null || mainCrop == null) return vm;

        var pictureSources = new List<PictureSourceViewModel>();

        if (sources?.Any() == true)
        {
            foreach (var source in sources)
            {
                pictureSources.Add(CreateSourceItem(mediaItem, source));
            }
        }

        var altText = mediaItem.AltText();

        return new ImageItemViewModel
        {
            Url = mediaItem.TryGetCropUrl(mainCrop.Width, mainCrop.Height),
            Alt = altText,
            Sources = pictureSources
        };
    }

    private static PictureSourceViewModel CreateSourceItem(MediaWithCrops image, PictureSourceItem source)
    {
        var src = image.TryGetCropUrl();

        if (string.IsNullOrEmpty(src) || source.ResolutionScale <= 1)
        {
            return new PictureSourceViewModel
            {
                Src = src,
                MediaQuery = source.MediaQuery
            };
        }

        for (var i = 2; i <= source.ResolutionScale; i++)
        {
            var width = source.Width * i;
            var height = source.Height * i;

            var resCropUrl = image.TryGetCropUrl();

            if (!string.IsNullOrEmpty(resCropUrl)) src += $",{resCropUrl}";
        }

        return new PictureSourceViewModel
        {
            Src = src,
            MediaQuery = source.MediaQuery
        };
    }

    #endregion
}