using Atomic.ExceptionHandling;
using Microsoft.AspNetCore.Identity;

namespace Atomic.Identity.Api.Extensions;

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

        throw new UserFriendlyException(result.Errors.First().Description);
    }
}