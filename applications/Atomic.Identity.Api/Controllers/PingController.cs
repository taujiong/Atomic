using Atomic.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.Identity.Api.Controllers;

public class PingController : AtomicControllerBase
{
    [HttpGet]
    public ActionResult<string> OnGet(string? name)
    {
        return string.IsNullOrEmpty(name) ? NotFound() : new ObjectResult($"pong, {name}");
    }
}