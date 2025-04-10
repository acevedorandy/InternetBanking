

using System.ComponentModel.DataAnnotations;

namespace InternetBanking.Application.Models.ViewModel
{
    public class EditUserModel
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }

        [Display(Name = "Correo")]
        public string Email { get; set; }

        [Display(Name = "Usuario")]
        public string UserName { get; set; }
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coiciden.")]
        [Required(ErrorMessage = "Debe colocar una contraseña.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Telefono")]
        public string Phone { get; set; }
        public decimal? Monto { get; set; }

    }
}
