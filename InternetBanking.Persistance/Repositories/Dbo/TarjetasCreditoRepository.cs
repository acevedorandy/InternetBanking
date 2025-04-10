

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
    public sealed class TarjetasCreditoRepository(InternetBankingContext internetBankingContext, IdentityContext identityContext,
                                                ILogger<TarjetasCreditoRepository> logger) : BaseRepository<TarjetasCredito>(internetBankingContext), ITarjetasCreditoRepository
    {
        private readonly InternetBankingContext _internetBankingContext = internetBankingContext;
        private readonly IdentityContext _identityContext = identityContext;
        private readonly ILogger<TarjetasCreditoRepository> _logger = logger;

        public async override Task<OperationResult> Save(TarjetasCredito tarjetasCredito)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Save(tarjetasCredito);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error guardando la tarjeta de credito";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Update(TarjetasCredito tarjetasCredito)
        {
            OperationResult result = new OperationResult();

            try
            {
                TarjetasCredito? tarjetasCreditoToUpdate = await _internetBankingContext.TarjetasCreditos.FindAsync(tarjetasCredito.TarjetaID);

                tarjetasCreditoToUpdate.TarjetaID = tarjetasCredito.TarjetaID;
                tarjetasCreditoToUpdate.UsuarioID = tarjetasCredito.UsuarioID;
                tarjetasCreditoToUpdate.NumeroTarjeta = tarjetasCredito.NumeroTarjeta;
                tarjetasCreditoToUpdate.LimiteCredito = tarjetasCredito.LimiteCredito;
                tarjetasCreditoToUpdate.SaldoDeuda = tarjetasCredito.SaldoDeuda;
                tarjetasCreditoToUpdate.SaldoDisponible = tarjetasCredito.SaldoDisponible;
                tarjetasCreditoToUpdate.TipoTarjeta = tarjetasCredito.TipoTarjeta;
                tarjetasCreditoToUpdate.Icono = tarjetasCredito.Icono;

                result = await base.Update(tarjetasCreditoToUpdate);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error guardando la tarjeta de credito";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async override Task<OperationResult> Remove(TarjetasCredito tarjetasCredito)
        {
            OperationResult result = new OperationResult();

            try
            {
                result = await base.Remove(tarjetasCredito);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error eliminando la tarjeta";
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

                var tarjetas = await _internetBankingContext.TarjetasCreditos
                    .ToArrayAsync();

                var datos = (from dbo in tarjetas
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()

                           select new TarjetasCreditoModel
                           {
                               TarjetaID = dbo.TarjetaID,
                               UsuarioID = dbo1.Id,
                               NumeroTarjeta = dbo.NumeroTarjeta,
                               LimiteCredito = dbo.LimiteCredito,
                               SaldoDeuda = dbo.SaldoDeuda,
                               SaldoDisponible = dbo.SaldoDisponible,
                               TipoTarjeta = dbo.TipoTarjeta,
                               Icono = dbo.Icono

                           }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo las tarjetas de credito";
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

                var tarjetas = await _internetBankingContext.TarjetasCreditos
                    .ToArrayAsync();

                var datos = (from dbo in tarjetas
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()

                                where dbo.TarjetaID == id

                                select new TarjetasCreditoModel
                                {
                                    TarjetaID = dbo.TarjetaID,
                                    UsuarioID = dbo1.Id,
                                    NumeroTarjeta = dbo.NumeroTarjeta,
                                    LimiteCredito = dbo.LimiteCredito,
                                    SaldoDeuda = dbo.SaldoDeuda,
                                    SaldoDisponible = dbo.SaldoDisponible,
                                    TipoTarjeta = dbo.TipoTarjeta,
                                    Icono = dbo.Icono

                                }).FirstOrDefault();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo latarjeta de credito";
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

                var tarjetas = await _internetBankingContext.TarjetasCreditos
                    .ToArrayAsync();

                var datos = (from dbo in tarjetas
                             join dbo1 in usuarios on dbo.UsuarioID.ToString() equals dbo1.Id.ToString()

                             where dbo.UsuarioID == userId

                             select new TarjetasCreditoModel
                             {
                                 TarjetaID = dbo.TarjetaID,
                                 UsuarioID = dbo1.Id,
                                 NumeroTarjeta = dbo.NumeroTarjeta,
                                 LimiteCredito = dbo.LimiteCredito,
                                 SaldoDeuda = dbo.SaldoDeuda,
                                 SaldoDisponible = dbo.SaldoDisponible,
                                 TipoTarjeta = dbo.TipoTarjeta,
                                 Icono = dbo.Icono

                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo las tarjetas de credito";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

    }
}
