using InternetBanking.Application.Base;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.expreso;
using InternetBanking.Domain.Result;


namespace InternetBanking.Application.Contracts.dbo
{
    public interface ITransaccionesService : IBaseService<ServiceResponse, TransaccionesDto>
    {
        Task<ServiceResponse> PagoExpresoAsync(ConfirmarPagoExpresoDto dto);
        Task<ServiceResponse> LoadExpresoView(string cuentaDestino, int cuentaOrigen);
        Task<ServiceResponse> GetAllByDateAsync(DateTime fecha);
        Task<ServiceResponse> GetAllPagoAsync();
        Task<ServiceResponse> GetAllPagoByDateAsync(DateTime fecha);
        Task<ServiceResponse> GettTransactionByAccountAsync(int cuentaId);


    }
}
