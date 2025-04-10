

namespace InternetBanking.Application.Dtos.dbo.pagos.tarjeta
{
    public class DetallesPagoTarjetaDto
    {
        public int CuentaID { get; set; }
        public int TarjetaID { get; set; }
        public decimal Monto { get; set; }

    }
}
