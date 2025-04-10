
using InternetBanking.Application.Dtos.identity.account.@base;
using System.ComponentModel.DataAnnotations;


namespace InternetBanking.Application.Dtos.identity.account
{
    public class ForgotPasswordDto : BaseAccountDto
    {
        [Required(ErrorMessage = "Debe colocar su correo.")]
        [DataType(DataType.Text)]
        public string Email { get; set; }
    }
}
