using System.Security.Claims;
using Atomic.ExceptionHandling;
using Atomic.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class AccountPageModel : PageModel
{
    protected const string LoginProviderKey = "LoginProvider";

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? PageErrorMessage { get; protected set; }

    protected async Task<ActionResult> PageWithError(HttpResponseMessage response)
    {
        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        PageErrorMessage = error?.Title ?? AtomicException.DefaultExceptionMessage;

        return Page();
    }

    protected ActionResult RedirectSafely()
    {
        if (string.IsNullOrEmpty(ReturnUrl))
        {
            return RedirectToPage("/Index");
        }

        return Redirect(ReturnUrl);
    }

    protected async Task<ExternalLoginInfo?> GetExternalLoginInfoAsync()
    {
        var auth = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        var items = auth.Properties?.Items;

        if (auth.Principal == null || items == null || !items.ContainsKey(LoginProviderKey))
        {
            return null;
        }

        var provider = items[LoginProviderKey];
        var providerKey = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (provider == null || providerKey == null)
        {
            return null;
        }

        var schemeProvider = HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var providerDisplayName = (await schemeProvider.GetAllSchemesAsync())
            .FirstOrDefault(s => s.Name == provider && !string.IsNullOrEmpty(s.DisplayName))?.DisplayName ?? provider;

        return new ExternalLoginInfo(auth.Principal, provider, providerKey, providerDisplayName)
        {
            AuthenticationTokens = auth.Properties!.GetTokens(),
            AuthenticationProperties = auth.Properties,
        };
    }

    protected async Task SignInWithUserAsync(IdentityUserOutputDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id!),
            new(ClaimTypes.Name, user.UserName!),
        };
        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

        if (!string.IsNullOrEmpty(user.Email))
        {
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        }

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
        }

        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
    }
}