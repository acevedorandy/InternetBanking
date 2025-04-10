

using InternetBanking.Application.Dtos.identity.account.@base;
using System.ComponentModel.DataAnnotations;

namespace InternetBanking.Application.Dtos.identity.account
{
    public class ResetPasswordDto : BaseAccountDto
    {
        [Required(ErrorMessage = "Debe colocar su correo.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe tener un token.")]
        [DataType(DataType.Text)]
        public string Token { get; set; }

        [Required(ErrorMessage = "Debe colocar una contraseña.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe colocar una contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

    }
}
