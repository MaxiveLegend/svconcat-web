using Microsoft.AspNetCore.Mvc;
using SvConcatWeb.Extensions.Services.Interfaces;

namespace SvConcatWeb.Extensions.Controllers.Api;

[ApiController]
[Route("umbraco/api/overview")]
[Produces("application/json")]
public class OverviewApiController(IOverviewQueryService overviewQueryService) : ControllerBase
{
    [HttpGet("items")]
    public IActionResult GetItems(
        [FromQuery] Guid pageKey,
        [FromQuery] int year,
        [FromQuery] string ordering = "newest",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        if (pageKey == Guid.Empty)
        {
            return BadRequest(new { error = "pageKey is required." });
        }

        var result = overviewQueryService.GetCards(pageKey, year, ordering, page, pageSize);
        if (result == null)
        {
            return NotFound(new { error = "Overview page not found." });
        }

        return Ok(result);
    }
}
