using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class Logout : AccountPageModel
{
    private readonly IIdentityServerInteractionService _interaction;

    public Logout(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    public string? PostLogoutRedirectUri { get; set; }

    public string? ClientName { get; set; }

    public string? SignOutIframeUrl { get; set; }

    public async Task<ActionResult> OnGetAsync(string? logoutId)
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        await HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);

        if (string.IsNullOrEmpty(logoutId))
        {
            return RedirectSafely();
        }

        var logoutContext = await _interaction.GetLogoutContextAsync(logoutId);
        PostLogoutRedirectUri = logoutContext.PostLogoutRedirectUri;
        ClientName = logoutContext.ClientName ?? logoutContext.ClientId;
        SignOutIframeUrl = logoutContext.SignOutIFrameUrl;

        return Page();
    }
}