using InternetBanking.Application.Dtos.identity.account.@base;
using System.ComponentModel.DataAnnotations;


namespace InternetBanking.Application.Dtos.identity.account
{
    public class RegisterDto : BaseAccountDto
    {
        [Required(ErrorMessage = "Debe colocar su nombre.")]
        [DataType(DataType.Text)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe colocar su apellido.")]
        [DataType(DataType.Text)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Debe colocar la cedula.")]
        [DataType(DataType.Text)]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "Debe colocar su nombre de usuario.")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Debe colocar una contraseña.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coiciden.")]
        [Required(ErrorMessage = "Debe colocar una contraseña.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Debe colocar su correo.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe colocar su telefono.")]
        [DataType(DataType.Text)]
        public string Phone { get; set; }

    }
}
