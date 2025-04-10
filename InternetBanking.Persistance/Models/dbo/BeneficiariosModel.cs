
namespace InternetBanking.Persistance.Models.dbo
{
    public class BeneficiariosModel
    {
        public int BeneficiarioID { get; set; }
        public string UsuarioID { get; set; }
        public string BeneficiarioUsuarioID { get; set; }
        public int CuentaBeneficiarioID { get; set; }
        public string CuentaBeneficiario { get; set; }
        public string Alias { get; set; }

    }
}
