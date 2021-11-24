using Atomic.Identity;
using Dapr.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class Login : AccountPageModel
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<Login> _logger;

    public Login(DaprClient daprClient, ILogger<Login> logger)
    {
        Input ??= new PasswordLoginDto();
        _daprClient = daprClient;
        _logger = logger;
    }

    [BindProperty]
    public PasswordLoginDto Input { get; set; }

    public async Task OnGetAsync()
    {
        // clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
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
            return RedirectSafely();
        }

        return await PageWithError(response);
    }
}