

namespace InternetBanking.Application.Models.beneficiario
{
    public class PagoBeneficiarioModel
    {
        public string UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public string BeneficiarioUsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int CuentaBeneficiarioID { get; set; }
        public string CuentaBeneficiario { get; set; }
        public decimal Monto { get; set; }
        public string Detalles { get; set; }
    }
}
