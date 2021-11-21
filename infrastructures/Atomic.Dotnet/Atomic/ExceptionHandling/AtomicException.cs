using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.ExceptionHandling;

public class AtomicException : Exception
{
    public static readonly AtomicException InternalServer500Exception = new(
        "Unexpected error occured.",
        "This is an internal server error, please try again later.");

    public AtomicException(string message, string? detail = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Detail = detail ?? "Please try again later or correct your operation.";
    }

    public string Detail { get; }

    public virtual bool ShouldLocalizeException => true;

    public virtual object[] MessageLocalizationParameters => Array.Empty<object>();

    public virtual object[] DetailLocalizationParameters => Array.Empty<object>();

    public virtual ProblemDetails ToProblemDetails()
    {
        return new ProblemDetails
        {
            Title = Message,
            Detail = Detail,
            Status = StatusCodes.Status500InternalServerError,
        };
    }
}