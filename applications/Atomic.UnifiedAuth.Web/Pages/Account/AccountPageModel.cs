using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atomic.UnifiedAuth.Web.Pages.Account;

public class AccountPageModel : PageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? PageErrorMessage { get; protected set; }
}