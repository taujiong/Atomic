using Microsoft.AspNetCore.Mvc;

namespace Atomic.Identity.Api.Controllers;

[ApiController]
[Route("ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> OnGet()
    {
        return new ObjectResult("pong");
    }
}