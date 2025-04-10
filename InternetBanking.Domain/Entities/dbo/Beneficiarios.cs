

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBanking.Domain.Entities.dbo
{
    [Table("Beneficiarios", Schema = "dbo")]
    public class Beneficiarios
    {
        [Key]
        public int BeneficiarioID { get; set; }
        public string UsuarioID { get; set; }
        public string BeneficiarioUsuarioID { get; set; }
        public string CuentaBeneficiario { get; set; }
        public string Alias { get; set; }
    }
}
