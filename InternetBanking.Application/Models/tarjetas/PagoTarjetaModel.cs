

namespace InternetBanking.Application.Models.tarjetas
{
    public class PagoTarjetaModel
    {
        public int TarjetaID { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal SaldoDeuda { get; set; }
        public int CuentaID { get; set; }
        public string NumeroCuenta { get; set; }
        public decimal SaldoDisponible { get; set; }
        public decimal Monto { get; set; }
    }
}

