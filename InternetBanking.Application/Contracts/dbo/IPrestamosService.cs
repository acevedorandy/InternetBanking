

using InternetBanking.Application.Base;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.prestamo;

namespace InternetBanking.Application.Contracts.dbo
{
    public interface IPrestamosService : IBaseService<ServiceResponse, PrestamosDto>
    {
        Task<ServiceResponse> LoadPagoPrestamoView(int cuentaId, int prestamoId);
        Task<ServiceResponse> PagoPrestamo(PagoPrestamoDto dto);
        Task<ServiceResponse> GetPrestamoAsProductAsync(string userId);
    }
}
