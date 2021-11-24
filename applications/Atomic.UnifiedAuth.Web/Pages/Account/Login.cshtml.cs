using Atomic.ExceptionHandling;
using Atomic.Identity;
using Dapr.Client;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class Login : AccountPageModel
{
    private readonly DaprClient _daprClient;
    private readonly IIdentityServerInteractionService _interaction;

    public Login(DaprClient daprClient, IIdentityServerInteractionService interaction)
    {
        Input ??= new PasswordLoginDto();
        _daprClient = daprClient;
        _interaction = interaction;
    }

    [BindProperty]
    public PasswordLoginDto Input { get; set; }

    public async Task OnGetAsync()
    {
        // clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);
        Input.UserNameOrEmail = context?.LoginHint;
    }

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var request = _daprClient.CreateInvokeMethodRequest(
            HttpMethod.Post,
            "Identity",
            "api/identity/logins/password",
            Input);
        var response = await _daprClient.InvokeMethodWithResponseAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<IdentityUserOutputDto>();
            if (user == null) throw AtomicException.InternalServer500Exception;
            await SignInWithUserAsync(user);

            return RedirectSafely();
        }

        return await PageWithError(response);
    }

    public async Task<ActionResult> OnGetCancelAsync()
    {
        var context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);
        if (context == null)
        {
            return Redirect("/Index");
        }

        await _interaction.GrantConsentAsync(context, new ConsentResponse
        {
            Error = AuthorizationError.LoginRequired,
        });

        return RedirectSafely();
    }
}