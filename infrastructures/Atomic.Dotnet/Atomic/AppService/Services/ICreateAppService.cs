namespace Atomic.AppService.Services;

public interface ICreateAppService<TEntityDto>
    : ICreateAppService<TEntityDto, TEntityDto>
{
}

public interface ICreateAppService<TGetOutputDto, in TCreateInput>
{
    Task<TGetOutputDto> Create(TCreateInput input);
}