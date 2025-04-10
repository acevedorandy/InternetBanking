

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBanking.Domain.Entities.dbo
{
    [Table("Usuarios", Schema = "dbo")]
    public class Usuarios
    {
        [Key]
        public string Nombre { get; set; }
    }
}
