


namespace InternetBanking.Application.Dtos.dbo.pagos.cuenta
{
    public class PagoCuentaDto
    {
        public string UsuarioID { get; set; }
        public int CuentaOrigenID { get; set; }
        public int CuentaDestinoID { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

    }
}
