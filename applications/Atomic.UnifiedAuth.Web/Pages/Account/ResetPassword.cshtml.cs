using Atomic.ExceptionHandling;
using Atomic.Identity;
using Atomic.UnifiedAuth.Web.Localization;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class ResetPassword : AccountPageModel
{
    private readonly DaprClient _daprClient;
    private readonly IStringLocalizer<AtomicAuthResource> _localizer;

    public ResetPassword(
        IStringLocalizer<AtomicAuthResource> localizer,
        DaprClient daprClient
    )
    {
        _localizer = localizer;
        _daprClient = daprClient;
        Input = new ResetPasswordDto();
    }

    [BindProperty(SupportsGet = true)]
    public ResetPasswordDto Input { get; set; }

    public bool PasswordReset { get; set; }

    public IActionResult OnGet(string token, string userId)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            throw new UserFriendlyException(_localizer["Wrong password reset link."]);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var request = _daprClient.CreateInvokeMethodRequest(
            HttpMethod.Put,
            "Identity",
            "api/identity/passwords/reset-password",
            Input);
        var response = await _daprClient.InvokeMethodWithResponseAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return await PageWithError(response);
        }

        PasswordReset = true;
        return Page();
    }
}