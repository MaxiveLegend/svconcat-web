using Microsoft.AspNetCore.Mvc;

namespace SvConcatWeb.Extensions.Controllers;

// Debug-only helper for exercising the 500 error page. Hit /debug/throw to
// trigger an unhandled exception. Remove (or gate behind an environment check)
// before going to production.
public class DebugController : Controller
{
    [Route("debug/throw")]
    public IActionResult Throw()
    {
        throw new InvalidOperationException("Deliberate test exception from /debug/throw.");
    }
}
