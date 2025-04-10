using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Web.Controllers
{
    [ServiceFilter(typeof(LoginAuthorize))]
    [Authorize(Roles = "Admin")]
    public class TransaccionesController : Controller
    {
        private readonly ITransaccionesService _transaccionesService;

        public TransaccionesController(ITransaccionesService transaccionesService)
        {
            _transaccionesService = transaccionesService;
        }

        public async Task <IActionResult> Transferencias()
        {
            var result = await _transaccionesService.GetAllAsync();

            if (result.IsSuccess)
            {
                List<TransaccionesModel> transaccionesModels = (List<TransaccionesModel>)result.Model;
                return View(transaccionesModels);
            }
            return View();
        }

        public async Task<IActionResult> ByFechas(DateTime fecha)
        {
            var result = await _transaccionesService.GetAllByDateAsync(fecha);

            if (result.IsSuccess)
            {
                List<TransaccionesModel> transaccionesModels = (List<TransaccionesModel>)result.Model;
                return View(transaccionesModels);
            }
            return View();
        }

        public async Task<IActionResult> Pagos()
        {
            var result = await _transaccionesService.GetAllPagoAsync();

            if (result.IsSuccess)
            {
                List<TransaccionesModel> transacciones = (List<TransaccionesModel>)result.Model;
                return View(transacciones);
            }
            return View();
        }

        public async Task<IActionResult> ByFechasPagos(DateTime fecha)
        {
            var result = await _transaccionesService.GetAllPagoByDateAsync(fecha);

            if (result.IsSuccess)
            {
                List<TransaccionesModel> transacciones = (List<TransaccionesModel>)result.Model;
                return View(transacciones);
            }
            return View();
        }

        public ActionResult Details(int id)
        {
            return View();
        }


    }
}
