

using AutoMapper;
using Azure;
using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.cuenta;
using InternetBanking.Application.Enum.pagos;
using InternetBanking.Application.Helpers;
using InternetBanking.Application.Helpers.web;
using InternetBanking.Application.Models.beneficiario;
using InternetBanking.Application.Models.cuenta;
using InternetBanking.Application.Responses.identity;
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Persistance.Context;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Persistance.Models.dbo;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InternetBanking.Application.Services.dbo
{
    public class CuentasAhorroService : ICuentasAhorroService
    {
        private readonly ICuentasAhorroRepository _cuentasAhorroRepository;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly InternetBankingContext _internetBankingContext;
        private readonly ILogger<CuentasAhorroService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse authentication;

        public CuentasAhorroService(ICuentasAhorroRepository cuentasAhorroRepository, ILogger<CuentasAhorroService> logger,
                                    IMapper mapper, IHttpContextAccessor httpContextAccessor, InternetBankingContext internetBankingContext)
        {
            _cuentasAhorroRepository = cuentasAhorroRepository;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            authentication = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("usuario");
            _internetBankingContext = internetBankingContext;
        }

        public async Task<ServiceResponse> DeleteAccountAsync(int cuentaId)
        {
            ServiceResponse response = new ServiceResponse();

            ServiceResponse SetError(string errorMessage)
            {
                response.IsSuccess = false;
                response.Messages = errorMessage;
                return response;
            }

            try
            {
                var cuenta = await _internetBankingContext.CuentasAhorros.FindAsync(cuentaId);

                if (cuenta == null)
                    return SetError("La cuenta no existe.");
                

                if (cuenta.Principal == true)
                    return SetError("La cuenta principal no puede ser eliminada.");

                var cuentaPrincipal = await _internetBankingContext.CuentasAhorros
                    .FirstOrDefaultAsync(c => c.Principal == true);

                if (cuenta.Saldo > 0)
                {
                    cuentaPrincipal.Saldo += cuenta.Saldo;
                    _internetBankingContext.Update(cuentaPrincipal);
                }

                var transacciones = _internetBankingContext.Transacciones
                    .Where(t => t.CuentaID == cuentaId)
                    .ToList();

                _internetBankingContext.Transacciones.RemoveRange(transacciones);

                _internetBankingContext.CuentasAhorros.Remove(cuenta);

                await _internetBankingContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo eliminando la cuenta de ahorro.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }


        public async Task<ServiceResponse> GetAccountByNumAsync(string cuenta)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var getDestino = await _cuentasAhorroRepository.GetAccountByNum(cuenta);

                if (!getDestino.Success)
                {
                    getDestino.Success = response.IsSuccess;
                    getDestino.Message = response.Messages;
                    return response;
                }
                response.Model = getDestino.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo las cuentas de ahorro.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }


        public async Task<ServiceResponse> GetAllAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _cuentasAhorroRepository.GetAll();

                var cuentas = result.Data as List<CuentasAhorroModel>;

                response.Model = cuentas
                    .Where(c => c.UsuarioID == authentication.Id)
                    .ToList();

                if (!result.Success)
                {
                    result.Success = response.IsSuccess;
                    result.Message = response.Messages;

                    return response;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo las cuentas de ahorro.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetByIDAsync(int id)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _cuentasAhorroRepository.GetById(id);

                if (!result.Success)
                {
                    result.Success = response.IsSuccess;
                    result.Message = response.Messages;

                    return response;
                }
                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo la cuenta de ahorro.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetTarjetaAsProductAsync(string userId)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                var result = await _cuentasAhorroRepository.GetTarjetaAsProduct(userId);

                if (!result.Success)
                {
                    result.Success = response.IsSuccess;
                    result.Message = response.Messages;

                    return response;
                }

                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo las cuentas de ahorro.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetUserByAccountIDAsync(int id)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _cuentasAhorroRepository.GetUserByAccountID(id);

                if (!result.Success)
                {
                    result.Success = response.IsSuccess;
                    result.Message = response.Messages;

                    return response;
                }

                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un procesando la solicitud.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> LoadPagoCuenta(int cuentaOrigenId, int cuentaDestinoId)
        {
            ServiceResponse response = new ServiceResponse();

            ServiceResponse SetError(string errorMessage)
            {
                response.IsSuccess = false;
                response.Messages = errorMessage;
                return response;
            }

            try
            {
                var cuentaOrigen = await _cuentasAhorroRepository.GetById(cuentaOrigenId);
                if (!cuentaOrigen.Success)
                    return SetError($"La cuenta no fue encontrada.");

                var cuentaOrigenData = cuentaOrigen.Data as CuentasAhorroModel;

                var cuentaDestino = await _cuentasAhorroRepository.GetById(cuentaDestinoId);
                if (!cuentaDestino.Success)
                    return SetError($"La cuenta no fue encontrada.");

                var cuentaDestinoData = cuentaDestino.Data as CuentasAhorroModel;

                CuentaTransferenciaModel cuentaTransferencia = new CuentaTransferenciaModel
                {
                    CuentaID = cuentaOrigenData.CuentaID,
                    CuentaOrigenID = cuentaOrigenData.CuentaID,
                    NumeroCuentaOrigen = cuentaOrigenData.NumeroCuenta,
                    Saldo = cuentaOrigenData.Saldo,
                    CuentaDestinoID = cuentaDestinoData.CuentaID,
                    NumeroCuentaDestino = cuentaDestinoData.NumeroCuenta,
                };
                response.Model = cuentaTransferencia;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un procesando la solicitud.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> PagoCuenta(PagoCuentaDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            ServiceResponse SetError(string errorMessage)
            {
                response.IsSuccess = false;
                response.Messages = errorMessage;
                return response;
            }

            using (var transaction = await _internetBankingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var cuentaOrigen = await _internetBankingContext.CuentasAhorros.FindAsync(dto.CuentaOrigenID);
                    if (cuentaOrigen == null || cuentaOrigen.Saldo < dto.Monto)
                        return SetError("Fondos insuficientes.");

                    cuentaOrigen.Saldo -= dto.Monto;
                    _internetBankingContext.Update(cuentaOrigen);

                    var cuentaDestino = await _internetBankingContext.CuentasAhorros.FindAsync(dto.CuentaDestinoID);
                    if(cuentaDestino == null)
                        return SetError("La cuenta no existe.");

                    cuentaDestino.Saldo += dto.Monto;
                    _internetBankingContext.Update(cuentaDestino);

                    dto.UsuarioID = authentication.Id;
                    dto.Tipo = Pagos.Transferencia.ToString();
                    var transacion = _mapper.Map<Transacciones>(dto);
                    await _internetBankingContext.AddAsync(transacion);

                    await _internetBankingContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Messages = "Ha ocurrido un error con la transacción.";
                    _logger.LogError(response.Messages, ex.ToString());
                }
                return response;
            }
        }

        public async Task<ServiceResponse> RemoveAsync(CuentasAhorroDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                CuentasAhorro cuentasAhorro = new CuentasAhorro();

                cuentasAhorro.CuentaID = dto.CuentaID;
                var result = await _cuentasAhorroRepository.Remove(cuentasAhorro);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error eliminando la cuenta de ahorro.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> SaveAsync(CuentasAhorroDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                dto.NumeroCuenta = GeneradosNumeros.GenerateNineDigite();
                var cuentas =  _mapper.Map<CuentasAhorro>(dto);
                var result = await _cuentasAhorroRepository.Save(cuentas);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error guardando la cuenta de ahorro.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> UpdateAsync(CuentasAhorroDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var resultGetBy = await _cuentasAhorroRepository.GetById(dto.CuentaID);

                if (!resultGetBy.Success)
                {
                    response.IsSuccess = resultGetBy.Success;
                    response.Messages = resultGetBy.Message;

                    return response;
                }

                var cuentas = _mapper.Map<CuentasAhorro>(dto);
                var result = await _cuentasAhorroRepository.Update(cuentas);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error actualizando la cuenta de ahorro.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }
    }
}
