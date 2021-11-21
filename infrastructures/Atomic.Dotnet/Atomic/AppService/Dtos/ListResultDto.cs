namespace AppService.Dtos;

[Serializable]
public record ListResultDto<T>
{
    public ListResultDto(IReadOnlyList<T>? items)
    {
        Items = items ?? new List<T>();
    }

    public IReadOnlyList<T> Items { get; set; }
}