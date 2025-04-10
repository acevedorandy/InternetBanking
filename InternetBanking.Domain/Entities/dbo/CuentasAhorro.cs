

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBanking.Domain.Entities.dbo
{
    [Table("CuentasAhorro", Schema = "dbo")]
    public class CuentasAhorro
    {
        [Key]
        public int CuentaID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public bool? Principal { get; set; }

    }
}
