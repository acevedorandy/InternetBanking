

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
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Application.Models.beneficiario;
using InternetBanking.Application.Dtos.dbo.pagos.beneficiario;
using InternetBanking.Persistance.Context;
using InternetBanking.Application.Enum.pagos;


namespace InternetBanking.Application.Services.dbo
{
    public class BeneficiariosService : IBeneficiariosService
    {
        private readonly IBeneficiariosRepository _beneficiariosRepository;
        private readonly ICuentasAhorroRepository _cuentasAhorroRepository;
        private readonly InternetBankingContext _internetBankingContext;
        private readonly ILogger<BeneficiariosService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse authenticationResponse;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly ICuentasAhorroService _cuentasAhorroService;

        public BeneficiariosService(IBeneficiariosRepository beneficiariosRepository, ILogger<BeneficiariosService> logger,
                                 IMapper mapper, IHttpContextAccessor httpContextAccessor, ICuentasAhorroRepository cuentasAhorroRepository,
                                 IUsuariosRepository usuariosRepository, InternetBankingContext internetBankingContext, ICuentasAhorroService cuentasAhorroService)
        {
            _beneficiariosRepository = beneficiariosRepository;
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            authenticationResponse = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("usuario");
            _cuentasAhorroRepository = cuentasAhorroRepository;
            _usuariosRepository = usuariosRepository;
            _internetBankingContext = internetBankingContext;
            _cuentasAhorroService = cuentasAhorroService;
        }

        public async Task<bool> ExistRelationAsync(BeneficiariosDto dto)
        {
            var result = await _beneficiariosRepository.ExistRelation(dto.CuentaBeneficiario);
            return result;
        }

        public async Task<ServiceResponse> GetAllAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _beneficiariosRepository.GetAll();

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
                response.Messages = "Ha ocurrido un error obteniendo los beneficiarios.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetBeneficieresAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                string userId = authenticationResponse.Id;
                var result = await _beneficiariosRepository.GetBeneficiaries(userId);

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
                response.Messages = "Ha ocurrido un error obteniendo los beneficiarios.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }


        public async Task<ServiceResponse> GetByIDAsync(int id)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _beneficiariosRepository.GetById(id);

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
                response.Messages = "Ha ocurrido un error obteniendo el beneficiario.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> LoadPagoBeneficiario(int cuentaId, int cuentaBeneficiarioId, string usuarioId)
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
                var cuentaUsuario = await _cuentasAhorroRepository.GetById(cuentaId);
                if (!cuentaUsuario.Success)
                    return SetError($"La cuenta no fue encontrada.");

                var cuentaUsuarioData = cuentaUsuario.Data as CuentasAhorroModel;

                var cuentaBeneficiario = await _cuentasAhorroService.GetUserByAccountIDAsync(cuentaBeneficiarioId);
                if (!cuentaBeneficiario.IsSuccess)
                    return SetError($"La cuenta no fue encontrada.");

                var cuentaBeneficiarioData = cuentaBeneficiario.Model as CuentasAhorroModel;

                PagoBeneficiarioModel pagoBeneficiario = new PagoBeneficiarioModel
                {
                    CuentaID = cuentaUsuarioData.CuentaID,
                    NumeroCuenta = cuentaUsuarioData.NumeroCuenta,
                    Saldo = cuentaUsuarioData.Saldo,
                    BeneficiarioUsuarioID = cuentaBeneficiarioData.UsuarioID,
                    Nombre = cuentaBeneficiarioData.Nombre,
                    Apellido = cuentaBeneficiarioData.Apellido,
                    CuentaBeneficiarioID = cuentaBeneficiarioData.CuentaID,
                    CuentaBeneficiario = cuentaBeneficiarioData.NumeroCuenta,
                };

                response.Model = pagoBeneficiario;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un procesando la solicitud.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> PagoBeneficiario(PagoBeneficiarioDto dto)
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
                    _internetBankingContext.Update(cuentas);

                    var cuentaBeneficiario = await _internetBankingContext.CuentasAhorros.FindAsync(dto.CuentaBeneficiarioID);
                    if (cuentas == null)
                        return SetError("La cuenta no existe.");

                    cuentaBeneficiario.Saldo += dto.Monto;
                    _internetBankingContext.Update(cuentaBeneficiario);

                    dto.UsuarioID = authenticationResponse.Id;
                    dto.Tipo = Pagos.Pago.ToString();
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

        public async Task<ServiceResponse> RemoveAsync(BeneficiariosDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                Beneficiarios beneficiarios = new Beneficiarios();

                beneficiarios.BeneficiarioID = dto.BeneficiarioID;
                var result = await _beneficiariosRepository.Remove(beneficiarios);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error elimninando el beneficiario.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> SaveAsync(BeneficiariosDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                string numCuenta = dto.CuentaBeneficiario;
                var check = await _cuentasAhorroRepository.GetAccountByNum(numCuenta);

                if (check.Success && check.Data is CuentasAhorroModel cuenta)
                {
                    dto.UsuarioID = authenticationResponse.Id;
                    var beneficiario = _mapper.Map<Beneficiarios>(dto);

                    beneficiario.CuentaBeneficiario = cuenta.NumeroCuenta;
                    beneficiario.BeneficiarioUsuarioID = cuenta.UsuarioID;

                    await _beneficiariosRepository.Save(beneficiario);
                }
                else 
                {
                    response.Messages = check.Message;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error agregando el beneficiario.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> UpdateAsync(BeneficiariosDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var resultGetBy = await _beneficiariosRepository.GetById(dto.BeneficiarioID);

                if (!resultGetBy.Success)
                {
                    response.IsSuccess = resultGetBy.Success;
                    response.Messages = resultGetBy.Message;

                    return response;
                }
                var beneficiario = _mapper.Map<Beneficiarios>(dto);
                var result = await _beneficiariosRepository.Update(beneficiario);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error actualizando el amigo.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }
    }
}
