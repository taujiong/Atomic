using System.ComponentModel.DataAnnotations;

namespace Atomic.Identity.Api.Dtos;

public record IdentityUserUpdateDto
{
    [Required]
    public string? UserName { get; set; }

    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
}

public record IdentityUserCreateDto : IdentityUserUpdateDto
{
    [Required]
    [StringLength(16, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}