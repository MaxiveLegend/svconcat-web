namespace SvConcatWeb.Extensions.ViewModels.Common.Overview;

public class PaginationViewModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}
