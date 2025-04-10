
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Domain.Repositories;
using InternetBanking.Domain.Result;

namespace InternetBanking.Persistance.Interfaces.dbo
{
    public interface ICuentasAhorroRepository : IBaseRepository<CuentasAhorro>
    {
        Task<OperationResult> GetAccountByNum(string num);
        Task<OperationResult> GetTarjetaAsProduct(string userId);
        Task<OperationResult> GetUserByAccountID(int id);
    }
}
