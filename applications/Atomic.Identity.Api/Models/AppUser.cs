using Microsoft.AspNetCore.Identity;

namespace Atomic.Identity.Api.Models;

public class AppUser : IdentityUser
{
    public AppUser(string userName) : base(userName)
    {
    }
}