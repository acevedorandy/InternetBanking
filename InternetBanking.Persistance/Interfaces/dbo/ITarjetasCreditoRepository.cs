

using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Domain.Repositories;
using InternetBanking.Domain.Result;

namespace InternetBanking.Persistance.Interfaces.dbo
{
    public interface ITarjetasCreditoRepository : IBaseRepository<TarjetasCredito>
    {
        Task<OperationResult> GetTarjetaAsProduct(string userId);
    }
}
