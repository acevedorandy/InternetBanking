using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.identity.account;
using InternetBanking.Application.Models.ViewModel;
using InternetBanking.Persistance.Models.dbo;
using InternetBanking.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InternetBanking.Web.Controllers
{

    [ServiceFilter(typeof(LoginAuthorize))]
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ICuentasAhorroService _cuentasAhorroService;
        private readonly ITarjetasCreditoService _tarjetasCreditoService;
        private readonly IPrestamosService _prestamosService;


        public UsuariosController(IUsuarioService usuarioService, ICuentasAhorroService cuentasAhorroService,
                                  ITarjetasCreditoService tarjetasCreditoService, IPrestamosService prestamosService)
        {
            _usuarioService = usuarioService;
            _cuentasAhorroService = cuentasAhorroService;
            _tarjetasCreditoService = tarjetasCreditoService;
            _prestamosService = prestamosService;

        }
        public async Task<IActionResult> Index()
        {
            var result = await _usuarioService.GetAllAsync();

            if (result.IsSuccess)
            {
                List<UsuariosModel> usuariosModels = (List<UsuariosModel>)result.Model;
                return View(usuariosModels);
            }
            return View();
        }

        public async Task<IActionResult> ActivarOrDesactivar(string id)
        {
            var result = await _usuarioService.ActivarOrDesactivarAsync(id);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task <IActionResult> EditBasic(string id)
        {
            var result = await _usuarioService.GetIdentityUserByASYNC(id);

            if (result.IsSuccess)
            {
                EditUserModel usuarios = (EditUserModel)result.Model;
                return View(usuarios);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditBasic(UsuariosDto dto, decimal monto)
        {
            try
            {
                var result = await _usuarioService.UpdateIdentityUserAsync(dto, monto);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Messages;
                    return View(dto);
                }
            }
            catch (Exception)
            {
                return View();
            }
        }

        public async Task <IActionResult> EditAdmin(string id)
        {
            var result = await _usuarioService.GetIdentityUserByASYNC(id);

            if (result.IsSuccess)
            {
                EditUserModel usuarios = (EditUserModel)result.Model;
                return View(usuarios);
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EditAdmin(UsuariosDto dto, decimal? monto)
        {
            try
            {
                var result = await _usuarioService.UpdateIdentityUserAsync(dto, monto);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = result.Messages;
                    return View(dto);
                }
            }
            catch (Exception)
            {
                return View();
            }
        }

        public async Task<IActionResult> FiltrarRol(string rol)
        {
            var result = await _usuarioService.GetUserByRolAsync(rol);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction("Index");
            }

            var usuariosFiltrados = result.Model as List<UsuariosModel>;

            if (usuariosFiltrados == null || !usuariosFiltrados.Any())
            {
                TempData["ErrorMessage"] = "No se encontraron usuarios para este rol.";
            }

            TempData["Rol"] = rol;

            return View(usuariosFiltrados);
        }

        [HttpGet]
        public async Task<IActionResult> ListaDeProductos(string id)
        {
            ProductosViewModel productosView = new ProductosViewModel();

            var cuentasResult = await _cuentasAhorroService.GetTarjetaAsProductAsync(id);
            if (cuentasResult.IsSuccess)
            {
                productosView.Cuentas = (List<CuentasAhorroModel>)cuentasResult.Model;
            }

            var tarjetasResult = await _tarjetasCreditoService.GetTarjetaAsProductAsync(id);
            if (tarjetasResult.IsSuccess)
            {
                productosView.Tarjetas = (List<TarjetasCreditoModel>)tarjetasResult.Model;
            }

            var prestamosResult = await _prestamosService.GetPrestamoAsProductAsync(id);
            if (prestamosResult.IsSuccess)
            {
                productosView.Prestamos = (List<PrestamosModel>)prestamosResult.Model;
            }
            return View(productosView);
        }

        public async Task<IActionResult> EliminarPrestamo(int id)
        {
            var result = await _prestamosService.GetByIDAsync(id);

            if (result.IsSuccess)
            {
                PrestamosModel prestamos = (PrestamosModel)result.Model;
                return View(prestamos);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EliminarPrestamo(PrestamosDto dto)
        {
            var result = await _prestamosService.RemoveAsync(dto);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Prestamo eliminada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction(nameof(Index));

            }
        }

        public async Task<IActionResult> EliminarTarjeta(int id)
        {
            var result = await _tarjetasCreditoService.GetByIDAsync(id);

            if (result.IsSuccess)
            {
                TarjetasCreditoModel tarjetasCredito = (TarjetasCreditoModel)result.Model;
                return View(tarjetasCredito);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EliminarTarjeta(TarjetasCreditoDto dto)
        {
            var result = await _tarjetasCreditoService.RemoveAsync(dto);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Tarjeta eliminada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction(nameof(Index));

            }
        }

        public async Task<IActionResult> EliminarCuenta(int id)
        {
            var result = await _cuentasAhorroService.GetByIDAsync(id);

            if (result.IsSuccess)
            {
                CuentasAhorroModel cuentasAhorro = (CuentasAhorroModel)result.Model;
                return View(cuentasAhorro);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCuenta(CuentasAhorroDto dto)
        {
            var result = await _cuentasAhorroService.DeleteAccountAsync(dto.CuentaID);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Cuenta eliminada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Messages;
                return RedirectToAction(nameof(Index));

            }
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterBasicUserDto dto)
        {
            try
            {
                var result = await _usuarioService.RegisterBasicUserAsync(dto);

                if (!result.HasError)
                {
                    TempData["SuccessMessage"] = "La operacion fue exitosa.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Ha ocurrido un error.";
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
