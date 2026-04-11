using SvConcatWeb.Extensions.ViewModels.Common;

namespace SvConcatWeb.Extensions.ViewModels.Blocks;

public class CtaBlockViewModel
{
    public string Title { get; set; }
    public string Text { get; set; }
    public IEnumerable<LinkViewModel> Links { get; set; }
}