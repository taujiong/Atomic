using System.ComponentModel.DataAnnotations;
using Atomic.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Atomic.AppService.Dtos;

[Serializable]
public record QueryRequestDto : IValidatableObject
{
    public static int DefaultMaxResultCount { get; set; } = 10;

    public static int MaxMaxResultCount { get; set; } = 1000;

    [Range(1, int.MaxValue)]
    public int MaxResultCount { get; set; } = DefaultMaxResultCount;

    [Range(0, int.MaxValue)]
    public int SkipCount { get; set; } = 0;

    public string? Sort { get; set; }

    public string? Filter { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxResultCount > MaxMaxResultCount)
        {
            var localizer = validationContext.GetRequiredService<IStringLocalizer<AtomicSharedResource>>();

            yield return new ValidationResult(
                localizer[
                    "{0} can not be more than {1}! Increase {2}.{3} on the server side to allow more results.",
                    nameof(MaxResultCount),
                    MaxMaxResultCount,
                    typeof(QueryRequestDto).FullName!,
                    nameof(MaxMaxResultCount)
                ],
                new[] { nameof(MaxResultCount) });
        }
    }
}