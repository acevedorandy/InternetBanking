
using InternetBanking.Application.Base;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;

namespace InternetBanking.Application.Contracts.dbo
{
    public interface IHistorialTransaccionesService : IBaseService<ServiceResponse, HistorialTransaccionesDto>
    {
    }
}
