using AutoMapper;
using Azure;
using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Contracts.identity;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.identity;
using InternetBanking.Application.Dtos.identity.account;
using InternetBanking.Application.Models.ViewModel;
using InternetBanking.Application.Responses.identity;
using InternetBanking.Identity.Shared.Entities;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Persistance.Models.dbo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using InternetBanking.Application.Helpers.web;

namespace InternetBanking.Application.Services.dbo
{
    public class UsuariosService : IUsuarioService
    {
        private readonly IAccountService _accountService;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UsuariosService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse authentication;

        public UsuariosService(IAccountService accountService, IUsuariosRepository usuariosRepository, IMapper mapper, 
                               ILogger<UsuariosService> logger, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {
            _accountService = accountService;
            _usuariosRepository = usuariosRepository;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _httpContextAccessor = httpContext;
            authentication = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("usuario");
        }

        public async Task<ServiceResponse> ActivarOrDesactivarAsync(string userId)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                if (userId == authentication.Id)
                {
                    response.IsSuccess = false;
                    response.Messages = "No puedes activar o desactivar tu propio usuario.";
                    return response;
                }

                var result = await _usuariosRepository.ActivarOrDesactivar(userId);

                if (!result.Success)
                {
                    response.IsSuccess = result.Success;
                    response.Messages = response.Messages;
                    return response;
                }

                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error actualiando el estado del usuario.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<string> ConfirmEmailAsync(string userId, string token)
        {
            return await _accountService.ConfirmAccountAsync(userId, token);
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto, string origin)
        {
            ForgotPasswordRequest forgotPassword = _mapper.Map<ForgotPasswordRequest>(forgotPasswordDto);
            return await _accountService.ForgotPasswordAsync(forgotPassword, origin);

        }

        public async Task<ServiceResponse> GetAllAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _usuariosRepository.GetIdentityUserAll();

                if (!result.Success)
                {
                    response.IsSuccess = result.Success;
                    response.Messages = response.Messages;
                    return response;
                }

                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo los usuarios.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public Task<ServiceResponse> GetByIDAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse> GetIdentityUserByASYNC(string userId)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _usuariosRepository.GetIdentityUserBy(userId);

                if (!result.Success)
                {
                    response.IsSuccess = result.Success;
                    response.Messages = response.Messages;
                    return response;
                }

                var usuario = result.Data as UsuariosModel;

                EditUserModel userModel = new EditUserModel
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Cedula = usuario.Cedula,
                    Email = usuario.Correo,
                    UserName = usuario.UserName,
                };
                response.Model = userModel;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo el usuario.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetUserByCedulaAsync(string cedula)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _usuariosRepository.GetUserByCedula(cedula);

                if (!result.Success)
                {
                    response.IsSuccess = result.Success;
                    response.Messages = response.Messages;
                    return response;
                }

                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo el usuario.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetUserByRolAsync(string rolId)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _usuariosRepository.GetUserByRol(rolId);

                if (!result.Success)
                {
                    response.IsSuccess = result.Success;
                    response.Messages = response.Messages;
                    return response;
                }

                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo los usuarios.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginDto loginDto)
        {
            AuthenticationRequest authentication = _mapper.Map<AuthenticationRequest>(loginDto);
            AuthenticationResponse response = await _accountService.AuthenticateAsync(authentication);
            return response;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterDto registerDto, string origin)
        {
            RegisterRequest register = _mapper.Map<RegisterRequest>(registerDto);
            RegisterResponse response = await _accountService.RegisterAdminUserAsync(register, origin);
            return response;
        }

        public async Task<RegisterResponse> RegisterBasicUserAsync(RegisterBasicUserDto registerDto)
        {
            RegisterResponse response = await _accountService.RegisterBasicUserAsync(registerDto);
            return response;
        }

        public Task<ServiceResponse> RemoveAsync(UsuariosDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            ResetPasswordRequest passwordRequest = _mapper.Map<ResetPasswordRequest>(resetPasswordDto);
            return await _accountService.ResetPasswordAsync(passwordRequest);
        }

        public Task<ServiceResponse> SaveAsync(UsuariosDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task SignOutAsync()
        {
            await _accountService.SignOutAsync();
        }

        public Task<ServiceResponse> UpdateAsync(UsuariosDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse> UpdateIdentityUserAsync(UsuariosDto user, decimal? monto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var resultGetBy = await _usuariosRepository.GetIdentityUserBy(user.Id);

                if (!resultGetBy.Success)
                {
                    resultGetBy.Success = response.IsSuccess;
                    resultGetBy.Message = response.Messages;

                    return response;
                }

                var usuario = _mapper.Map<ApplicationUser>(user);

                if (!string.IsNullOrEmpty(user.Password))
                {
                    var passwordHash = _userManager.PasswordHasher.HashPassword(usuario, user.Password);
                    usuario.PasswordHash = passwordHash;
                }

                var result = await _usuariosRepository.UpdateIdentityUser(usuario, monto);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error actualizando el usuario.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }
    }
}
