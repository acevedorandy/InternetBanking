

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBanking.Domain.Entities.dbo
{
    [Table("HistorialTransacciones", Schema = "dbo")]
    public class HistorialTransacciones
    {
        [Key]
        public int HistorialID { get; set; }
        public int TransaccionID { get; set; }
        public string Estado { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
