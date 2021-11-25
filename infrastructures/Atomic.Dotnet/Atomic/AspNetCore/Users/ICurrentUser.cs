using System.Security.Claims;

namespace Atomic.AspNetCore.Users;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    string? Id { get; }

    string? UserName { get; }

    string? PhoneNumber { get; }

    bool PhoneNumberVerified { get; }

    string? Email { get; }

    bool EmailVerified { get; }

    Claim? FindClaim(string claimType);

    Claim[] FindClaims(string claimType);

    Claim[] GetAllClaims();
}