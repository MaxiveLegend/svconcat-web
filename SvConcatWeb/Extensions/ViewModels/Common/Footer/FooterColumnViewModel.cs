namespace SvConcatWeb.Extensions.ViewModels.Common.Footer;

public class FooterColumnViewModel
{
    public string ColumnName { get; set; }
    public IEnumerable<FooterColumnItemViewModel> items { get; set; }
}