using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atomic.Identity;

public record PasswordLoginDto
{
    [Required]
    [DisplayName("User name or email")]
    public string? UserNameOrEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(16, MinimumLength = 6)]
    [DisplayName("Password")]
    public string? Password { get; set; }
}

public record ExternalLoginDto
{
    [Required]
    public string? LoginProvider { get; set; }

    [Required]
    public string? ProviderKey { get; set; }
}