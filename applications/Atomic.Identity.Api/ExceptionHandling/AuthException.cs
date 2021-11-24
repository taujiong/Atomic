using Atomic.ExceptionHandling;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.Identity.Api.ExceptionHandling;

public class AuthException : AtomicException
{
    public static readonly Dictionary<AuthError, string> AuthErrorMessageMap = new Dictionary<AuthError, string>
    {
        { AuthError.IsLockedOut, "The user is locked out for excessive login failure." },
        { AuthError.IsNotAllowed, "The user is not allowed to login." },
        { AuthError.WrongCredential, "Your login credential is not valid." },
    };

    public static readonly Dictionary<AuthError, string> AuthErrorDetailMap = new Dictionary<AuthError, string>
    {
        { AuthError.IsLockedOut, "Please hold on and try again later." },
        { AuthError.IsNotAllowed, "Please contact the administrator or try again later." },
        { AuthError.WrongCredential, "Please try again." },
    };

    public AuthException(AuthError authError, Exception? innerException = null)
        : base(AuthErrorMessageMap[authError], AuthErrorDetailMap[authError], innerException)
    {
        switch (authError)
        {
            case AuthError.IsLockedOut:
                StatusCode = StatusCodes.Status423Locked;
                break;
            case AuthError.IsNotAllowed:
                StatusCode = StatusCodes.Status403Forbidden;
                break;
            case AuthError.WrongCredential:
            default:
                StatusCode = StatusCodes.Status401Unauthorized;
                break;
        }
    }

    public int StatusCode { get; set; }

    public override ProblemDetails ToProblemDetails()
    {
        var details = base.ToProblemDetails();
        details.Status = StatusCode;

        return details;
    }
}

public enum AuthError
{
    IsLockedOut,
    IsNotAllowed,
    WrongCredential,
}