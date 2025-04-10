

using AutoMapper;
using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Responses.identity;
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Persistance.Interfaces.dbo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using InternetBanking.Application.Helpers.web;
using InternetBanking.Application.Dtos.dbo.pagos.expreso;
using InternetBanking.Persistance.Context;
using InternetBanking.Application.Enum.pagos;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Persistance.Models.ViewModels.pagos;
using InternetBanking.Application.Enum.tarjeta;

namespace InternetBanking.Application.Services.dbo
{
    public class TransaccionesService : ITransaccionesService
    {
        private readonly ITransaccionesRepository _transaccionesRepository;
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private InternetBankingContext _InternetBankingContext;
        private readonly ILogger<TransaccionesService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse _authentication;

        public TransaccionesService(ITransaccionesRepository transaccionesRepository, ILogger<TransaccionesService> logger,
                                    IMapper mapper, IHttpContextAccessor httpContext, InternetBankingContext context, ICuentasAhorroService cuentasAhorroService)
        {
            _transaccionesRepository = transaccionesRepository;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContext;
            _authentication = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("usuario");
            _InternetBankingContext = context;
            _cuentasAhorroService = cuentasAhorroService;
        }
        public async Task<ServiceResponse> GetAllAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _transaccionesRepository.GetAll();

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
                response.Messages = "Ha ocurrido un error obteniendo las transacciones.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetByIDAsync(int id)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _transaccionesRepository.GetById(id);

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
                response.Messages = "Ha ocurrido un error obteniendo la transaccion.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> PagoExpresoAsync(ConfirmarPagoExpresoDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            ServiceResponse SetError(string errorMessage)
            {
                response.IsSuccess = false;
                response.Messages = errorMessage;
                return response;
            }

            using (var transaction = await _InternetBankingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var cuentaOrigen = await _InternetBankingContext.CuentasAhorros.FindAsync(dto.CuentaOrigenID);
                   
                    if (cuentaOrigen == null || cuentaOrigen.Saldo < dto.Monto)
                        return SetError("Fondos insuficientes.");

                    cuentaOrigen.Saldo -= dto.Monto;
                    _InternetBankingContext.CuentasAhorros.Update(cuentaOrigen);

                    var cuentaDestino = await _InternetBankingContext.CuentasAhorros.FindAsync(dto.CuentaID);
                    if (cuentaDestino == null)
                        return SetError("Cuenta no encontrada o no existe.");

                    cuentaDestino.Saldo += dto.Monto;
                    _InternetBankingContext.CuentasAhorros.Update(cuentaDestino);

                    dto.UsuarioID = _authentication.Id;
                    dto.Tipo = Pagos.Pago.ToString();
                    var transaccion = _mapper.Map<Transacciones>(dto);

                    await _InternetBankingContext.Transacciones.AddAsync(transaccion);
                    await _InternetBankingContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Messages = "Ha ocurrido un error con la transaccion.";
                    _logger.LogError(response.Messages, ex.ToString());
                }
            }
            return response;
        }

        public async Task<ServiceResponse> RemoveAsync(TransaccionesDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                Transacciones transacciones = new Transacciones();

                transacciones.TransaccionID = dto.TransaccionID;
                var result = await _transaccionesRepository.Remove(transacciones);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error eliminando la transaccion.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> SaveAsync(TransaccionesDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var transaccion = _mapper.Map<Transacciones>(dto);
                var result = await _transaccionesRepository.Save(transaccion);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error guardando la transaccion.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> UpdateAsync(TransaccionesDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var resultGetBy = await _transaccionesRepository.GetById(dto.TransaccionID);

                if (!resultGetBy.Success)
                {
                    resultGetBy.Success = response.IsSuccess;
                    resultGetBy.Message = response.Messages;

                    return response;
                }

                var transaccion = _mapper.Map<Transacciones>(dto);
                var result = await _transaccionesRepository.Update(transaccion);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error actualizando la transaccion.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> LoadExpresoView(string cuentaDestino, int cuentaOrigen)
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
                var cuenta = await _cuentasAhorroService.GetAccountByNumAsync(cuentaDestino);

                if (!cuenta.IsSuccess)
                    return SetError($"Ha ocurrido un error.");

                var cuentaDestinoData = cuenta.Model as CuentasAhorroModel;

                if (cuentaDestinoData == null)
                    return SetError($"La cuenta destino no existe.");

                var origen = await _cuentasAhorroService.GetByIDAsync(cuentaOrigen);

                if (!origen.IsSuccess)
                    return SetError($"No existe esta cuenta en el usuario logueado.");

                var cuentaOrigenData = origen.Model as CuentasAhorroModel;

                if (cuentaOrigenData == null)
                    return SetError($"La cuenta origen no existe");


                ExpresoViewModel expresoView = new ExpresoViewModel
                {
                    CuentaOrigenID = cuentaOrigenData.CuentaID,
                    CuentaID = cuentaDestinoData.CuentaID,
                    NumeroCuentaOrigen = cuentaOrigenData.NumeroCuenta,
                    NumeroCuenta = cuentaDestinoData.NumeroCuenta,
                    Nombre = cuentaDestinoData.Nombre,
                    Apellido = cuentaDestinoData.Apellido,
                };
                response.Model = expresoView;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un procesando la solicitud.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetAllByDateAsync(DateTime fecha)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _transaccionesRepository.GetAllByDate(fecha);

                var transacciones = result.Data as List<TransaccionesModel>;

                response.Model = transacciones
                    .Where(t => t.Tipo == Pagos.Transferencia.ToString())
                    .ToList();

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
                response.Messages = "Ha ocurrido un error obteniendo las transacciones.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetAllPagoAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _transaccionesRepository.GetAllPago();

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
                response.Messages = "Ha ocurrido un error obteniendo los pagos.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetAllPagoByDateAsync(DateTime fecha)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _transaccionesRepository.GetAllByDate(fecha);

                var transacciones = result.Data as List<TransaccionesModel>;

                response.Model = transacciones
                    .Where(t => t.Tipo == Pagos.Pago.ToString())
                    .ToList();

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
                response.Messages = "Ha ocurrido un error obteniendo los pagos.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GettTransactionByAccountAsync(int cuentaId)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _transaccionesRepository.GettTransactionByAccount(cuentaId);

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
                response.Messages = "Ha ocurrido un error obteniendo la transaccion.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }
    }
}
