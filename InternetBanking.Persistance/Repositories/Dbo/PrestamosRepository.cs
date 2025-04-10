
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Domain.Result;
using InternetBanking.Identity.Shared.Context;
using InternetBanking.Persistance.Base;
using InternetBanking.Persistance.Context;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Persistance.Models.dbo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InternetBanking.Persistance.Repositories.Dbo
{
    public sealed class PrestamosRepository(InternetBankingContext internetBankingContext, IdentityContext identityContext,
                                                ILogger<PrestamosRepository> logger) : BaseRepository<Prestamos>(internetBankingContext), IPrestamosRepository
    {
        private readonly InternetBankingContext _internetBankingContext = internetBankingContext;
        private readonly IdentityContext _identityContext = identityContext;
        private readonly ILogger<PrestamosRepository> _logger = logger;

        public async override Task<OperationResult> Save(Prestamos prestamos)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Save(prestamos);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error guardando el prestamo.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Update(Prestamos prestamos)
        {
            OperationResult result = new OperationResult();

            try
            {
                Prestamos? prestamosToUpdate = await _internetBankingContext.Prestamos.FindAsync(prestamos.PrestamoID);

                prestamosToUpdate.PrestamoID = prestamos.PrestamoID;
                prestamosToUpdate.UsuarioID = prestamos.UsuarioID;
                prestamosToUpdate.Monto = prestamos.Monto;
                prestamosToUpdate.SaldoDeuda = prestamos.SaldoDeuda;
                prestamosToUpdate.Pagado = prestamos.Pagado;

                result = await base.Update(prestamosToUpdate);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error actualizando el prestamo.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Remove(Prestamos prestamos)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Remove(prestamos);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error eliminando el prestamo";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users
                    .ToListAsync();

                var prestamos = await _internetBankingContext.Prestamos
                    .ToListAsync();

                var datos = (from dbo in prestamos
                                 join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()

                                 select new PrestamosModel
                                 {
                                     PrestamoID = dbo.PrestamoID,
                                     UsuarioID = dbo1.Id,
                                     NumeroPrestamo = dbo.NumeroPrestamo,
                                     Monto = dbo.Monto,
                                     SaldoDeuda = dbo.SaldoDeuda,
                                     Pagado = dbo.Pagado,

                                 }).ToList();

               result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los prestamos";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> GetById(int id)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users
                    .ToListAsync();

                var prestamos = await _internetBankingContext.Prestamos
                    .ToListAsync();

                var datos = (from dbo in prestamos
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()

                             where dbo.PrestamoID == id

                             select new PrestamosModel
                             {
                                 PrestamoID = dbo.PrestamoID,
                                 UsuarioID = dbo1.Id,
                                 NumeroPrestamo = dbo.NumeroPrestamo,
                                 Monto = dbo.Monto,
                                 SaldoDeuda = dbo.SaldoDeuda,
                                 Pagado = dbo.Pagado,

                             }).FirstOrDefault();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los prestamos";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetPrestamoAsProduct(string userId)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users
                    .ToListAsync();

                var prestamos = await _internetBankingContext.Prestamos
                    .ToListAsync();

                var datos = (from dbo in prestamos
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()

                             where dbo.UsuarioID == userId

                             select new PrestamosModel
                             {
                                 PrestamoID = dbo.PrestamoID,
                                 UsuarioID = dbo1.Id,
                                 NumeroPrestamo = dbo.NumeroPrestamo,
                                 Monto = dbo.Monto,
                                 SaldoDeuda = dbo.SaldoDeuda,
                                 Pagado = dbo.Pagado,

                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los prestamos";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }
    }
}
