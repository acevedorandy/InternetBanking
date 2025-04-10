

using AutoMapper;
using InternetBanking.Application.Contracts.identity;
using InternetBanking.Application.Dtos.identity;
using InternetBanking.Application.Dtos.identity.account;
using InternetBanking.Application.Enum.identity;
using InternetBanking.Application.Responses.identity;
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Identity.Helpers;
using InternetBanking.Identity.Shared.Entities;
using InternetBanking.Infraestructure.Interfaces;
using InternetBanking.Persistance.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace InternetBanking.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly EmailHelper _emailHelper;
        private readonly InternetBankingContext _internetBankingContext;
        private readonly IMapper _mapper;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                               IEmailService emailService, EmailHelper emailHelper, InternetBankingContext internetBankingContext,
                               IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _emailHelper = emailHelper;
            _internetBankingContext = internetBankingContext;
            _mapper = mapper;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            AuthenticationResponse response = new AuthenticationResponse();

            AuthenticationResponse SetError(string errorMessage)
            {
                response.HasError = true;
                response.Error = errorMessage;
                return response;
            }

            var usuario = await _userManager.FindByEmailAsync(request.Email);
            if (usuario == null)
                return SetError($"No hay cuentas registradas con el correo {request.Email}");

            var result = await _signInManager.PasswordSignInAsync(usuario.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
                return SetError($"Credenciales inválidas para {request.Email}");

            if (!usuario.EmailConfirmed)
                return SetError($"Se necesita la activación del correo: {request.Email} para iniciar sesión.");

            response.Id = usuario.Id;
            response.Email = usuario.Email;
            response.UserName = usuario.UserName;
            response.Roles = (await _userManager.GetRolesAsync(usuario)).ToList();
            response.IsVerified = usuario.EmailConfirmed;

            return response;
        }


        public async Task<string> ConfirmAccountAsync(string userId, string token)
        {
            async Task<string> SetError(string message) => message;

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return await SetError("Ninguna cuenta registrada con este usuario.");

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(usuario, token);

            return await SetError(result.Succeeded
                ? $"La cuenta con el correo {usuario.Email} ha sido confirmada."
                : $"Ha ocurrido un error registrando el correo {usuario.Email}.");
        }


        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            ForgotPasswordResponse response = new ForgotPasswordResponse();

            var usuario = await _userManager.FindByEmailAsync(request.Email);

            if (usuario == null)
            {
                response.HasError = true;
                response.Error = $"Ninguna cuenta registrada con el correo {request.Email}.";
                return response;
            }

            var verificationURL = await _emailHelper.ForgotPasswordURL(usuario, origin);

            await _emailService.SendEmailAsync(new Infraestructure.Dto.EmailRequest()
            {
                To = usuario.Email,
                Body = $"Por favor, reinicia tu cuenta visitando esta URL {verificationURL}",
                Subject = "Restablecimiento de contraseña"
            });

            return response;
        }

        public async Task<RegisterResponse> RegisterAdminUserAsync(RegisterRequest request, string origin)
        {
            RegisterResponse response = new();

            RegisterResponse SetError(string errorMessage)
            {
                response.HasError = true;
                response.Error = errorMessage;
                return response;
            }

            if (await _userManager.FindByNameAsync(request.UserName) != null)
                return SetError($"El nombre de usuario {request.UserName} ya existe, por favor elija otro.");

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return SetError($"El correo {request.Email} ya existe.");

            if (await _userManager.FindByNameAsync(request.Cedula) != null)
                return SetError($"Ya existe un usuario con la cedula {request.Cedula}.");

            var usuario = new ApplicationUser
            {
                UserName = request.UserName,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Cedula = request.Cedula,
                Email = request.Email,
                PhoneNumber = request.Phone,
                IsActive = request.IsActive,
            };

            var result = await _userManager.CreateAsync(usuario, request.Password);

            if (!result.Succeeded)
                return SetError($"Ha ocurrido un error intentando registrar el usuario.");

            await _userManager.AddToRoleAsync(usuario, Roles.Admin.ToString());

            var verificacionURL = await _emailHelper.VerificationEmailURL(usuario, origin);
            await _emailService.SendEmailAsync(new Infraestructure.Dto.EmailRequest
            {
                To = usuario.Email,
                Body = $"Por favor, confirme su cuenta ingresando a esta URL: {verificacionURL}",
                Subject = "Registro de confirmación"
            });

            return response;
        }

        public async Task<RegisterResponse> RegisterBasicUserAsync(RegisterBasicUserDto userDto)
        {
            RegisterResponse response = new();

            RegisterResponse SetError(string errorMessage)
            {
                response.HasError = true;
                response.Error = errorMessage;
                return response;
            }

            using (var transanction = await _internetBankingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await _userManager.FindByNameAsync(userDto.UserName) != null)
                        return SetError($"El nombre de usuario {userDto.UserName} ya existe, por favor elija otro.");

                    if (await _userManager.FindByEmailAsync(userDto.Email) != null)
                        return SetError($"El correo {userDto.Email} ya existe.");

                    if (await _userManager.FindByNameAsync(userDto.Cedula) != null)
                        return SetError($"Ya existe un usuario con la cedula {userDto.Cedula}.");

                    var usuario = new ApplicationUser
                    {
                        UserName = userDto.UserName,
                        Nombre = userDto.Nombre,
                        Apellido = userDto.Apellido,
                        Cedula = userDto.Cedula,
                        Email = userDto.Email,
                        PhoneNumber = userDto.Phone,
                        EmailConfirmed = true,
                        IsActive = userDto.IsActive,
                    };

                    var result = await _userManager.CreateAsync(usuario, userDto.Password);

                    if (!result.Succeeded)
                        return SetError($"Ha ocurrido un error intentando registrar el usuario.");

                    await _userManager.AddToRoleAsync(usuario, Roles.Basic.ToString());

                    userDto.NumeroCuenta = AccountGenerator.GenerateAccount();

                    if (userDto.Saldo < 1)
                    {
                        userDto.Saldo = 0;
                    }

                    var cuenta = new CuentasAhorro
                    {
                        UsuarioID = usuario.Id,
                        NumeroCuenta = userDto.NumeroCuenta,
                        Saldo = userDto.Saldo,
                        Principal = true
                    };

                    await _internetBankingContext.CuentasAhorros.AddAsync(cuenta);
                    await _internetBankingContext.SaveChangesAsync();

                    await transanction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transanction.RollbackAsync();
                    response.HasError = false;
                    response.Error = "Ha ocurrido un error con la transacción.";
                    return response;
                }
                return response;
            }
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ResetPasswordResponse response = new();

            ResetPasswordResponse SetError(string errorMessage)
            {
                response.HasError = true;
                response.Error = errorMessage;
                return response;
            }

            if (string.IsNullOrEmpty(request.Email) ||
                string.IsNullOrEmpty(request.Password) ||
                string.IsNullOrEmpty(request.ConfirmPassword))
            {
                return SetError("Por favor llenar todos los campos.");
            }

            var usuario = await _userManager.FindByEmailAsync(request.Email);
            if (usuario == null)
                return SetError($"No hay cuentas registradas con el correo: {request.Email}");

            request.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            var result = await _userManager.ResetPasswordAsync(usuario, request.Token, request.Password);
            if (!result.Succeeded)
                return SetError("Ha ocurrido un error al restablecer la contraseña.");

            return response;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
