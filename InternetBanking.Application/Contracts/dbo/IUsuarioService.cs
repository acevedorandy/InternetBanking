

using InternetBanking.Application.Base;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.identity.account;
using InternetBanking.Application.Responses.identity;

namespace InternetBanking.Application.Contracts.dbo
{
    public interface IUsuarioService : IBaseService<ServiceResponse, UsuariosDto>
    {
        Task<string> ConfirmEmailAsync(string userId, string token);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto, string origin);
        Task<AuthenticationResponse> LoginAsync(LoginDto loginDto);
        Task<RegisterResponse> RegisterAsync(RegisterDto registerDto, string origin);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task SignOutAsync();
        Task<RegisterResponse> RegisterBasicUserAsync(RegisterBasicUserDto registerDto);
        Task<ServiceResponse> GetUserByRolAsync(string rolId);
        Task<ServiceResponse> GetUserByCedulaAsync(string cedula);
        Task<ServiceResponse> UpdateIdentityUserAsync(UsuariosDto user, decimal? monto);
        Task<ServiceResponse> GetIdentityUserByASYNC(string userId);
        Task<ServiceResponse> ActivarOrDesactivarAsync(string userId);

    }
}
