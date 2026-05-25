using SvConcatWeb.Extensions.Services.Interfaces;
using SvConcatWeb.Extensions.ViewModels.Common;
using SvConcatWeb.Extensions.ViewModels.Common.Overview;
using SvConcatWeb.Extensions.ViewModelStrategy.Interfaces;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;

namespace SvConcatWeb.Extensions.Services;

public class OverviewQueryService(
    IUmbracoContextAccessor umbracoContextAccessor,
    IViewmodelFactory viewmodelFactory) : IOverviewQueryService
{
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 12;

    private const string EventDateAlias = "eventDate";

    public OverviewResultViewModel? GetCards(Guid pageKey, int year, string ordering, int page, int pageSize)
    {
        pageSize = pageSize <= 0 ? DefaultPageSize : Math.Clamp(pageSize, MinPageSize, MaxPageSize);

        if (!umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
        {
            return null;
        }

        var pageContent = umbracoContext.Content?.GetById(pageKey);
        if (pageContent == null)
        {
            return null;
        }

        var matching = pageContent
            .Children<OverviewDetailPage>()
            .Where(child => child.IsVisible())
            .Where(child => GetEventDate(child).Year == year)
            .ToList();

        var ordered = string.Equals(ordering, "oldest", StringComparison.OrdinalIgnoreCase)
            ? matching.OrderBy(GetEventDate)
            : matching.OrderByDescending(GetEventDate);

        var totalItems = matching.Count;
        var totalPages = totalItems == 0
            ? 1
            : (int)Math.Ceiling(totalItems / (double)pageSize);
        var clampedPage = Math.Clamp(page <= 0 ? 1 : page, 1, totalPages);

        var cards = ordered
            .Skip((clampedPage - 1) * pageSize)
            .Take(pageSize)
            .Select(child => viewmodelFactory.CreateViewModel<ICard, CardViewModel>(child))
            .ToList();

        return new OverviewResultViewModel
        {
            Items = cards,
            Pagination = new PaginationViewModel
            {
                Page = clampedPage,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems,
                HasPrevious = clampedPage > 1,
                HasNext = clampedPage < totalPages,
            },
        };
    }

    // Read via the alias rather than the strongly-typed property so this compiles
    // before/after ModelsBuilder regenerates with the new `eventDate` property.
    private static DateTime GetEventDate(IPublishedContent content) =>
        content.Value<DateTime>(EventDateAlias);
}
