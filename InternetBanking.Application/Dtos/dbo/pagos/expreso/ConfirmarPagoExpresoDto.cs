

namespace InternetBanking.Application.Dtos.dbo.pagos.expreso
{
    public class ConfirmarPagoExpresoDto
    {
        public string UsuarioID { get; set; }
        public int CuentaOrigenID { get; set; }
        public int CuentaID { get; set; }
        public string NumeroCuenta { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; } = DateTime.Now;
        public string? Detalles { get; set; }

    }
}
