using SvConcatWeb.Extensions.ViewModels.Common;

namespace SvConcatWeb.Extensions.ViewModels.Blocks;

public class MediaBlockViewModel
{
    public string Title { get; set; }
    public string Text { get; set; }
    public LinkViewModel Cta { get; set; }
    public ImageViewModel Media { get; set; }
}