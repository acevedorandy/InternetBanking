

using InternetBanking.Domain.Result;
using System.Linq.Expressions;

namespace InternetBanking.Domain.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<OperationResult> Save(TEntity entity);
        Task<OperationResult> Update(TEntity entity);
        Task<OperationResult> Remove(TEntity entity);
        Task<OperationResult> GetAll();
        Task<OperationResult> GetById(int id);
        Task<bool> Exists(Expression<Func<TEntity, bool>> filter);

    }
}
