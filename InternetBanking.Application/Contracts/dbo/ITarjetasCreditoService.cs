

using InternetBanking.Application.Base;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.avance;
using InternetBanking.Application.Dtos.dbo.pagos.tarjeta;

namespace InternetBanking.Application.Contracts.dbo
{
    public interface ITarjetasCreditoService : IBaseService<ServiceResponse, TarjetasCreditoDto>
    {
        Task<ServiceResponse> LoadAvanceView(int cuentaId, int tarjetaId);
        Task<ServiceResponse> AvanceEfectivo(AvanceEfectivoDto dto);
        Task<ServiceResponse> LoadPagoView(int cuentaId, int tarjetaId);
        Task<ServiceResponse> PagoTarjeta(PagoTarjetaDto dto);
        Task<ServiceResponse> GetTarjetaAsProductAsync(string userId);
    }
}
