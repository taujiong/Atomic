using Microsoft.AspNetCore.Mvc;

namespace Atomic.AspNetCore.Mvc;

[ApiController]
[Route("/api/app/[controller]")]
[ApiConventionType(typeof(AtomicApiConventions))]
public class AtomicControllerBase : ControllerBase
{
}