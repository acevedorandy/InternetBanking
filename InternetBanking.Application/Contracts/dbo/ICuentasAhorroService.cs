

using InternetBanking.Application.Base;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.cuenta;


namespace InternetBanking.Application.Contracts.dbo
{
    public interface ICuentasAhorroService : IBaseService<ServiceResponse, CuentasAhorroDto>
    {
        Task<ServiceResponse> GetAccountByNumAsync(string cuenta);
        Task<ServiceResponse> LoadPagoCuenta(int cuentaOrigenId, int cuentaDestinoId);
        Task<ServiceResponse> PagoCuenta(PagoCuentaDto dto);
        Task<ServiceResponse> GetTarjetaAsProductAsync(string userId);
        Task<ServiceResponse> DeleteAccountAsync(int cuentaId);
        Task<ServiceResponse> GetUserByAccountIDAsync(int id);

    }
}
