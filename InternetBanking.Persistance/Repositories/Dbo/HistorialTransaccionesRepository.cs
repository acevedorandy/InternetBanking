

using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Domain.Result;
using InternetBanking.Persistance.Base;
using InternetBanking.Persistance.Context;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Persistance.Models.dbo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InternetBanking.Persistance.Repositories.Dbo
{
    public sealed class HistorialTransaccionesRepository(InternetBankingContext internetBankingContext,
                                                ILogger<HistorialTransaccionesRepository> logger) : BaseRepository<HistorialTransacciones>(internetBankingContext), IHistorialTransaccionesRepository
    {
        private readonly InternetBankingContext _internetBankingContext = internetBankingContext;
        private readonly ILogger<HistorialTransaccionesRepository> _logger = logger;

        public async override Task<OperationResult> Save(HistorialTransacciones historialTransacciones)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Save(historialTransacciones);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error guardando el historial de transaccion.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Update(HistorialTransacciones historialTransacciones)
        {
            OperationResult result = new OperationResult();

            try
            {
                HistorialTransacciones? historialTransaccionesToUpdate = await _internetBankingContext.HistorialTransacciones.FindAsync(historialTransacciones.TransaccionID);

                historialTransaccionesToUpdate.HistorialID = historialTransacciones.HistorialID;
                historialTransaccionesToUpdate.TransaccionID = historialTransacciones.TransaccionID;
                historialTransaccionesToUpdate.Estado = historialTransacciones.Estado;
                historialTransaccionesToUpdate.Fecha = historialTransacciones.Fecha;

                result = await base.Update(historialTransaccionesToUpdate);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error actualizando el historial de transaccion.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Remove(HistorialTransacciones historialTransacciones)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Remove(historialTransacciones);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error eliminando el historial de transaccion.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();

            try
            {
                result.Data = await (from dbo in _internetBankingContext.HistorialTransacciones
                               join dbo1 in _internetBankingContext.Transacciones on dbo.TransaccionID equals dbo1.TransaccionID
                               
                               select new HistorialTransaccionesModel
                               {
                                   HistorialID = dbo.HistorialID,
                                   TransaccionID = dbo1.TransaccionID,
                                   Estado = dbo.Estado,
                                   Fecha = dbo.Fecha

                               }).AsNoTracking()
                               .ToListAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo el historial de transacciones.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> GetById(int id)
        {
            OperationResult result = new OperationResult();

            try
            {
                result.Data = await (from dbo in _internetBankingContext.HistorialTransacciones
                               join dbo1 in _internetBankingContext.Transacciones on dbo.TransaccionID equals dbo1.TransaccionID

                               where dbo.HistorialID == id

                               select new HistorialTransaccionesModel
                               {
                                   HistorialID = dbo.HistorialID,
                                   TransaccionID = dbo1.TransaccionID,
                                   Estado = dbo.Estado,
                                   Fecha = dbo.Fecha

                               }).AsNoTracking()
                               .ToListAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo el historial de transacciones.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }
    }
}
