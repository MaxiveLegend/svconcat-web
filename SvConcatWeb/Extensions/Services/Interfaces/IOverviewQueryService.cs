using SvConcatWeb.Extensions.ViewModels.Common.Overview;

namespace SvConcatWeb.Extensions.Services.Interfaces;

public interface IOverviewQueryService
{
    /// <summary>
    /// Returns the paginated, filtered and ordered set of card view models for the
    /// children of the page identified by <paramref name="pageKey"/>.
    /// Returns <c>null</c> when the page cannot be resolved from the published cache.
    /// </summary>
    OverviewResultViewModel? GetCards(Guid pageKey, int year, string ordering, int page, int pageSize);
}
