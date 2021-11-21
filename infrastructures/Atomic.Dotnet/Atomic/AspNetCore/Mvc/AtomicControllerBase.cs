using Microsoft.AspNetCore.Mvc;

namespace Atomic.AspNetCore.Mvc;

[ApiController]
[Route("/api/app/[controller]")]
public class AtomicControllerBase : ControllerBase
{
}