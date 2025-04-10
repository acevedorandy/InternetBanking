
namespace InternetBanking.Application.Dtos.dbo.avance
{
    public class AvanceEfectivoDto
    {
        public int CuentaID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroCuenta { get; set; }
        public int TarjetaID { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal Monto { get; set; }
        public string Tipo { get; set; }
        public DateTime? Fecha { get; set; } = DateTime.Now;

    }
}
