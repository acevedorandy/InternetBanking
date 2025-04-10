

namespace InternetBanking.Application.Models.cuenta
{
    public class CuentaTransferenciaModel
    {
        public int CuentaID { get; set; }
        public int CuentaOrigenID { get; set; }
        public string NumeroCuentaOrigen { get; set; }
        public decimal Saldo { get; set; }
        public int CuentaDestinoID { get; set; }
        public string NumeroCuentaDestino { get; set; }
        public decimal Monto { get; set; }
    }
}
