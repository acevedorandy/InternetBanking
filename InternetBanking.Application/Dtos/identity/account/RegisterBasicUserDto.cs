

namespace InternetBanking.Application.Dtos.identity.account
{
    public class RegisterBasicUserDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; } = true;


        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public bool? Principal { get; set; }
    }
}
