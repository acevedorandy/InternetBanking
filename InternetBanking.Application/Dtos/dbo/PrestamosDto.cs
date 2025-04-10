

namespace InternetBanking.Application.Dtos.dbo
{
    public class PrestamosDto
    {
        public int PrestamoID { get; set; }
        public string UsuarioID { get; set; }
        public string NumeroPrestamo { get; set; }
        public decimal Monto { get; set; }
        public decimal SaldoDeuda { get; set; }
        public bool Pagado { get; set; }

    }
}
