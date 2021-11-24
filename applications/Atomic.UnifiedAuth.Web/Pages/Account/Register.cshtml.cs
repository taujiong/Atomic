using Atomic.Identity;
using Atomic.UnifiedAuth.Web.Localization;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class Register : AccountPageModel
{
    private readonly DaprClient _daprClient;
    private readonly IStringLocalizer<AtomicAuthResource> _localizer;
    private readonly ILogger<Register> _logger;

    public Register(
        ILogger<Register> logger,
        IStringLocalizer<AtomicAuthResource> localizer,
        DaprClient daprClient
    )
    {
        _logger = logger;
        _localizer = localizer;
        _daprClient = daprClient;
        Input ??= new IdentityUserCreateDto();
    }

    [BindProperty]
    public IdentityUserCreateDto Input { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IsExternal { get; set; }

    public ActionResult OnGet(string userName, string email)
    {
        Input.UserName = userName;
        Input.Email = email;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var request = _daprClient.CreateInvokeMethodRequest(
            HttpMethod.Post,
            "Identity",
            "api/identity/users",
            Input);
        var response = await _daprClient.InvokeMethodWithResponseAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return await PageWithError(response);
        }

        if (IsExternal)
        {
            var loginInfo = await GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                const string message = "Error loading external login information";
                _logger.LogWarning(message);
                PageErrorMessage = _localizer[message];

                return Page();
            }

            // TODO: add external login via dapr
        }

        return RedirectSafely();
    }
}