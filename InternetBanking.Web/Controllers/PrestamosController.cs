using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.prestamo;
using InternetBanking.Application.Models.prestamos;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Web.Helpers.transacciones;
using InternetBanking.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Web.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly IPrestamosService _prestamosService;
        private readonly CuentasHelper _cuentasHelper;

        public PrestamosController(IPrestamosService prestamosService, CuentasHelper cuentasHelper)
        {
            _prestamosService = prestamosService;
            _cuentasHelper = cuentasHelper;
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task <IActionResult> Index()
        {
            var result = await _prestamosService.GetAllAsync();

            if (result.IsSuccess)
            {
                List<PrestamosModel> prestamosModels = (List<PrestamosModel>)result.Model;
                return View(prestamosModels);
            }
            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task <IActionResult> Details(int id)
        {
            var result = await _prestamosService.GetByIDAsync(id);

            if (result.IsSuccess)
            {
                PrestamosModel prestamosModel = (PrestamosModel)result.Model;
                return View(prestamosModel);
            }
            return View();
        }


        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task<IActionResult> PagarPrestamo()
        {
            var cuentas = await _cuentasHelper.GetAccountUser();
            var prestamos = await _cuentasHelper.GetPrestamos();

            ViewBag.Cuentas = cuentas;
            ViewBag.Prestamos = prestamos;

            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpGet]
        public async Task<IActionResult> ProcesarPagoPrestamo(int cuentaID, int prestamoID, decimal monto)
        {
            var request = new DetallesPrestamosRequest
            {
                CuentaID = cuentaID,
                PrestamoID = prestamoID,
                Monto = monto
            };

            var result = await _prestamosService.LoadPagoPrestamoView(request.CuentaID, request.PrestamoID);

            if (!result.IsSuccess || result.Model == null)
            {
                TempData["Error"] = "No se pudo recuperar la información de las cuentas.";
                return RedirectToAction("PagarPrestamo");
            }

            ViewBag.Monto = request.Monto;
            return View("ResumenPagoPrestamo", result.Model);
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpGet]
        public IActionResult ResumenPagoPrestamo(PagoPrestamoModel model, decimal monto)
        {
            ViewBag.Monto = monto;  
            return View(model);
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpPost]
        public async Task<IActionResult> ConfirmarPagoPrestamo(PagoPrestamoDto dto)
        {
            var result = await _prestamosService.PagoPrestamo(dto);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pago realizado con éxito.";
                return RedirectToAction("Index", "Pagos");
            }
            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction("ProcesarPagoPrestamo", new
                {
                    cuentaID = dto.CuentaID,
                    prestamoID = dto.PrestamoID,
                    monto = dto.Monto
                });
            }
        }

    }
}
