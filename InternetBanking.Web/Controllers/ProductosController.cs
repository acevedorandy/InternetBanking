using Humanizer;
using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Web.Helpers.transacciones;
using InternetBanking.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Web.Controllers
{
    [ServiceFilter(typeof(LoginAuthorize))]
    [Authorize(Roles = "Admin")]
    public class ProductosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private readonly IPrestamosService _prestamosService;
        private readonly ITarjetasCreditoService _tarjetasCreditoService;
        private readonly CuentasHelper _cuentasHelper;

        public ProductosController(CuentasHelper cuentasHelper, IUsuarioService usuarioService, ICuentasAhorroService cuentasAhorroService, IPrestamosService prestamosService, ITarjetasCreditoService tarjetasCreditoService)
        {
            _usuarioService = usuarioService;
            _cuentasHelper = cuentasHelper;
            _cuentasAhorroService = cuentasAhorroService;
            _prestamosService = prestamosService;
            _tarjetasCreditoService = tarjetasCreditoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task <IActionResult> CrearCuenta()
        {
            var usuarios = await _cuentasHelper.GetAllUser();

            ViewBag.Usuarios = usuarios;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearCuenta(CuentasAhorroDto dto)
        {
            try
            {
                var result = await _cuentasAhorroService.SaveAsync(dto);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Cuenta agregada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Messages;
                    return View(dto);
                }
            }
            catch
            {
                return View();
            }

        }

        public async Task<IActionResult> CrearTarjeta()
        {
            var usuarios = await _cuentasHelper.GetAllUser();
            var tarjetas = _cuentasHelper.GetTiposTarjeta();

            ViewBag.Usuarios = usuarios;
            ViewBag.Tarjetas = tarjetas;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearTarjeta(TarjetasCreditoDto dto)
        {
            try
            {
                var result = await _tarjetasCreditoService.SaveAsync(dto);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Tarjeta creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Messages;
                    return View(dto);
                }
            }
            catch
            {
                return View();
            }
        }


        public async Task<ActionResult> CrearPrestamo()
        {
            var usuarios = await _cuentasHelper.GetUsers();
            ViewBag.Usuarios = usuarios;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CrearPrestamo(PrestamosDto dto)
        {
            try
            {
                var result = await _prestamosService.SaveAsync(dto);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Prestamo creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Messages;
                    return View(dto);
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
