

using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.identity;
using InternetBanking.Application.Dtos.identity.account;
using InternetBanking.Application.Responses.identity;

namespace InternetBanking.Application.Contracts.identity
{
    public interface IAccountService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest authenticationRequest);
        Task SignOutAsync();
        Task<RegisterResponse> RegisterAdminUserAsync(RegisterRequest request, string origin);
        Task<string> ConfirmAccountAsync(string userId, string token);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task<RegisterResponse> RegisterBasicUserAsync(RegisterBasicUserDto request);
    }
}
