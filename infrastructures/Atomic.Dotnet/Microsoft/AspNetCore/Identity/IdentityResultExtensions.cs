using Atomic.ExceptionHandling;

namespace Microsoft.AspNetCore.Identity;

public static class IdentityResultExtensions
{
    public static void CheckErrors(this IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        if (result.Errors == null)
        {
            throw AtomicException.InternalServer500Exception;
        }

        throw new AtomicException(result.Errors.First().Description);
    }
}