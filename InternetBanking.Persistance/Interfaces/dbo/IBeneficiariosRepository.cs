

using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Domain.Repositories;
using InternetBanking.Domain.Result;

namespace InternetBanking.Persistance.Interfaces.dbo
{
    public interface IBeneficiariosRepository : IBaseRepository<Beneficiarios>
    {
        Task<OperationResult> GetBeneficiaries(string userId);
        Task<bool> ExistRelation(string cuentaBeneficiario);
    }
}
