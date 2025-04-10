

namespace InternetBanking.Application.Dtos.dbo.pagos.prestamo
{
    public class PagoPrestamoDto
    {
        public string UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public string Tipo { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Monto { get; set; }
        public int PrestamoID { get; set; }

    }
}
