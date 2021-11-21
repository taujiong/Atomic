using AppService.Dtos;
using Atomic.AppService.Dtos;

namespace Atomic.AppService.Services;

public interface IReadOnlyAppService<in TKey, TEntityDto>
    : IReadOnlyAppService<TKey, TEntityDto, TEntityDto>
{
}

public interface IReadOnlyAppService<in TKey, TGetOutputDto, TGetListOutputDto>
{
    Task<TGetOutputDto> GetById(TKey id);

    Task<PagedResultDto<TGetListOutputDto>> GetListByQuery(QueryRequestDto input);
}