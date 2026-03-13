namespace SvConcatWeb.Extensions.ViewModels.Common.Footer;

public class FooterColumnItemViewModel
{
    public ImageItemViewModel Icon { get; set; }
    public LinkViewModel Link { get; set; }
    public string Text { get; set; }
    
    public bool HasIcon => Icon?.Url != null;
    public bool HasLink => Link?.Url != null;
    public bool HasText => !string.IsNullOrWhiteSpace(Text);
}