
using System.ComponentModel.DataAnnotations;

namespace InternetBanking.Persistance.Models.ViewModels.beneficiario
{
    public class BeneficiariosViewModel
    {
        public int BeneficiarioID { get; set; }
        public string UsuarioID { get; set; }
        public string BeneficiarioUsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        [Display(Name = "Numero de cuenta")]
        public string CuentaBeneficiario { get; set; }
        public string Alias { get; set; }
    }
}
