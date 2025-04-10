

using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Domain.Repositories;
using InternetBanking.Domain.Result;

namespace InternetBanking.Persistance.Interfaces.dbo
{
    public interface ITransaccionesRepository : IBaseRepository<Transacciones>
    {
        Task<OperationResult> GetAllByDate(DateTime date);
        Task<OperationResult> GetAllPago();
        Task<OperationResult> GetAllPagoByDate(DateTime date);
        Task<OperationResult> GettTransactionByAccount(int cuentaId);
    }
}
