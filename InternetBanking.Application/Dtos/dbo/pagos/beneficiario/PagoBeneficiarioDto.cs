

namespace InternetBanking.Application.Dtos.dbo.pagos.beneficiario
{
    public class PagoBeneficiarioDto
    {
        public string UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public string BeneficiarioUsuarioID { get; set; }
        public int CuentaBeneficiarioID { get; set; }
        public string Tipo { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Monto { get; set; }
        public string? Detalles { get; set; }
    }
}
