namespace SvConcatWeb.Extensions.ViewModels.Common;

public class CardViewModel
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; }
    public ImageViewModel Image { get; set; }
    public LinkViewModel Cta { get; set; }
    
    public bool HasSubtitle => !string.IsNullOrEmpty(Subtitle);
    public bool HasDescription => !string.IsNullOrEmpty(Description);
    public bool HasCta => !string.IsNullOrWhiteSpace(Cta.Url);
}