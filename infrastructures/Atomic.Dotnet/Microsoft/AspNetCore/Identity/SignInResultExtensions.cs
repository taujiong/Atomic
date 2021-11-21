using Atomic.ExceptionHandling;

namespace Microsoft.AspNetCore.Identity;

public static class SignInResultExtensions
{
    public static void CheckErrors(this SignInResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        if (result.IsLockedOut)
        {
            throw new AuthException(AuthError.IsLockedOut);
        }

        if (result.IsNotAllowed)
        {
            throw new AuthException(AuthError.IsNotAllowed);
        }

        if (!result.RequiresTwoFactor)
        {
            throw new AuthException(AuthError.WrongCredential);
        }
    }
}