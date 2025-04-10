

using InternetBanking.Identity.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace InternetBanking.Identity.Helpers
{
    public class EmailHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public EmailHelper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> VerificationEmailURL(ApplicationUser user, string origin)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(await _userManager.GenerateEmailConfirmationTokenAsync(user))
            );

            var baseUri = new Uri($"{origin}/Account/ConfirmEmail");

            return QueryHelpers.AddQueryString(baseUri.ToString(), new Dictionary<string, string>
            {
            { "userId", user.Id },
            { "token", encodedToken }
            });
        }

        public async Task<string> ForgotPasswordURL(ApplicationUser user, string origin)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(await _userManager.GeneratePasswordResetTokenAsync(user))
            );

            var baseUri = new Uri($"{origin}/Account/ResetPassword");

            return QueryHelpers.AddQueryString(baseUri.ToString(), new Dictionary<string, string>
    {
        { "token", encodedToken }
    });
        }
    }
}
