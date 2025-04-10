

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternetBanking.Domain.Entities.dbo
{
    [Table("TarjetasCredito", Schema = "dbo")]
    public class TarjetasCredito
    {
        [Key]
        public int TarjetaID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoDeuda { get; set; }
        public decimal SaldoDisponible { get; set; }
        public string TipoTarjeta { get; set; }
        public string? Icono { get; set; }

    }
}
