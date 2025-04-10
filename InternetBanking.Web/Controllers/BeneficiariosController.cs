using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.pagos.beneficiario;
using InternetBanking.Application.Models.beneficiario;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Persistance.Models.ViewModels.beneficiario;
using InternetBanking.Web.Helpers.transacciones;
using InternetBanking.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Web.Controllers
{
    [ServiceFilter(typeof(LoginAuthorize))]
    [Authorize(Roles = "Basic")]
    public class BeneficiariosController : Controller
    {
        private readonly IBeneficiariosService _beneficiariosService;
        private readonly CuentasHelper _cuentasHelper;

        public BeneficiariosController(IBeneficiariosService beneficiariosService, CuentasHelper cuentasHelper)
        {
            _beneficiariosService = beneficiariosService;
            _cuentasHelper = cuentasHelper;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _beneficiariosService.GetBeneficieresAsync();

            if (result.IsSuccess)
            {
                List<BeneficiariosViewModel> beneficiariosModels = (List<BeneficiariosViewModel>)result.Model;
                return View(beneficiariosModels);
            }
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BeneficiariosDto beneficiariosDto)
        {
            try
            {
                var relation = await _beneficiariosService.ExistRelationAsync(beneficiariosDto);

                if (relation)
                {
                    TempData["ErrorMessage"] = "Esta cuenta ya se encuentra añadida";
                    return View(beneficiariosDto);
                }
                var result = await _beneficiariosService.SaveAsync(beneficiariosDto);

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Message = result.Messages;
                    return View(beneficiariosDto);
                }
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _beneficiariosService.GetByIDAsync(id);

            if (result.IsSuccess)
            {
                BeneficiariosModel beneficiariosModel = (BeneficiariosModel)result.Model;
                return View(beneficiariosModel);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BeneficiariosDto beneficiariosDto)
        {
            try
            {
                var result = await _beneficiariosService.UpdateAsync(beneficiariosDto);

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    result.IsSuccess = false;
                    ViewBag.Message = result.Messages;

                    return View(beneficiariosDto);
                }
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _beneficiariosService.GetByIDAsync(id);

            if (result.IsSuccess)
            {
                BeneficiariosModel beneficiariosModel = (BeneficiariosModel)result.Model;
                return View(beneficiariosModel);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(BeneficiariosDto beneficiariosDto)
        {
            try
            {
                var result = await _beneficiariosService.RemoveAsync(beneficiariosDto);

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Message = result.Messages;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> PagarBeneficiario()
        {
            var cuentas = await _cuentasHelper.GetAccountUser();
            var beneficiarios = await _cuentasHelper.GetBeneficiarios();
            var beneficiarioId = await _cuentasHelper.GetBeneficiarioID();

            ViewBag.Cuentas = cuentas;
            ViewBag.Beneficiarios = beneficiarios;
            ViewBag.BeneficiariosID = beneficiarioId;

            if (beneficiarios.Any())
            {
                ViewBag.BeneficiariosID = beneficiarioId.First().Value;
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ProcesarPagoBeneficiario(int cuentaId, int cuentaBeneficiarioId, string usuarioId, decimal monto, string detalles)
        {
            var request = new PagoBeneficiarioModel
            {
                BeneficiarioUsuarioID = usuarioId,
                CuentaID = cuentaId,
                CuentaBeneficiarioID = cuentaBeneficiarioId,
                Monto = monto
            };

            var result = await _beneficiariosService.LoadPagoBeneficiario(cuentaId, cuentaBeneficiarioId, usuarioId);

            if (!result.IsSuccess || result.Model == null)
            {
                TempData["Error"] = "No se pudo recuperar la información de las cuentas.";
                return RedirectToAction("PagarPrestamo");
            }

            ViewBag.Monto = request.Monto;
            ViewBag.Detalles = detalles;
            ViewBag.BeneficiarioUsuarioID = usuarioId;
            return View("ResumenPagoBeneficiario", result.Model);
        }

        [HttpGet]
        public IActionResult ResumenPagoBeneficiario(PagoBeneficiarioModel model, decimal monto, string detalles)
        {
            ViewBag.Monto = monto;
            ViewBag.Detalles = detalles;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarPagoBeneficiario(PagoBeneficiarioDto dto)
        {
            var result = await _beneficiariosService.PagoBeneficiario(dto);

           if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Pago realizado con éxito.";
                return RedirectToAction("Index", "Pagos");
            }
            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction("ProcesarPagoBeneficiario", new
                {
                    cuentaID = dto.CuentaID,
                    cuentaBeneficiarioID = dto.CuentaBeneficiarioID,
                    usuarioId = dto.BeneficiarioUsuarioID,
                    monto = dto.Monto,
                    detalles = dto.Detalles
                });
            }
        }
    }
}
