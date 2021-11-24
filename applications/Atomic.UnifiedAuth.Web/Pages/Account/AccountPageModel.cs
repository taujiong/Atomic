using System.Security.Claims;
using Atomic.ExceptionHandling;
using Atomic.UnifiedAuth.Web.Localization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

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
        var localizer = HttpContext.RequestServices.GetRequiredService<IStringLocalizer<AtomicAuthResource>>();
        if (string.IsNullOrEmpty(ReturnUrl))
        {
            throw new UserFriendlyException(localizer["No return url found."]);
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
}