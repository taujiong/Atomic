using System.Net;
using System.Security.Claims;
using Atomic.ExceptionHandling;
using Atomic.Identity;
using Atomic.UnifiedAuth.Web.Localization;
using Dapr.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class ExternalLogin : AccountPageModel
{
    private readonly DaprClient _daprClient;
    private readonly IStringLocalizer<AtomicAuthResource> _localizer;
    private readonly ILogger<ExternalLogin> _logger;

    public ExternalLogin(
        ILogger<ExternalLogin> logger,
        IStringLocalizer<AtomicAuthResource> localizer,
        DaprClient daprClient
    )
    {
        _logger = logger;
        _localizer = localizer;
        _daprClient = daprClient;
    }

    public ActionResult OnPost(string scheme)
    {
        var redirectUrl = Url.Page("./ExternalLogin", "Callback", new { ReturnUrl });
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
            Items =
            {
                [LoginProviderKey] = scheme,
            },
        };

        return new ChallengeResult(scheme, properties);
    }

    public async Task<ActionResult> OnGetCallbackAsync(string? remoteError = null)
    {
        if (remoteError != null)
        {
            _logger.LogWarning("External login failed: {reason}", remoteError);
            throw AtomicException.InternalServer500Exception;
        }

        var loginInfo = await GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            const string message = "Error loading external login info";
            _logger.LogWarning(message);
            throw new UserFriendlyException(_localizer[message]);
        }

        // sign in the user with external login provider first.
        var request = _daprClient.CreateInvokeMethodRequest(
            HttpMethod.Post,
            "Identity",
            "api/identity/logins/external",
            new ExternalLoginDto
            {
                LoginProvider = loginInfo.LoginProvider,
                ProviderKey = loginInfo.ProviderKey,
            });
        var response = await _daprClient.InvokeMethodWithResponseAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return RedirectSafely();
        }

        // 404 represents that the user is not found or registered.
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            var userName = loginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            return RedirectToPage("./Register", new
            {
                IsExternal = true,
                userName,
                email,
                ReturnUrl,
            });
        }

        return await PageWithError(response);
    }
}