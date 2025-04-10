

using System.ComponentModel.DataAnnotations;

namespace InternetBanking.Application.Models.prestamos
{
    public class PagoPrestamoModel
    {
        public int CuentaID { get; set; }

        [Required(ErrorMessage = "Seleccione una cuenta.")]
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }

        [Required(ErrorMessage = "Seleccione un prestamo.")]
        public int PrestamoID { get; set; }
        public string NumeroPrestamo { get; set; }
        public decimal SaldoDeuda { get; set; }
        public decimal Monto { get; set; }
    }
}

