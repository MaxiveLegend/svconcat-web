using SvConcatWeb.Extensions.ViewModels.Common;

namespace SvConcatWeb.Extensions.ViewModels.Blocks;

public class DetailPageHeroViewModel
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public ImageViewModel Image { get; set; }
    public LinkViewModel Cta { get; set; }
}