using Azure;
using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.avance;
using InternetBanking.Application.Dtos.dbo.pagos.tarjeta;
using InternetBanking.Application.Models.tarjetas;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Web.Helpers.photos;
using InternetBanking.Web.Helpers.transacciones;
using InternetBanking.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Web.Controllers
{
    public class TarjetasCreditoController : Controller
    {
        private readonly ITarjetasCreditoService _tarjetasCreditoService;
        private readonly CuentasHelper _cuentasHelper;
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private readonly PhotosHelper _photosHelper;

        public TarjetasCreditoController(ITarjetasCreditoService tarjetasCreditoService, CuentasHelper cuentasHelper,
                                         ICuentasAhorroService cuentasAhorroService, PhotosHelper photosHelper)
        {
            _tarjetasCreditoService = tarjetasCreditoService;
            _cuentasHelper = cuentasHelper;
            _cuentasAhorroService = cuentasAhorroService;
            _photosHelper = photosHelper;
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task<IActionResult> Index()
        {
            var result = await _tarjetasCreditoService.GetAllAsync();

            if (result.IsSuccess)
            {
                List<TarjetasCreditoModel> tarjetasCreditoModels = (List<TarjetasCreditoModel>)result.Model;
                return View(tarjetasCreditoModels);
            }
            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _tarjetasCreditoService.GetByIDAsync(id);

            if (result.IsSuccess)
            {
                TarjetasCreditoModel tarjetasCredito = (TarjetasCreditoModel)result.Model;
                return View(tarjetasCredito);
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
        public async Task<IActionResult> Create(TarjetasCreditoDto dto, IFormFile file)
        {
            try
            {
                var result = await _tarjetasCreditoService.SaveAsync(dto);

                if (result.IsSuccess)
                {
                    dto = result.Model;
                    dto = await _photosHelper.LoadPhoto(dto, file);
                }
                if (dto.Icono != null && dto.TarjetaID > 0)
                {
                    await _tarjetasCreditoService.UpdateAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = result.Messages;
                    return View(dto);
                }
            }
            catch
            {
                return View();
            }
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task<IActionResult> Avance(int id)
        {
            var cuentas = await _cuentasHelper.GetAccountUser();
            var tarjeta = await _tarjetasCreditoService.GetByIDAsync(id);

            if (!tarjeta.IsSuccess || tarjeta.Model == null)
            {
                TempData["Error"] = "No se encontró la tarjeta.";
                return RedirectToAction("Index");
            }

            var avanceDto = new AvanceEfectivoDto
            {
                TarjetaID = tarjeta.Model.TarjetaID,
                NumeroTarjeta = tarjeta.Model.NumeroTarjeta,
                Monto = 0
            };

            // Verifica que cuentas tiene datos
            if (cuentas == null || cuentas.Count == 0)
            {
                TempData["Error"] = "No hay cuentas disponibles.";
                return RedirectToAction("Index");
            }

            ViewBag.Cuentas = cuentas;

            return View(avanceDto);
        }


        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpGet]
        public async Task<IActionResult> ProcesarAvance(int cuentaId, int tarjetaId, decimal monto)
        {
            var request = new AvanceEfectivoDto
            {
                CuentaID = cuentaId,
                TarjetaID = tarjetaId,
                Monto = monto
            };

            var result = await _tarjetasCreditoService.LoadAvanceView(cuentaId, tarjetaId);

            if (!result.IsSuccess || result.Model == null)
            {
                TempData["Error"] = "Ha ocurrido un error.";
                return RedirectToAction("Index");
            }

            ViewBag.Monto = monto;
            return View("ResumenAvance", result.Model);
        }


        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpGet]
        public IActionResult ResumenAvance(AvanceEfectivoModel model, decimal monto)
        {
            ViewBag.Monto = monto;
            return View(model);
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpPost]
        public async Task<IActionResult> ConfirmarAvanceEfectivo(AvanceEfectivoDto dto)
        {
            var result = await _tarjetasCreditoService.AvanceEfectivo(dto);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Transacción realizada con éxito.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction("ProcesarAvance", new
                {
                    cuentaID = dto.CuentaID,
                    tarjetaID = dto.TarjetaID,
                    monto = dto.Monto
                });
            }
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        public async Task<IActionResult> PagarTarjeta()
        {
            var cuentas = await _cuentasHelper.GetAccountUser();
            var tarjetas = await _cuentasHelper.GetCreditCard();

            ViewBag.Cuentas = cuentas;
            ViewBag.Tarjetas = tarjetas;

            return View();
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpPost]
        public async Task<IActionResult> ProcesarPagarTarjeta(DetallesPagoTarjetaDto dto) 
        {
            if (!ModelState.IsValid)
            {
                return View("PagarTarjeta");
            }

            var result = await _tarjetasCreditoService.LoadPagoView(dto.CuentaID, dto.TarjetaID);

            if (!result.IsSuccess || result.Model == null)
            {
                TempData["Error"] = "No se pudo recuperar la información de las cuentas.";
                return RedirectToAction("PagarTarjeta");
            }

            ViewBag.Monto = dto.Monto;
            return View("ResumenPagoTarjeta", result.Model);
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpGet]
        public IActionResult ResumenPagoTarjeta(PagoTarjetaModel model, decimal monto) 
        { 
            ViewBag.Monto = monto;
            return View(model);
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Basic")]
        [HttpPost]
        public async Task<IActionResult> ConfirmarPagoTarjeta(PagoTarjetaDto dto) 
        {
            var result = await _tarjetasCreditoService.PagoTarjeta(dto);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pago realizado con éxito.";
                return RedirectToAction("Index", "Pagos");
            }
            else
            {
                TempData["Success"] = "Ha ocurrido un fallo.";
                return RedirectToAction("Index", "Pagos");
            }
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Admin")]
        public async Task <IActionResult> Delete(int id)
        {
            var result = await _tarjetasCreditoService.GetByIDAsync(id);

            if (result.IsSuccess)
            {
                TarjetasCreditoModel tarjetasCredito = (TarjetasCreditoModel)result.Model;
                return View(tarjetasCredito);
            }
            return View();
        }


        [ServiceFilter(typeof(LoginAuthorize))]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Delete(TarjetasCreditoDto dto)
        {
            try
            {
                var result = await _tarjetasCreditoService.RemoveAsync(dto);

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Error = result.Messages;
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
