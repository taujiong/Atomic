using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atomic.Identity;

public record IdentityUserUpdateDto
{
    [Required]
    [DisplayName("User name")]
    public string? UserName { get; set; }

    [DataType(DataType.EmailAddress)]
    [DisplayName("Email")]
    public string? Email { get; set; }

    [DataType(DataType.PhoneNumber)]
    [DisplayName("Phone number")]
    public string? PhoneNumber { get; set; }
}

public record IdentityUserCreateDto : IdentityUserUpdateDto
{
    [Required]
    [StringLength(16, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [DisplayName("Password")]
    public string? Password { get; set; }
}

public record IdentityUserOutputDto
{
    public string? Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool LockoutEnabled { get; set; }
}