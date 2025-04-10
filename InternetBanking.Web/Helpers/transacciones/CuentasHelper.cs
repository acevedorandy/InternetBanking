using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Responses.identity;
using InternetBanking.Application.Helpers.web;
using Microsoft.AspNetCore.Mvc.Rendering;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Application.Enum.tarjeta;

namespace InternetBanking.Web.Helpers.transacciones
{
    public class CuentasHelper
    {
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private readonly ITarjetasCreditoService _tarjetasCreditoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IPrestamosService _prestamosService;
        private readonly IBeneficiariosService _beneficiariosService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationResponse authentication;

        public CuentasHelper(ICuentasAhorroService cuentasAhorroService, ITarjetasCreditoService tarjetasCreditoService, IUsuarioService usuarioService, IHttpContextAccessor httpContextAccessor,
                             IPrestamosService prestamosService, IBeneficiariosService beneficiariosService)
        {
            _cuentasAhorroService = cuentasAhorroService;
            _tarjetasCreditoService = tarjetasCreditoService;
            _usuarioService = usuarioService;
            _httpContextAccessor = httpContextAccessor;
            authentication = _httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("usuario");
            _prestamosService = prestamosService;
            _beneficiariosService = beneficiariosService;
        }
        public async Task<List<SelectListItem>> GetAccountUser()
        {
            var cuentasList = new List<SelectListItem>();

            try
            {
                var response = await _cuentasAhorroService.GetAllAsync();

                if (response.IsSuccess && response.Model is List<CuentasAhorroModel> cuentas)
                {
                    cuentasList = cuentas.Select(c => new SelectListItem
                    {
                        Text = c.NumeroCuenta.ToString(),
                        Value = c.CuentaID.ToString()

                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return cuentasList; 
        }

        public async Task<List<SelectListItem>> GetCreditCard()
        {
            var cuentasList = new List<SelectListItem>();

            try
            {
                var response = await _tarjetasCreditoService.GetAllAsync();

                if (response.IsSuccess && response.Model is List<TarjetasCreditoModel> cuentas)
                {
                    cuentasList = cuentas.Select(c => new SelectListItem
                    {
                        Text = c.NumeroTarjeta.ToString(),
                        Value = c.TarjetaID.ToString()

                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return cuentasList;
        }

        public async Task<List<SelectListItem>> GetUsers()
        {
            var cuentasList = new List<SelectListItem>();

            try
            {
                var response = await _usuarioService.GetAllAsync();

                if (response.IsSuccess && response.Model is List<UsuariosModel> cuentas)
                {
                    cuentasList = cuentas.Select(c => new SelectListItem
                    {
                        Text = c.Nombre.ToString(),
                        Value = c.Id.ToString()

                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return cuentasList;
        }
        public async Task<List<SelectListItem>> GetPrestamos()
        {
            var cuentasList = new List<SelectListItem>();

            try
            {
                var response = await _prestamosService.GetAllAsync();

                if (response.IsSuccess && response.Model is List<PrestamosModel> cuentas)
                {
                    var prestamosFiltrados = cuentas.Where(c => c.Pagado == false).ToList(); 

                    cuentasList = prestamosFiltrados.Select(c => new SelectListItem
                    {
                        Text = c.NumeroPrestamo.ToString(),
                        Value = c.PrestamoID.ToString()
                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return cuentasList;
        }

        public async Task<List<SelectListItem>> GetBeneficiarios()
        {
            var cuentasList = new List<SelectListItem>();

            try
            {
                var response = await _beneficiariosService.GetAllAsync();

                if (response.IsSuccess && response.Model is List<BeneficiariosModel> beneficiarios)
                {
                    var beneficiariosFiltrados = beneficiarios.Where(c => c.UsuarioID == authentication.Id).ToList();

                    cuentasList = beneficiariosFiltrados.Select(c => new SelectListItem
                    {
                        Text = c.Alias.ToString(),
                        Value = c.CuentaBeneficiarioID.ToString()
                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return cuentasList;
        }
        public async Task<List<SelectListItem>> GetBeneficiarioID()
        {
            var cuentasList = new List<SelectListItem>();

            try
            {
                var response = await _beneficiariosService.GetAllAsync();

                if (response.IsSuccess && response.Model is List<BeneficiariosModel> beneficiarios)
                {
                    var beneficiariosFiltrados = beneficiarios.Where(c => c.UsuarioID == authentication.Id).ToList();

                    cuentasList = beneficiariosFiltrados.Select(c => new SelectListItem
                    {
                        Text = c.Alias.ToString(),
                        Value = c.BeneficiarioUsuarioID.ToString()
                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return cuentasList;
        }

        public async Task<List<SelectListItem>> TransferenciaCuenta(int? cuentaIdOrigen = null)
        {
            var cuentasList = new List<SelectListItem>();

            try
            {
                var response = await _cuentasAhorroService.GetAllAsync();

                if (response.IsSuccess && response.Model is List<CuentasAhorroModel> cuentas)
                {
                    if (cuentaIdOrigen.HasValue)
                    {
                        cuentas = cuentas.Where(c => c.CuentaID != cuentaIdOrigen.Value).ToList();
                    }

                    cuentasList = cuentas.Select(c => new SelectListItem
                    {
                        Text = c.NumeroCuenta.ToString(),
                        Value = c.CuentaID.ToString()

                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return cuentasList;
        }
        public async Task<List<SelectListItem>> GetAllUser()
        {
            var cuentasList = new List<SelectListItem>();

            try
            {
                var response = await _usuarioService.GetAllAsync();

                if (response.IsSuccess && response.Model is List<UsuariosModel> cuentas)
                {
                    cuentasList = cuentas.Select(c => new SelectListItem
                    {
                        Text = string.Concat(c.Nombre, " ", c.Apellido),
                        Value = c.Id.ToString()

                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return cuentasList;
        }

        public List<SelectListItem> GetTiposTarjeta()
        {
            return Enum.GetValues(typeof(Tipo))
                .Cast<Tipo>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(), 
                    Text = e.ToString()   
                }).ToList();
        }

    }
}
