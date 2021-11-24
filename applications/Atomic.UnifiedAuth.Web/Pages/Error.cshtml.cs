using Atomic.ExceptionHandling;
using Atomic.Localization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Atomic.UnifiedAuth.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class Error : PageModel
{
    public string? Path { get; set; }
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        var exceptionInfo = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        Path = exceptionInfo?.Path;

        var exception = exceptionInfo?.Error is AtomicException atomicException
            ? atomicException
            : AtomicException.InternalServer500Exception;

        if (exception.ShouldLocalizeException)
        {
            var localizer = HttpContext.RequestServices.GetRequiredService<IStringLocalizer<AtomicSharedResource>>();
            ErrorMessage = localizer[exception.Message];
        }
        else
        {
            ErrorMessage = exception.Message;
        }
    }
}