namespace Atomic.AppService.Services;

public interface ICrudAppService<in TKey, TEntityDto>
    : ICrudAppService<TKey, TEntityDto, TEntityDto>
{
}

public interface ICrudAppService<in TKey, TEntityDto, in TCreateDto>
    : ICrudAppService<TKey, TEntityDto, TCreateDto, TCreateDto>
{
}

public interface ICrudAppService<in TKey, TEntityDto, in TCreateInput, in TUpdateInput>
    : ICrudAppService<TKey, TEntityDto, TEntityDto, TCreateInput, TUpdateInput>
{
}

public interface ICrudAppService<in TKey, TGetOutputDto, TGetListOutputDto, in TCreateInput,
        in TUpdateInput>
    : IReadOnlyAppService<TKey, TGetOutputDto, TGetListOutputDto>,
        ICreateAppService<TGetOutputDto, TCreateInput>,
        IUpdateAppService<TKey, TGetOutputDto, TUpdateInput>,
        IDeleteAppService<TKey>
{
}