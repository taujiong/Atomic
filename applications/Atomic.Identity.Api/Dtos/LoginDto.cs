using System.ComponentModel.DataAnnotations;

namespace Atomic.Identity.Api.Dtos;

public record PasswordLoginDto
{
    [Required]
    public string? UserNameOrEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(16, MinimumLength = 6)]
    public string? Password { get; set; }
}

public record ExternalLoginDto
{
    [Required]
    public string? LoginProvider { get; set; }

    [Required]
    public string? ProviderKey { get; set; }
}