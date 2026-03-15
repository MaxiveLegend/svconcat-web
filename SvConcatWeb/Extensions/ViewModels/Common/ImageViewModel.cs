namespace SvConcatWeb.Extensions.ViewModels.Common;

public class ImageViewModel
{
    public string Url { get; set; }
    public string Alt { get; set; }
    public IEnumerable<PictureSourceViewModel> Sources { get; set; }
}