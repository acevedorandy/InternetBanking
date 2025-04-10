
using AutoMapper;
using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Responses.identity;
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Persistance.Models.dbo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using InternetBanking.Application.Helpers.web;
using InternetBanking.Application.Models.prestamos;
using InternetBanking.Application.Helpers;
using InternetBanking.Persistance.Context;
using InternetBanking.Application.Dtos.dbo.pagos.prestamo;
using InternetBanking.Application.Enum.pagos;
using Microsoft.EntityFrameworkCore;

namespace InternetBanking.Application.Services.dbo
{
    public class PrestamosService : IPrestamosService
    {
        private readonly IPrestamosRepository _prestamosRepository;
        private readonly ILogger<PrestamosService> _logger;
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private readonly InternetBankingContext _internetBankingContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse authentication;

        public PrestamosService(IPrestamosRepository prestamosRepository, ILogger<PrestamosService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor
                                ,ICuentasAhorroService cuentasAhorroService, InternetBankingContext internetBankingContext)
        {
            _prestamosRepository = prestamosRepository;
            _cuentasAhorroService = cuentasAhorroService;
            _logger = logger;
            _internetBankingContext = internetBankingContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            authentication = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("usuario");
        }
        public async Task<ServiceResponse> GetAllAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _prestamosRepository.GetAll();

                var prestamos = result.Data as List<PrestamosModel>;

                response.Model = prestamos
                    .Where(p => p.UsuarioID == authentication.Id)
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
                response.Messages = "Ha ocurrido un error obteniendo los prestamos.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetByIDAsync(int id)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _prestamosRepository.GetById(id);

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
                response.Messages = "Ha ocurrido un error obteniendo el prestamos.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetPrestamoAsProductAsync(string userId)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _prestamosRepository.GetPrestamoAsProduct(userId);

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
                response.Messages = "Ha ocurrido un error obteniendo el prestamos.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> LoadPagoPrestamoView(int cuentaId, int prestamoId)
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

                var prestamo = await GetByIDAsync(prestamoId);
                
                if (!prestamo.IsSuccess)
                    return SetError($"El prestamo no fue encontradol.");

                var prestamoData = prestamo.Model as PrestamosModel;

                PagoPrestamoModel pagoPrestamo = new PagoPrestamoModel
                {
                    CuentaID = cuentaData.CuentaID,
                    NumeroCuenta = cuentaData.NumeroCuenta,
                    Saldo = cuentaData.Saldo,
                    PrestamoID = prestamoData.PrestamoID,
                    NumeroPrestamo = prestamoData.NumeroPrestamo,
                    SaldoDeuda = prestamoData.SaldoDeuda,
                };
                response.Model = pagoPrestamo;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un procesando la solicitud.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> PagoPrestamo(PagoPrestamoDto dto)
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
                    var cuentas = await _internetBankingContext.CuentasAhorros.FindAsync(dto.CuentaID);
                    if (cuentas == null || cuentas.Saldo < dto.Monto)
                        return SetError("Fondos insuficientes.");

                    cuentas.Saldo -= dto.Monto;
                    _internetBankingContext.CuentasAhorros.Update(cuentas);

                    var prestamos = await _internetBankingContext.Prestamos.FindAsync(dto.PrestamoID);
                    if (prestamos == null)
                        return SetError("Prestamo no encontrado.");

                    decimal saldoDeudaRestante = prestamos.SaldoDeuda - dto.Monto;

                    if (saldoDeudaRestante <= 0)
                    {
                        decimal exceso = Math.Abs(saldoDeudaRestante);

                        cuentas.Saldo += exceso;
                        _internetBankingContext.CuentasAhorros.Update(cuentas);

                        prestamos.SaldoDeuda = 0;
                        prestamos.Pagado = true;
                    }
                    else
                    {
                        prestamos.SaldoDeuda = saldoDeudaRestante;
                    }

                    _internetBankingContext.Prestamos.Update(prestamos);
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

        public async Task<ServiceResponse> RemoveAsync(PrestamosDto dto)
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
                var prestamo = await _internetBankingContext.Prestamos.FindAsync(dto.PrestamoID);

                if (prestamo == null)
                    return SetError("El préstamo no existe.");

                if (prestamo.SaldoDeuda > 0)
                    return SetError("El préstamo aún no está saldado.");

                _internetBankingContext.Prestamos.Remove(prestamo);

                await _internetBankingContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error eliminando el prestamos.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> SaveAsync(PrestamosDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            ServiceResponse SetError(string errorMessage)
            {
                response.IsSuccess = false;
                response.Messages = errorMessage;
                return response;
            }
            using (var transacction = await _internetBankingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    dto.NumeroPrestamo = GeneradosNumeros.GenerateNineDigite();
                    dto.SaldoDeuda = dto.Monto;
                    var prestamos = _mapper.Map<Prestamos>(dto);

                    await _internetBankingContext.AddAsync(prestamos);

                    var cuenta = await _internetBankingContext.CuentasAhorros
                        .FirstOrDefaultAsync(c => c.UsuarioID == dto.UsuarioID || c.Principal == true);

                    if (cuenta != null)
                    {
                        // Lógica si se encuentra la cuenta
                        // Ejemplo: hacer algo con la cuenta
                        cuenta.Saldo += prestamos.Monto;
                        _internetBankingContext.CuentasAhorros.Update(cuenta);
                    }
                    else
                    {
                        return SetError("El usuario no posee una cuenta al cual depositar el prestamo. Por favor crear una antes.");
                    }

                    await _internetBankingContext.SaveChangesAsync();
                    await transacction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transacction.RollbackAsync();
                    response.IsSuccess = false;
                    response.Messages = "Ha ocurrido un error guardando el prestamos.";
                    _logger.LogError(response.Messages, ex.ToString());
                }
                return response;
            }

        }

        public async Task<ServiceResponse> UpdateAsync(PrestamosDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var resultGetBy = await _prestamosRepository.GetById(dto.PrestamoID);

                if (!resultGetBy.Success)
                {
                    resultGetBy.Success = response.IsSuccess;
                    resultGetBy.Message = response.Messages;

                    return response;
                }

                var prestamos = _mapper.Map<Prestamos>(dto);
                var result = await _prestamosRepository.Update(prestamos);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error guardando el prestamos.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }
    }
}
