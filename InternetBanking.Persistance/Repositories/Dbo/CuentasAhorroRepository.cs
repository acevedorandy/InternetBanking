

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
    public sealed class CuentasAhorroRepository(InternetBankingContext internetBankingContext, IdentityContext identityContext,
                                                ILogger<CuentasAhorroRepository> logger) : BaseRepository<CuentasAhorro>(internetBankingContext), ICuentasAhorroRepository
    {
        private readonly InternetBankingContext _internetBankingContext = internetBankingContext;
        private readonly IdentityContext _identityContext = identityContext;
        private readonly ILogger<CuentasAhorroRepository> _logger = logger;

        public async override Task<OperationResult> Save(CuentasAhorro cuentasAhorro)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Save(cuentasAhorro);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error guardando la cuenta de ahorros.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Update(CuentasAhorro cuentasAhorro)
        {
            OperationResult result = new OperationResult();

            try
            {
                CuentasAhorro? cuentasAhorroToUpdate = await _internetBankingContext.CuentasAhorros.FindAsync(cuentasAhorro.CuentaID);

                cuentasAhorroToUpdate.CuentaID = cuentasAhorro.CuentaID;
                cuentasAhorroToUpdate.UsuarioID = cuentasAhorro.UsuarioID;
                cuentasAhorroToUpdate.NumeroCuenta = cuentasAhorro.NumeroCuenta;
                cuentasAhorroToUpdate.Saldo = cuentasAhorro.Saldo;
                cuentasAhorroToUpdate.Principal = cuentasAhorro.Principal;

                result = await base.Update(cuentasAhorroToUpdate);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error actualizando la cuenta de ahorros.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Remove(CuentasAhorro cuentasAhorro)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Remove(cuentasAhorro);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error eliminando la cuenta de ahorros.";
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

                var datos = (from dbo in cuentas
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id
                               
                               select new CuentasAhorroModel
                               {
                                   CuentaID = dbo.CuentaID,
                                   UsuarioID = dbo1.Id,
                                   NumeroCuenta = dbo.NumeroCuenta,
                                   Saldo = dbo.Saldo,
                                   Principal = dbo.Principal

                               }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error las cuentas de ahorros.";
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

                var datos = (from dbo in cuentas
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id

                               where dbo.CuentaID == id

                               select new CuentasAhorroModel
                               {
                                   CuentaID = dbo.CuentaID,
                                   UsuarioID = dbo.NumeroCuenta,
                                   NumeroCuenta = dbo.NumeroCuenta,
                                   Saldo = dbo.Saldo,
                                   Principal = dbo.Principal,
                                   Nombre = dbo1.Nombre,
                                   Apellido = dbo1.Apellido

                               }).FirstOrDefault();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error las cuentas de ahorros.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetAccountByNum(string num)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users
                    .ToListAsync();

                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();

                var datos = (from dbo in cuentas
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()

                             where dbo.NumeroCuenta == num

                             select new CuentasAhorroModel()
                             {
                                 CuentaID = dbo.CuentaID,
                                 UsuarioID = dbo1.Id,
                                 NumeroCuenta = dbo.NumeroCuenta,
                                 Saldo = dbo.Saldo,
                                 Principal = dbo.Principal,
                                 Nombre = dbo1.Nombre,
                                 Apellido = dbo1.Apellido,

                             }).FirstOrDefault();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetTarjetaAsProduct(string userId)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users
                    .ToListAsync();

                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();

                var datos = (from dbo in cuentas
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id

                             where dbo.UsuarioID == userId

                             select new CuentasAhorroModel
                             {
                                 CuentaID = dbo.CuentaID,
                                 UsuarioID = dbo.NumeroCuenta,
                                 NumeroCuenta = dbo.NumeroCuenta,
                                 Saldo = dbo.Saldo,
                                 Principal = dbo.Principal,

                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error las cuentas de ahorros.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetUserByAccountID(int id)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users
                    .ToListAsync();

                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();

                var datos = (from dbo in cuentas
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id
                             where dbo.CuentaID == id
                             select new CuentasAhorroModel
                             {
                                 CuentaID = dbo.CuentaID,
                                 UsuarioID = dbo.NumeroCuenta,
                                 NumeroCuenta = dbo.NumeroCuenta,
                                 Saldo = dbo.Saldo,
                                 Principal = dbo.Principal,
                                 Nombre = dbo1.Nombre,
                                 Apellido = dbo1.Apellido

                             }).FirstOrDefault();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error al obtener los detalles de la cuenta de ahorro.";
                _logger.LogError(result.Message, ex.ToString());
            }

            return result;
        }

    }
}
