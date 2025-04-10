using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.cuenta;
using InternetBanking.Application.Models.cuenta;
using InternetBanking.Application.Services.dbo;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Web.Helpers.transacciones;
using InternetBanking.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Web.Controllers
{
    public class CuentasAhorroController : Controller
    {
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private readonly ITransaccionesService _transaccionesService;
        private readonly CuentasHelper _cuentasHelper;

        public CuentasAhorroController(ICuentasAhorroService cuentasAhorroService, CuentasHelper cuentasHelper, ITransaccionesService transaccionesService)
        {
            _cuentasAhorroService = cuentasAhorroService;
            _cuentasHelper = cuentasHelper;
            _transaccionesService = transaccionesService;
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task <IActionResult> Index()
        {
            var result = await _cuentasAhorroService.GetAllAsync();

            if (result.IsSuccess)
            {
                List<CuentasAhorroModel> cuentasAhorroModels = (List<CuentasAhorroModel>)result.Model;
                return View(cuentasAhorroModels);
            }
            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task <IActionResult> TransaccionesPorCuenta(int id)
        {
            var result = await _transaccionesService.GettTransactionByAccountAsync(id);

            if (result.IsSuccess)
            {
                List<TransaccionesModel> transacciones = (List<TransaccionesModel>)result.Model;
                return View(transacciones);
            }
            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(CuentasAhorroDto cuentasAhorroDto)
        {
            try
            {
                var result = await _cuentasAhorroService.SaveAsync(cuentasAhorroDto);

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else 
                {
                    result.IsSuccess = false;
                    ViewBag.Meesage = result.Messages; 
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task<IActionResult> PagarCuenta()
        {
            var cuentasNo1 = await _cuentasHelper.GetAccountUser();
            var cuentasNo2 = await _cuentasHelper.GetAccountUser();

            ViewBag.CuentaOrigen = cuentasNo1;
            ViewBag.CuentaDestino = cuentasNo2;

            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpGet]
        public async Task<IActionResult> ProcesarPagarCuenta(int cuentaOrigenId, int cuentaDestinoId, decimal monto)
        {
            var request = new CuentaTransferenciaModel
            {
                CuentaOrigenID = cuentaOrigenId,
                CuentaDestinoID = cuentaDestinoId,
                Monto = monto
            };

            if (cuentaOrigenId == cuentaDestinoId)
            {
                TempData["ErrorMessage"] = "No puedes transferir a una misma cuenta.";
                return RedirectToAction("PagarCuenta");
            }

            var result = await _cuentasAhorroService.LoadPagoCuenta(cuentaOrigenId, cuentaDestinoId);

            if (!result.IsSuccess || result.Model == null)
            {
                TempData["ErrorMessage"] = "No se pudo recuperar la información de las cuentas.";
                return RedirectToAction("ResumenPagoCuenta");
            }

            ViewBag.Monto = monto;
            return View("ResumenPagoCuenta", result.Model);
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpGet]
        public IActionResult ResumenPagoCuenta(CuentaTransferenciaModel model, decimal monto) 
        {
            ViewBag.Monto = monto;
            ViewBag.CuentaID = model.CuentaOrigenID;
            return View(model);
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpPost]
        public async Task<IActionResult> ConfirmarPagoCuenta(PagoCuentaDto dto)
        {
            var result = await _cuentasAhorroService.PagoCuenta(dto);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pago realizado con éxito.";
                return RedirectToAction("Index", "CuentasAhorro");
            }
            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction("ProcesarPagarCuenta", new
                {
                    cuentaOrigenId = dto.CuentaOrigenID,
                    cuentaDestinoId = dto.CuentaDestinoID,
                    monto = dto.Monto
                });
            }
        }
    }
}
