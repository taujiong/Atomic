namespace AppService.Dtos;

[Serializable]
public record PagedResultDto<T> : ListResultDto<T>
{
    public PagedResultDto(long totalCount, IReadOnlyList<T> items)
        : base(items)
    {
        TotalCount = totalCount;
    }

    public long TotalCount { get; set; }
}