namespace SvConcatWeb.Extensions.ViewModels.Common.Overview;

public class OverviewResultViewModel
{
    public IReadOnlyList<CardViewModel> Items { get; set; } = [];
    public PaginationViewModel Pagination { get; set; } = new();
}
