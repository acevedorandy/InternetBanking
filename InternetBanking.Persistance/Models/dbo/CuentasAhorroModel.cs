

namespace InternetBanking.Persistance.Models.dbo
{
    public class CuentasAhorroModel
    {
        public int CuentaID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public bool? Principal { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

    }
}
