using Atomic.Identity.Api.ExceptionHandling;
using Microsoft.AspNetCore.Identity;

namespace Atomic.Identity.Api.Extensions;

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
            throw new SignInException(AuthError.IsLockedOut);
        }

        if (result.IsNotAllowed)
        {
            throw new SignInException(AuthError.IsNotAllowed);
        }

        if (!result.RequiresTwoFactor)
        {
            throw new SignInException(AuthError.WrongCredential);
        }
    }
}