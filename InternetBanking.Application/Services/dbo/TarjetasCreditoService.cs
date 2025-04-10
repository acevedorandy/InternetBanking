
using AutoMapper;
using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Responses.identity;
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Application.Helpers.web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using InternetBanking.Application.Helpers;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Application.Models.tarjetas;
using InternetBanking.Persistance.Context;
using InternetBanking.Application.Dtos.dbo.avance;
using InternetBanking.Application.Enum.pagos;
using InternetBanking.Application.Dtos.dbo.pagos.tarjeta;
using Microsoft.EntityFrameworkCore;


namespace InternetBanking.Application.Services.dbo
{
    public class TarjetasCreditoService : ITarjetasCreditoService
    {
        private readonly ITarjetasCreditoRepository _tarjetasCreditoRepository;
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private readonly ICuentasAhorroRepository _cuentasAhorroRepository;
        private readonly ITransaccionesRepository _transaccionesRepository;
        private readonly ILogger<TarjetasCreditoService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse authentication;
        private readonly InternetBankingContext _internetBankingContext;

        public TarjetasCreditoService(ITarjetasCreditoRepository tarjetasCreditoRepository, ILogger<TarjetasCreditoService> logger,
                                      IMapper mapper, IHttpContextAccessor httpContextAccessor, ICuentasAhorroService cuentasAhorroService,
                                      InternetBankingContext internetBankingContext, ICuentasAhorroRepository cuentasAhorroRepository,
                                      ITransaccionesRepository transaccionesRepository)
        {
            _tarjetasCreditoRepository = tarjetasCreditoRepository;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            authentication = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("usuario");
            _cuentasAhorroService = cuentasAhorroService;
            _internetBankingContext = internetBankingContext;
            _cuentasAhorroRepository = cuentasAhorroRepository;
            _transaccionesRepository = transaccionesRepository;
        }

        public async Task<ServiceResponse> AvanceEfectivo(AvanceEfectivoDto dto)
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
                    var tarjeta = await _internetBankingContext.TarjetasCreditos.FindAsync(dto.TarjetaID);
                    if (tarjeta == null || tarjeta.SaldoDisponible < dto.Monto)
                        return SetError("Fondos insuficientes.");

                    decimal interes = 6.25M / 100;

                    decimal montoConInteres = dto.Monto * (1 + interes);

                    if (tarjeta.SaldoDisponible < montoConInteres)
                        return SetError("Fondos insuficientes.");
                   
                    tarjeta.SaldoDisponible -= montoConInteres;

                    tarjeta.SaldoDeuda += montoConInteres;

                    _internetBankingContext.TarjetasCreditos.Update(tarjeta);

                    var cuenta = await _internetBankingContext.CuentasAhorros.FindAsync(dto.CuentaID);
                    if (cuenta == null)
                        return SetError("Cuenta no encontrada.");

                    cuenta.Saldo += dto.Monto;
                    _internetBankingContext.CuentasAhorros.Update(cuenta); 

                    await _internetBankingContext.SaveChangesAsync();

                    dto.UsuarioID = authentication.Id;
                    dto.Tipo = Pagos.AvanceDeEfectivo.ToString().Replace("De", " de ");
                    var transaccion = _mapper.Map<Transacciones>(dto);
                    await _internetBankingContext.Transacciones.AddAsync(transaccion);

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

        public async Task<ServiceResponse> GetAllAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _tarjetasCreditoRepository.GetAll();

                var tarjetas = result.Data as List<TarjetasCreditoModel>;

                response.Model = tarjetas
                    .Where(t => t.UsuarioID == authentication.Id)
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
                response.Messages = "Ha ocurrido un error obteniendo las tarjetas de credito.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetByIDAsync(int id)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _tarjetasCreditoRepository.GetById(id);

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
                response.Messages = "Ha ocurrido un error obteniendo la tarjeta de credito.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetTarjetaAsProductAsync(string userId)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _tarjetasCreditoRepository.GetTarjetaAsProduct(userId);

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
                response.Messages = "Ha ocurrido un error obteniendo la tarjeta de credito.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> LoadAvanceView(int cuentaId, int tarjetaId)
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
                var cuenta = await _cuentasAhorroService.GetByIDAsync(cuentaId);

                if(!cuenta.IsSuccess)
                    return SetError($"La cuenta no fue encontrada.");

                var cuentaData = cuenta.Model as CuentasAhorroModel;

                var tarjeta = await GetByIDAsync(tarjetaId);

                if(!tarjeta.IsSuccess)
                    return SetError($"La tarjeta no fue encontrada.");

                var tarjetaData = tarjeta.Model as TarjetasCreditoModel;

