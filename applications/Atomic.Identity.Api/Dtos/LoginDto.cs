using System.ComponentModel.DataAnnotations;

namespace Atomic.UnifiedAuth.Api.Dtos;

public record LoginDto
{
    [Required]
    public string? UserNameOrEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(16, MinimumLength = 6)]
    public string? Password { get; set; }

    public bool PersistLogin { get; set; }
}