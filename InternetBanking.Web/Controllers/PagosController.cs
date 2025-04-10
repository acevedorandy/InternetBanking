using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.expreso;
using InternetBanking.Persistance.Models.ViewModels.pagos;
using InternetBanking.Web.Helpers.transacciones;
using InternetBanking.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Web.Controllers
{
    [ServiceFilter(typeof(LoginAuthorize))]
    [Authorize(Roles = "Basic")]
    public class PagosController : Controller
    {
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private readonly ITransaccionesService _transaccionesService;
        private readonly CuentasHelper _cuentasHelper;

        public PagosController(ICuentasAhorroService cuentasAhorroService, ITransaccionesService transaccionesService,
                                CuentasHelper cuentasHelper)
        {
            _cuentasAhorroService = cuentasAhorroService;
            _transaccionesService = transaccionesService;
            _cuentasHelper = cuentasHelper;
        }
        public ActionResult Index()
        {
            return View();
        }

        public async Task <ActionResult> PagoExpreso()
        {
            var cuentas = await _cuentasHelper.GetAccountUser();

            ViewBag.Cuentas = cuentas;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProcesarPagoExpreso(string cuenta, int cuentaOrigenId, decimal monto)
        {
            var request = new PagoExpresoDto
            { 
                CuentaOrigenID = cuentaOrigenId,
                Cuenta = cuenta,
                Monto = monto
            };

            var response = await _transaccionesService.LoadExpresoView(cuenta, cuentaOrigenId);

            if (!response.IsSuccess || response.Model == null)
            {
                TempData["ErrorMessage"] = response.Messages;
                return RedirectToAction("PagoExpreso");
            }

            ViewBag.Monto = request.Monto;
            return View("ResumenPagoExpreso", response.Model);
        }

        [HttpGet]
        public IActionResult ResumenPagoExpreso(ExpresoViewModel model, decimal monto)
        {
            ViewBag.Monto = monto;
            return View(model);
        }

        [HttpPost]
        public async Task <IActionResult> ConfirmarPagoExpreso(ConfirmarPagoExpresoDto dto)
        {
            var result = await _transaccionesService.PagoExpresoAsync(dto);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pago realizado con éxito.";
                return RedirectToAction("Index", "Pagos");
            }
            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction("ProcesarPagoExpreso", new
                {
                    cuenta = dto.NumeroCuenta,
                    cuentaOrigenID = dto.CuentaOrigenID,
                    monto = dto.Monto
                });
            }
        }
    }
}
