


namespace InternetBanking.Application.Models.tarjetas
{
    public class AvanceEfectivoModel
    {
        public int CuentaID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroCuenta { get; set; }
        public int TarjetaID { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal SaldoDisponible { get; set; }

    }
}
