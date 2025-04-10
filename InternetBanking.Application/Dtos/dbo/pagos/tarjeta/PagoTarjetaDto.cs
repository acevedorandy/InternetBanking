

namespace InternetBanking.Application.Dtos.dbo.pagos.tarjeta
{
    public class PagoTarjetaDto
    {
        public string UsuarioID { get; set; }
        public int TarjetaID { get; set; }
        public decimal Monto { get; set; }
        public int CuentaID { get; set; }
        public decimal SaldoDisponible { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Tipo { get; set; }
    }
}
