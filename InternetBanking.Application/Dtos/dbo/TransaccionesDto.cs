

namespace InternetBanking.Application.Dtos.dbo
{
    public class TransaccionesDto
    {
        public int TransaccionID { get; set; }
        public string UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalles { get; set; }

    }
}
