using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Atomic.Identity;

public record ChangePasswordDto
{
    [Required]
    public string? Id { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [DisplayName("Current password")]
    public string? CurrentPassword { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [DisplayName("New password")]
    public string? NewPassword { get; set; }
}

public enum ContactMethod
{
    [Display(Name = "Phone")]
    Phone,
    [Display(Name = "Email")]
    Email,
}

public record RequireResetPasswordDto
{
    [DisplayName("Contact method")]
    public ContactMethod Method { get; set; } = ContactMethod.Email;

    [Required]
    [DisplayName("Id, user name or email")]
    public string? UserIdentifier { get; set; }
}

public record ResetPasswordDto
{
    [Required]
    [HiddenInput]
    public string? UserId { get; set; }

    [Required]
    [HiddenInput]
    public string? Token { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [DisplayName("Password")]
    public string? Password { get; set; }
}