using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace Atomic.AspNetCore.Users;

public class CurrentUser : ICurrentUser
{
    private static readonly Claim[] EmptyClaimArray = Array.Empty<Claim>();

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(Id);

    public string? Id => FindClaim(JwtClaimTypes.Id)?.Value;

    public string? UserName => FindClaim(JwtClaimTypes.PreferredUserName)?.Value;

    public string? PhoneNumber => FindClaim(JwtClaimTypes.PhoneNumber)?.Value;

    public bool PhoneNumberVerified => string.Equals(FindClaim(JwtClaimTypes.PhoneNumberVerified)?.Value, "true",
        StringComparison.InvariantCulture);

    public string? Email => FindClaim(JwtClaimTypes.Email)?.Value;

    public bool EmailVerified => string.Equals(FindClaim(JwtClaimTypes.EmailVerified)?.Value, "true",
        StringComparison.InvariantCulture);

    public Claim? FindClaim(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public Claim[] FindClaims(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User.Claims.Where(c => c.Type == claimType).ToArray() ??
               EmptyClaimArray;
    }

    public Claim[] GetAllClaims()
    {
        return _httpContextAccessor.HttpContext?.User.Claims.ToArray() ?? EmptyClaimArray;
    }
}