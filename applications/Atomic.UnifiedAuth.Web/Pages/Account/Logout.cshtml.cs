using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class Logout : AccountPageModel
{
    public async Task<ActionResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return RedirectSafely();
    }
}