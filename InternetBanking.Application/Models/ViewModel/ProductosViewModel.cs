using InternetBanking.Persistance.Models.dbo;


namespace InternetBanking.Application.Models.ViewModel
{
    public class ProductosViewModel
    {
        public IEnumerable<CuentasAhorroModel> Cuentas { get; set; }
        public IEnumerable<TarjetasCreditoModel> Tarjetas { get; set; }
        public IEnumerable<PrestamosModel> Prestamos { get; set; }
    }
}
