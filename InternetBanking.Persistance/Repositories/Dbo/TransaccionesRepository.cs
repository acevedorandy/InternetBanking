

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
    public sealed class TransaccionesRepository(InternetBankingContext internetBankingContext, IdentityContext identityContext,
                                                ILogger<TransaccionesRepository> logger) : BaseRepository<Transacciones>(internetBankingContext), 
                                                ITransaccionesRepository
    {
        private readonly InternetBankingContext _internetBankingContext = internetBankingContext;
        private readonly IdentityContext _identityContext = identityContext;
        private readonly ILogger<TransaccionesRepository> _logger = logger;

        public async override Task<OperationResult> Save(Transacciones transacciones)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Save(transacciones);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error guardando la transaccion.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Update(Transacciones transacciones)
        {
            OperationResult result = new OperationResult();

            try
            {
                Transacciones? transaccionesToUpdate = await _internetBankingContext.Transacciones.FindAsync(transacciones.TransaccionID);

                transaccionesToUpdate.TransaccionID = transacciones.TransaccionID;
                transaccionesToUpdate.UsuarioID = transacciones.UsuarioID;
                transaccionesToUpdate.CuentaID = transacciones.CuentaID;
                transaccionesToUpdate.Tipo = transacciones.Tipo;
                transaccionesToUpdate.Monto = transacciones.Monto;
                transaccionesToUpdate.Fecha = transacciones.Fecha;
                transaccionesToUpdate.Detalles = transacciones.Detalles;

                result = await base.Update(transaccionesToUpdate);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error actualizando el transaccion.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Remove(Transacciones transacciones)
        {
            OperationResult result = new OperationResult();

            try 
            {
                result = await base.Remove(transacciones);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error eliminando la transaccion";
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
                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();
                var transaccion = await _internetBankingContext.Transacciones
                    .ToListAsync();

                var datos = (from dbo in transaccion
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id
                             join dbo2 in cuentas on dbo.CuentaID equals dbo2.CuentaID

                             where dbo.Tipo == "Transferencia"

                             select new TransaccionesModel
                             {
                                 TransaccionID = dbo.TransaccionID,
                                 UsuarioID = dbo1.Id,
                                 CuentaID = dbo2.CuentaID,
                                 Tipo = dbo.Tipo,
                                 Monto = dbo.Monto,
                                 Fecha = dbo.Fecha,
                                 Detalles = dbo.Detalles ?? string.Empty 
                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo las transacciones.";
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
                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();
                var transaccion = await _internetBankingContext.Transacciones
                    .ToListAsync();


                var datos = (from dbo in transaccion
                                     join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id
                                     join dbo2 in cuentas on dbo.CuentaID equals dbo2.CuentaID

                                     where dbo.TransaccionID == id

                                     select new TransaccionesModel
                                     {
                                         TransaccionID = dbo.TransaccionID,
                                         UsuarioID = dbo1.Id,
                                         CuentaID = dbo2.CuentaID,
                                         Tipo = dbo.Tipo,
                                         Monto = dbo.Monto,
                                         Fecha = dbo.Fecha,
                                         Detalles = dbo.Detalles ?? string.Empty

                                     }).FirstOrDefault();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo la transaccion.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetAllByDate(DateTime date)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users.ToListAsync();
                var cuentas = await _internetBankingContext.CuentasAhorros.ToListAsync();
                var transaccion = await _internetBankingContext.Transacciones.ToListAsync();

                var datos = (from dbo in transaccion
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id
                             join dbo2 in cuentas on dbo.CuentaID equals dbo2.CuentaID

                             where dbo.Fecha.Date == date.Date 

                             select new TransaccionesModel
                             {
                                 TransaccionID = dbo.TransaccionID,
                                 UsuarioID = dbo1.Id,
                                 CuentaID = dbo2.CuentaID,
                                 Tipo = dbo.Tipo,
                                 Monto = dbo.Monto,
                                 Fecha = dbo.Fecha,
                                 Detalles = dbo.Detalles != null ? dbo.Detalles : string.Empty

                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo las transacciones para la fecha indicada.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetAllPago()
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users
                    .ToListAsync();
                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();
                var transaccion = await _internetBankingContext.Transacciones
                    .ToListAsync();

                var datos = (from dbo in transaccion
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id
                             join dbo2 in cuentas on dbo.CuentaID equals dbo2.CuentaID

                             where dbo.Tipo == "Pago"

                             select new TransaccionesModel
                             {
                                 TransaccionID = dbo.TransaccionID,
                                 UsuarioID = dbo1.Id,
                                 CuentaID = dbo2.CuentaID,
                                 Tipo = dbo.Tipo,
                                 Monto = dbo.Monto,
                                 Fecha = dbo.Fecha,
                                 Detalles = dbo.Detalles ?? string.Empty

                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo las transacciones.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public Task<OperationResult> GetAllPagoByDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> GettTransactionByAccount(int cuentaId)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users
                    .ToListAsync();
                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();
                var transaccion = await _internetBankingContext.Transacciones
                    .ToListAsync();

                var datos = (from dbo in transaccion
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id
                             join dbo2 in cuentas on dbo.CuentaID equals dbo2.CuentaID

                             where dbo2.CuentaID == cuentaId

                             select new TransaccionesModel
                             {
                                 TransaccionID = dbo.TransaccionID,
                                 UsuarioID = dbo1.Id,
                                 CuentaID = dbo2.CuentaID,
                                 NumeroCuenta = dbo2.NumeroCuenta,
                                 Tipo = dbo.Tipo,
                                 Monto = dbo.Monto,
                                 Fecha = dbo.Fecha,
                                 Detalles = dbo.Detalles ?? string.Empty

                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo las transacciones.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        //public async Task<OperationResult> PagoExpreso(Transacciones transacciones)
        //{
        //    OperationResult result = new OperationResult();

        //    try
        //    {
        //        var cuenta = await _internetBankingContext.CuentasAhorros
        //            .FirstOrDefaultAsync(c => c.CuentaID == transacciones.CuentaID);

        //        cuenta.Saldo += transacciones.Monto;

        //        _internetBankingContext.CuentasAhorros.Update(cuenta);
        //        await _internetBankingContext.SaveChangesAsync();

        //        result = await base.Save(transacciones);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = "Ocurrió un error al realizar la transferencia.";
        //        _logger.LogError(result.Message, ex.ToString());
        //    }
        //    return result;
        //}

        //public async Task<OperationResult> LessBalance(int cuentaId, decimal monto, string usuarioId)
        //{
        //    OperationResult result = new OperationResult();

        //    try
        //    {
        //        var cuenta = await _internetBankingContext.CuentasAhorros
        //            .FirstOrDefaultAsync(c => c.CuentaID == cuentaId);

        //        if (cuenta == null)
        //        {
        //            result.Success = false;
        //            result.Message = "La cuenta no existe.";
        //            return result;
        //        }

        //        // Verificar si la cuenta pertenece al usuario
        //        if (cuenta.UsuarioID != usuarioId)
        //        {
        //            result.Success = false;
        //            result.Message = "La cuenta no pertenece al usuario.";
        //            return result;
        //        }

        //        // Validar si el saldo es suficiente
        //        if (cuenta.Saldo < monto)
        //        {
        //            result.Success = false;
        //            result.Message = "Saldo insuficiente.";
        //            return result;
        //        }

        //        // Restar el saldo
        //        cuenta.Saldo -= monto;

        //        // Guardar cambios en la base de datos
        //        _internetBankingContext.CuentasAhorros.Update(cuenta);
        //        await _internetBankingContext.SaveChangesAsync();

        //        result.Success = true;
        //        result.Message = "Saldo actualizado correctamente.";
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = "Ocurrió un error al realizar la transferencia.";
        //        _logger.LogError(ex, "Error en LessBalance para la cuenta {CuentaId}", cuentaId);
        //    }

        //    return result;
        //}



        //public async Task<OperationResult> GetAccountByNum(string numCuenta)
        //{
        //    OperationResult result = new OperationResult();

        //    try
        //    {
        //        var usuarios = await _identityContext.Users
        //            .ToArrayAsync();

        //        var cuentas = await _internetBankingContext.CuentasAhorros
        //            .ToListAsync();

        //        var datos = (from dbo in cuentas
        //                     join dbo1 in usuarios on dbo.UsuarioID equals dbo1.Id

        //                     where dbo.NumeroCuenta == numCuenta

        //                     select new ExpresoViewModel()
        //                     {
        //                      CuentaID = dbo.CuentaID,
        //                      NumeroCuenta = dbo.NumeroCuenta,
        //                      Nombre = dbo1.Nombre,
        //                      Apellido = dbo1.Apellido 

        //                     }).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Success = false;
        //        result.Message = "Ha ocurrido un error verificando la cuenta.";
        //        _logger.LogError(result.Message, ex.ToString());
        //    }
        //    return result;
        //}
    }
}
