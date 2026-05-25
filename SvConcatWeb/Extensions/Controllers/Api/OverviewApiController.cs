using Microsoft.AspNetCore.Mvc;

namespace SvConcatWeb.Extensions.Controllers.Api;

// TODO: this is a stub used while the real overview/events API is being built.
//       Replace the static seed data with real Umbraco content queries and move the
//       DTOs into Extensions/ViewModels (or wherever they're shared) when ready.
[ApiController]
[Route("umbraco/api/overview")]
[Produces("application/json")]
public class OverviewApiController : ControllerBase
{
    private static readonly IReadOnlyList<StubEvent> StubData = BuildStubData();

    [HttpGet("items")]
    public IActionResult GetItems(
        [FromQuery] int year,
        [FromQuery] string ordering = "newest",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        if (pageSize <= 0) pageSize = 12;
        if (pageSize > 100) pageSize = 100;

        var matching = StubData.Where(e => e.Date.Year == year);

        matching = ordering?.ToLowerInvariant() == "oldest"
            ? matching.OrderBy(e => e.Date).ThenBy(e => e.Title)
            : matching.OrderByDescending(e => e.Date).ThenBy(e => e.Title);

        var materialized = matching.ToList();
        var totalItems = materialized.Count;
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var clampedPage = Math.Clamp(page, 1, totalPages);

        var pageItems = materialized
            .Skip((clampedPage - 1) * pageSize)
            .Take(pageSize)
            .Select(e => e.ToCard())
            .ToList();

        var response = new OverviewResponse(
            Items: pageItems,
            Page: clampedPage,
            PageSize: pageSize,
            TotalPages: totalPages,
            TotalItems: totalItems);

        return Ok(response);
    }

    private static List<StubEvent> BuildStubData()
    {
        // Deterministic seed so devs see the same data across reloads.
        var random = new Random(42);

        var eventTypes = new[] { "Concert", "Festival", "Symposium", "Workshop", "Borrel", "Lezing", "Excursie", "Diner" };
        var locations = new[] { "Eindhoven", "Amsterdam", "Rotterdam", "Utrecht", "Den Haag", "Maastricht", "Groningen" };
        var palette = new[] { "#2b6cb0", "#2f855a", "#b7791f", "#9b2c2c", "#553c9a", "#285e61", "#702459" };

        var events = new List<StubEvent>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        for (var year = 2020; year <= today.Year; year++)
        {
            var count = random.Next(3, 14);
            for (var i = 0; i < count; i++)
            {
                var month = random.Next(1, 13);
                var day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
                var date = new DateOnly(year, month, day);
                var title = $"{eventTypes[random.Next(eventTypes.Length)]} #{i + 1}";
                var location = locations[random.Next(locations.Length)];
                var color = palette[random.Next(palette.Length)];

                events.Add(new StubEvent(date, title, location, color));
            }
        }

        return events;
    }

    private sealed record StubEvent(DateOnly Date, string Title, string Location, string Color)
    {
        public CardDto ToCard()
        {
            var formattedDate = Date.ToString("dd-MM-yyyy");
            var description =
                $"Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                $"Een stuk korte tekst over {Title.ToLowerInvariant()} in {Location}.";

            return new CardDto(
                Title: Title,
                Subtitle: $"{formattedDate} • {Location}",
                Description: description,
                Image: new ImageDto(
                    Url: BuildPlaceholderSvg(Title, Color),
                    Alt: $"Afbeelding voor {Title}",
                    Sources: Array.Empty<SourceDto>()),
                Cta: new LinkDto(
                    Url: $"/evenement/{Date:yyyy-MM-dd}/{Slugify(Title)}",
                    Name: "Bekijk",
                    Target: "_self"));
        }

        private static string BuildPlaceholderSvg(string text, string color)
        {
            var safeText = System.Net.WebUtility.HtmlEncode(text);
            var svg =
                $"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 800 450'>" +
                $"<rect width='800' height='450' fill='{color}'/>" +
                $"<text x='50%' y='50%' fill='white' font-family='sans-serif' font-size='40' " +
                $"text-anchor='middle' dominant-baseline='middle'>{safeText}</text>" +
                $"</svg>";

            return $"data:image/svg+xml;utf8,{Uri.EscapeDataString(svg)}";
        }

        private static string Slugify(string value) =>
            new string(value.ToLowerInvariant()
                .Select(c => char.IsLetterOrDigit(c) ? c : '-')
                .ToArray())
                .Trim('-');
    }

    // Response DTOs are intentionally kept in this file so the stub is self-contained.
    private sealed record OverviewResponse(
        IReadOnlyList<CardDto> Items,
        int Page,
        int PageSize,
        int TotalPages,
        int TotalItems);

    private sealed record CardDto(
        string Title,
        string Subtitle,
        string Description,
        ImageDto Image,
        LinkDto Cta);

    private sealed record ImageDto(string Url, string Alt, IReadOnlyList<SourceDto> Sources);

    private sealed record SourceDto(string Src, string MediaQuery);

    private sealed record LinkDto(string Url, string Name, string Target);
}
