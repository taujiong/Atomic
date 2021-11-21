namespace Atomic.AppService.Services;

public interface IDeleteAppService<in TKey>
{
    Task DeleteById(TKey id);
}