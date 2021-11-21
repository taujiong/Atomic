using System.ComponentModel.DataAnnotations;

namespace Atomic.Identity.Api.Dtos;

public record ChangePasswordDto
{
    [Required]
    public string? Id { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }
}

public enum ContactMethod
{
    Phone,
    Email,
}

public record RequireResetPasswordDto
{
    public ContactMethod Method { get; set; } = ContactMethod.Email;

    [Required]
    public string? UserIdentifier { get; set; }
}

public record ResetPasswordDto
{
    [Required]
    public string? UserId { get; set; }

    [Required]
    public string? Token { get; set; }

    [Required]
    public string? Password { get; set; }
}