

namespace InternetBanking.Application.Dtos.dbo
{
    public class CuentasAhorroDto
    {
        public int CuentaID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public bool? Principal { get; set; }

    }
}
