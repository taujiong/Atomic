using Atomic.ExceptionHandling;
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

        var createUserRequest = _daprClient.CreateInvokeMethodRequest(
            HttpMethod.Post,
            "Identity",
            "api/identity/users",
            Input);
        var createUserResponse = await _daprClient.InvokeMethodWithResponseAsync(createUserRequest);

        if (!createUserResponse.IsSuccessStatusCode)
        {
            return await PageWithError(createUserResponse);
        }

        // now we create the user successfully.
        // if this is an external login, we should remember it.
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

            var newUser = await createUserResponse.Content.ReadFromJsonAsync<IdentityUserOutputDto>();
            if (newUser == null)
            {
                // this is not supposed to happen, actually.
                throw AtomicException.InternalServer500Exception;
            }

            var addLoginRequest = _daprClient.CreateInvokeMethodRequest(
                HttpMethod.Post,
                "Identity",
                "api/identity/external-logins",
                new ExternalLoginCreateDto
                {
                    UserId = newUser.Id,
                    LoginProvider = loginInfo.LoginProvider,
                    ProviderKey = loginInfo.ProviderKey,
                    DisplayName = loginInfo.ProviderDisplayName,
                });
            var addLoginResponse = await _daprClient.InvokeMethodWithResponseAsync(addLoginRequest);
            if (!addLoginResponse.IsSuccessStatusCode)
            {
                return await PageWithError(addLoginResponse);
            }
        }

        var user = await createUserResponse.Content.ReadFromJsonAsync<IdentityUserOutputDto>();
        if (user == null) throw AtomicException.InternalServer500Exception;
        await SignInWithUserAsync(user);

        return RedirectSafely();
    }
}