                AvanceEfectivoModel avanceEfectivoView = new AvanceEfectivoModel
                {
                    CuentaID = cuentaData.CuentaID,
                    UsuarioID = cuentaData.UsuarioID,
                    NumeroCuenta = cuentaData.NumeroCuenta,
                    TarjetaID = tarjetaData.TarjetaID,
                    NumeroTarjeta = tarjetaData.NumeroTarjeta,
                    SaldoDisponible = tarjetaData.SaldoDisponible,
                };
                response.Model = avanceEfectivoView;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un procesando la solicitud.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> LoadPagoView(int cuentaId, int tarjetaId)
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
                var cuenta = await _cuentasAhorroService.GetByIDAsync(cuentaId);

                if (!cuenta.IsSuccess)
                    return SetError($"La cuenta no fue encontrada.");

                var cuentaData = cuenta.Model as CuentasAhorroModel;

                var tarjeta = await _tarjetasCreditoRepository.GetById(tarjetaId);
                if (!tarjeta.Success)
                    return SetError($"La tarjeta no fue encontrada.");

                var tarjetaData = tarjeta.Data as TarjetasCreditoModel;

                PagoTarjetaModel pagoTarjeta = new PagoTarjetaModel
                {
                    TarjetaID = tarjetaData.TarjetaID,
                    NumeroTarjeta = tarjetaData.NumeroTarjeta,
                    SaldoDeuda = tarjetaData.SaldoDeuda,
                    CuentaID = tarjetaData.TarjetaID,
                    NumeroCuenta = cuentaData.NumeroCuenta,
                    SaldoDisponible = cuentaData.Saldo,
                };
                response.Model = pagoTarjeta;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un procesando la solicitud.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> PagoTarjeta(PagoTarjetaDto dto)
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
                    var cuenta = await _internetBankingContext.CuentasAhorros.FindAsync(dto.CuentaID);

                    if (cuenta == null || cuenta.Saldo < dto.Monto)
                        return SetError("Fondos insuficientes.");

                    cuenta.Saldo -= dto.Monto;
                    _internetBankingContext.CuentasAhorros.Update(cuenta);

                    var tarjeta = await _internetBankingContext.TarjetasCreditos.FindAsync(dto.TarjetaID);
                    if (tarjeta == null)
                        return SetError("La cuenta de tarjeta no fue encontrada.");

                    decimal saldoDeudaRestante = tarjeta.SaldoDeuda - dto.Monto;

                    if (saldoDeudaRestante < 0)
                    {
                        decimal exceso = Math.Abs(saldoDeudaRestante);

                        cuenta.Saldo += exceso;
                        _internetBankingContext.CuentasAhorros.Update(cuenta);

                        tarjeta.SaldoDeuda = 0;
                    }
                    else
                    {
                        tarjeta.SaldoDeuda = saldoDeudaRestante;
                    }

                    tarjeta.SaldoDisponible += dto.Monto;
                    _internetBankingContext.TarjetasCreditos.Update(tarjeta);
                    await _internetBankingContext.SaveChangesAsync();

                    dto.UsuarioID = authentication.Id;
                    dto.Tipo = Pagos.Pago.ToString();
                    var transaccion = _mapper.Map<Transacciones>(dto);
                    await _internetBankingContext.Transacciones.AddAsync(transaccion);

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

        public async Task<ServiceResponse> RemoveAsync(TarjetasCreditoDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                TarjetasCredito tarjetasCredito = new TarjetasCredito();

                tarjetasCredito.TarjetaID = dto.TarjetaID;
                var result = await _tarjetasCreditoRepository.Remove(tarjetasCredito);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error eliminando la tarjeta de credito.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> SaveAsync(TarjetasCreditoDto dto)
        {
            var response = new ServiceResponse();

            try
            {
                var principalCheck = await _internetBankingContext.CuentasAhorros.ToListAsync();

                dto.NumeroTarjeta = GeneradosNumeros.GenerarTarjeta();
                dto.SaldoDisponible = dto.LimiteCredito;
                dto.SaldoDeuda = 0;

                var tarjeta = _mapper.Map<TarjetasCredito>(dto);
                await _tarjetasCreditoRepository.Save(tarjeta);

                dto.TarjetaID = tarjeta.TarjetaID;
                response.Model = dto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error guardando la tarjeta de crédito.";
                _logger.LogError(response.Messages, ex);
            }

            return response;
        }

        public async Task<ServiceResponse> UpdateAsync(TarjetasCreditoDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var resultGetBy = await _tarjetasCreditoRepository.GetById(dto.TarjetaID);

                if (!resultGetBy.Success)
                {
                    resultGetBy.Success = response.IsSuccess;
                    resultGetBy.Message = response.Messages;

                    return response;
                }

                var tarjeta = _mapper.Map<TarjetasCredito>(dto);
                var result = await _tarjetasCreditoRepository.Update(tarjeta);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error actualizando la tarjeta de credito.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }
    }
}