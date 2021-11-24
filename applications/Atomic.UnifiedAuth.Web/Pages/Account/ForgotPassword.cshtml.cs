using Atomic.Identity;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class ForgotPassword : AccountPageModel
{
    private readonly DaprClient _daprClient;

    public ForgotPassword(DaprClient daprClient)
    {
        _daprClient = daprClient;
        Input = new RequireResetPasswordDto();
    }

    [BindProperty]
    public RequireResetPasswordDto Input { get; set; }

    public bool LinkSent { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var request = _daprClient.CreateInvokeMethodRequest(
            HttpMethod.Post,
            "Identity",
            "api/identity/passwords/reset-password",
            Input);
        var response = await _daprClient.InvokeMethodWithResponseAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return await PageWithError(response);
        }

        LinkSent = true;

        return Page();
    }
}