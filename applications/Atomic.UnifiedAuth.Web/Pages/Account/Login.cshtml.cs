using System.Text.Json;
using Atomic.Identity;
using Dapr.Client;
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

    public async Task<ActionResult> OnPostAsync()
    {
        var result =
            await _daprClient.InvokeMethodAsync<PasswordLoginDto, IdentityUserOutputDto>(HttpMethod.Post, "Identity",
                "api/identity/logins/password", Input);
        _logger.LogInformation("result is {result}", JsonSerializer.Serialize(result));

        return Page();
    }
}