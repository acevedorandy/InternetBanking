

using InternetBanking.Domain.Result;
using InternetBanking.Identity.Shared.Entities;

namespace InternetBanking.Persistance.Interfaces.dbo
{
    public interface IUsuariosRepository
    {
        Task<OperationResult> GetIdentityUserAll();
        Task<OperationResult> GetIdentityUserBy(string userId);
        Task<OperationResult> GetUserByRol(string rol);
        Task<OperationResult> GetUserByCedula(string cedula);
        Task<OperationResult> UpdateIdentityUser(ApplicationUser user, decimal? monto);
        Task<OperationResult> ActivarOrDesactivar(string userId);
    }
}
