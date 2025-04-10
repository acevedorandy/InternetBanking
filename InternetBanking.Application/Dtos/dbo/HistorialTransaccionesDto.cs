

namespace InternetBanking.Application.Dtos.dbo
{
    public class HistorialTransaccionesDto
    {
        public int HistorialID { get; set; }
        public int TransaccionID { get; set; }
        public string Estado { get; set; }
        public DateTime? Fecha { get; set; }

    }
}
