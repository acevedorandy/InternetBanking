
namespace InternetBanking.Persistance.Models.dbo
{
    public class HistorialTransaccionesModel
    {
        public int HistorialID { get; set; }
        public int TransaccionID { get; set; }
        public string Estado { get; set; }
        public DateTime? Fecha { get; set; }

    }
}
