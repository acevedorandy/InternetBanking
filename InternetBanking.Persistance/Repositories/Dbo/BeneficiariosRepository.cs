
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Domain.Result;
using InternetBanking.Identity.Shared.Context;
using InternetBanking.Persistance.Base;
using InternetBanking.Persistance.Context;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Persistance.Models.ViewModels.beneficiario;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InternetBanking.Persistance.Repositories.Dbo
{
    public sealed class BeneficiariosRepository(InternetBankingContext internetBankingContext, IdentityContext identityContext,
                                                ILogger<BeneficiariosRepository> logger) : BaseRepository<Beneficiarios>(internetBankingContext), IBeneficiariosRepository
    {
        private readonly InternetBankingContext _internetBankingContext = internetBankingContext;
        private readonly IdentityContext _identityContext = identityContext;
        private readonly ILogger<BeneficiariosRepository> _logger = logger;

        public async override Task<OperationResult> Save(Beneficiarios beneficiarios)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Save(beneficiarios);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error guardando el beneficiario.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Update(Beneficiarios beneficiarios)
        {
            OperationResult result = new OperationResult();

            try
            {
                Beneficiarios? beneficiariosToUpdate = await _internetBankingContext.Beneficiarios.FindAsync(beneficiarios.BeneficiarioID);

                beneficiariosToUpdate.BeneficiarioID = beneficiarios.BeneficiarioID;
                beneficiariosToUpdate.UsuarioID = beneficiarios.UsuarioID;
                beneficiariosToUpdate.BeneficiarioUsuarioID = beneficiarios.BeneficiarioUsuarioID;
                beneficiariosToUpdate.CuentaBeneficiario = beneficiarios.CuentaBeneficiario;
                beneficiariosToUpdate.Alias = beneficiarios.Alias;

                result = await base.Update(beneficiariosToUpdate);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error actualizando el beneficiario.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Remove(Beneficiarios beneficiarios)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Remove(beneficiarios);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error eliminando el beneficiario.";
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

                var beneficiarios = await _internetBankingContext.Beneficiarios
                    .ToListAsync(); 

                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();

                var datos = (from dbo in beneficiarios
                                     join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()
                                     join dbo2 in cuentas on dbo.CuentaBeneficiario equals dbo2.NumeroCuenta


                                     select new BeneficiariosModel
                                     {
                                         BeneficiarioID = dbo.BeneficiarioID,
                                         UsuarioID = dbo1.Id,
                                         BeneficiarioUsuarioID = dbo.BeneficiarioUsuarioID,
                                         CuentaBeneficiarioID = dbo2.CuentaID,
                                         CuentaBeneficiario = dbo2.NumeroCuenta,
                                         Alias = dbo.Alias

                                     }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los beneficiarios.";
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

                var beneficiarios = await _internetBankingContext.Beneficiarios
                    .ToListAsync();

                var cuentas = await _internetBankingContext.CuentasAhorros
                    .ToListAsync();

                var datos = (from dbo in beneficiarios
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()
                                     join dbo2 in cuentas on dbo.CuentaBeneficiario equals dbo2.NumeroCuenta

                                     where dbo.BeneficiarioID == id

                                     select new BeneficiariosModel
                                     {
                                         BeneficiarioID = dbo.BeneficiarioID,
                                         UsuarioID = dbo1.Id,
                                         BeneficiarioUsuarioID = dbo1.Id,
                                         CuentaBeneficiario = dbo.CuentaBeneficiario,
                                         Alias = dbo.Alias

                                     }).FirstOrDefault();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los beneficiarios.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetBeneficiaries(string userId)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users.ToListAsync();
                var beneficiarios = await _internetBankingContext.Beneficiarios.ToListAsync();
                var cuentas = await _internetBankingContext.CuentasAhorros.ToListAsync();

                var datos = (from dbo in beneficiarios
                             join usuario in usuarios on dbo.UsuarioID.ToString() equals usuario.Id.ToString() 
                             join cuenta in cuentas on dbo.CuentaBeneficiario equals cuenta.NumeroCuenta
                             join beneficiarioUsuario in usuarios on cuenta.UsuarioID.ToString() equals beneficiarioUsuario.Id.ToString() 

                             where dbo.UsuarioID == userId

                             select new BeneficiariosViewModel
                             {
                                 BeneficiarioID = dbo.BeneficiarioID,
                                 //UsuarioID = usuario.Id,  // Usuario logueado que registró
                                 //BeneficiarioUsuarioID = cuenta.UsuarioID, // ID real del beneficiario
                                 Nombre = beneficiarioUsuario.Nombre, // Nombre correcto del beneficiario
                                 Apellido = beneficiarioUsuario.Apellido,
                                 CuentaBeneficiario = cuenta.NumeroCuenta,
                                 Alias = dbo.Alias
                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los beneficiarios.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<bool> ExistRelation(string cuentaBeneficiario)
        {
            var relation = await _internetBankingContext.Beneficiarios
                .Where(b => b.CuentaBeneficiario == cuentaBeneficiario)
                .FirstOrDefaultAsync();

            return relation != null;

        }
    }
}
