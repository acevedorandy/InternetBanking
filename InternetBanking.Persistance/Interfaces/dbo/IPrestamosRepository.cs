

using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Domain.Repositories;
using InternetBanking.Domain.Result;

namespace InternetBanking.Persistance.Interfaces.dbo
{
    public interface IPrestamosRepository : IBaseRepository<Prestamos>
    {
        Task<OperationResult> GetPrestamoAsProduct(string userId);
    }
}
