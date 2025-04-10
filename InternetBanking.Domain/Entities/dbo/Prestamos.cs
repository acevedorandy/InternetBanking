
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBanking.Domain.Entities.dbo
{
    [Table("Prestamos", Schema = "dbo")]
    public class Prestamos
    {
        [Key]
        public int PrestamoID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroPrestamo { get; set; }
        public decimal Monto { get; set; }
        public decimal SaldoDeuda { get; set; }
        public bool Pagado { get; set; }
    }
}
