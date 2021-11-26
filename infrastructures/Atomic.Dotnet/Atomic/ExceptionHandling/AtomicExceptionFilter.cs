using System.Diagnostics;
using Atomic.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Atomic.ExceptionHandling;

public class AtomicExceptionFilter : IExceptionFilter
{
    private readonly ILogger<AtomicExceptionFilter> _logger;

    public AtomicExceptionFilter(ILogger<AtomicExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError("Error occured: {Message}\nStackTrace:\n{StackTrace}",
            context.Exception.Message,
            context.Exception.StackTrace);
        var exception = context.Exception is AtomicException atomicException
            ? atomicException
            : AtomicException.InternalServer500Exception;

        var problemDetails = exception.ToProblemDetails();
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        if (exception.ShouldLocalizeException)
        {
            var localizer = context.HttpContext.RequestServices
                .GetRequiredService<IStringLocalizerFactory>()
                .Create(typeof(AtomicSharedResource));
            problemDetails.Title = localizer[problemDetails.Title!, exception.MessageLocalizationParameters];
            problemDetails.Detail = localizer[problemDetails.Detail!, exception.DetailLocalizationParameters];
        }

        // TODO: log here or notify exception to Dapr
        context.Result = new ObjectResult(problemDetails);
        context.ExceptionHandled = true;
    }
}