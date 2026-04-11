using SvConcatWeb.Extensions.ViewModels.Common;

namespace SvConcatWeb.Extensions.ViewModels.Blocks;

public class VideoBlockViewModel
{
    public string VideoUrl { get; set; }
    public ImageViewModel? Thumbnail { get; set; }
    public int StartTime { get; set; }
    public int Duration { get; set; }
    
    public bool HasThumbnail => Thumbnail != null;
    
    public string GetFullUrl
    {
        get
        {
            var src = $"{VideoUrl}#t={StartTime}";
            if (Duration > 0) src += $",{StartTime + Duration}";
            return src;
        }
    }
}