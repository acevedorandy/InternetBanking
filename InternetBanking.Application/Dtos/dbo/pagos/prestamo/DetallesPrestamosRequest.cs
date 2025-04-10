

namespace InternetBanking.Application.Dtos.dbo.pagos.prestamo
{
    [Serializable]
    public class DetallesPrestamosRequest
    {
        public int CuentaID { get; set; }
        public int PrestamoID { get; set; }
        public decimal Monto { get; set; }

    }
}
