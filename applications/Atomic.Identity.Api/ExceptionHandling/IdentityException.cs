using Atomic.ExceptionHandling;

namespace Atomic.Identity.Api.ExceptionHandling;

public class IdentityException : AtomicException
{
    public IdentityException(string message, string? detail = null, Exception? innerException = null)
        : base(message, detail, innerException)
    {
    }

    public override bool ShouldLocalizeException => false;
}