using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atomic.UnifiedAuth.Web.Pages;

public class Index : PageModel
{
    public bool IsAuthenticated { get; set; }

    public void OnGet()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        IsAuthenticated = !string.IsNullOrEmpty(userId);
    }
}