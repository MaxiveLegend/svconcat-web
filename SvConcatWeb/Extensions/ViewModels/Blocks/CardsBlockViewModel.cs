using SvConcatWeb.Extensions.ViewModels.Common;

namespace SvConcatWeb.Extensions.ViewModels.Blocks;

public class CardsBlockViewModel
{
    public string Title { get; set; }
    public string Text { get; set; }
    public LinkViewModel Cta { get; set; }
    public IEnumerable<CardViewModel> Cards { get; set; }
    
    public bool HasTitle => !string.IsNullOrWhiteSpace(Title);
    public bool HasText => !string.IsNullOrWhiteSpace(Text);
    public bool HasLink => !string.IsNullOrWhiteSpace(Cta?.Url);
    public bool HasBody => HasText || HasLink;
}