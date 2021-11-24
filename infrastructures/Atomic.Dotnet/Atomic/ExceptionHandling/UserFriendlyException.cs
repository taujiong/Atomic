namespace Atomic.ExceptionHandling;

public class UserFriendlyException : AtomicException
{
    public UserFriendlyException(string message, string? detail = null, Exception? innerException = null)
        : base(message, detail, innerException)
    {
    }

    public override bool ShouldLocalizeException => false;
}