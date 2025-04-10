

namespace InternetBanking.Application.Base
{
    public interface IBaseService<TResponse, TEntityDto>
    {
        Task<TResponse> SaveAsync(TEntityDto dto);
        Task<TResponse> UpdateAsync(TEntityDto dto);
        Task<TResponse> RemoveAsync(TEntityDto dto);
        Task<TResponse> GetAllAsync();
        Task<TResponse> GetByIDAsync(int id);
    }
}
