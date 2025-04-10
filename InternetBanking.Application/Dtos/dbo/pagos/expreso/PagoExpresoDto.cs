namespace InternetBanking.Application.Dtos.dbo.pagos.expreso
{
    public class PagoExpresoDto
    {
        public int CuentaOrigenID { get; set; }
        public int CuentaDestinoID { get; set; }
        public string Cuenta { get; set; }
        public decimal Monto { get; set; }
    }
}
