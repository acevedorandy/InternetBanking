

using InternetBanking.Application.Base;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.beneficiario;

namespace InternetBanking.Application.Contracts.dbo
{
    public interface IBeneficiariosService : IBaseService<ServiceResponse, BeneficiariosDto>
    {
        Task<ServiceResponse> GetBeneficieresAsync();
        Task<bool> ExistRelationAsync(BeneficiariosDto dto);
        Task<ServiceResponse> LoadPagoBeneficiario(int cuentaId, int cuentaBeneficiarioId, string usuarioId);
        Task<ServiceResponse> PagoBeneficiario(PagoBeneficiarioDto dto);

    }
}
 