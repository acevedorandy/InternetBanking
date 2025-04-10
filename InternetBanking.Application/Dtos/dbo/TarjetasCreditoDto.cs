

using Microsoft.AspNetCore.Http;

namespace InternetBanking.Application.Dtos.dbo
{
    public class TarjetasCreditoDto
    {
        public int TarjetaID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroTarjeta { get; set; }
        public decimal LimiteCredito { get; set; }
        public decimal SaldoDeuda { get; set; }
        public decimal SaldoDisponible { get; set; }
        public string TipoTarjeta { get; set; }
        public string? Icono { get; set; }
        public IFormFile File { get; set; }

    }
}
