namespace Atomic.AppService.Services;

public interface IUpdateAppService<in TKey, TEntityDto>
    : IUpdateAppService<TKey, TEntityDto, TEntityDto>
{
}

public interface IUpdateAppService<in TKey, TGetOutputDto, in TUpdateInput>
{
    Task<TGetOutputDto> UpdateById(TKey id, TUpdateInput input);
}