

using System.ComponentModel.DataAnnotations;

namespace InternetBanking.Persistance.Models.dbo
{
    public class TransaccionesModel
    {
        public int TransaccionID { get; set; }
        public string UsuarioID { get; set; }
        public int CuentaID { get; set; }

        [Display(Name = "Numero de cuenta")]
        public string NumeroCuenta { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string? Detalles { get; set; }

    }
}
