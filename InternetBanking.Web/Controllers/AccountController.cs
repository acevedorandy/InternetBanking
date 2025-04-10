using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Dtos.identity.account;
using InternetBanking.Application.Responses.identity;
using Microsoft.AspNetCore.Mvc;
using InternetBanking.Application.Helpers.web;
using Microsoft.AspNetCore.Identity;
using InternetBanking.Identity.Shared.Entities;

namespace InternetBanking.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IUsuarioService usuarioService, UserManager<ApplicationUser> userManager)
        {
            _usuarioService = usuarioService;
            _userManager = userManager;
        }

        public ActionResult Home()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            AuthenticationResponse authentication = await _usuarioService.LoginAsync(loginDto);

            if (authentication != null && authentication.HasError != true)
            {
                // Guardar el usuario en la sesión
                HttpContext.Session.Set<AuthenticationResponse>("usuario", authentication);

                // Obtener el usuario autenticado
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                // Verificar el rol del usuario y redirigir
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Welcome", "Account"); 
                }
                else if (await _userManager.IsInRoleAsync(user, "Basic"))
                {
                    return RedirectToAction("Welcome", "Account"); // Redirige a la vista de Beneficiarios si es Basic
                }
                return RedirectToAction("Home", "Account");
            }
            else
            {
                loginDto.HasError = authentication.HasError;
                loginDto.Error = authentication.Error;
                return View(loginDto);
            }
        }


        public IActionResult Register()
        {
            return View(new RegisterDto());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var origen = Request.Headers["origin"];

            RegisterResponse response = await _usuarioService.RegisterAsync(registerDto, origen);

            if (response.HasError)
            {
                registerDto.HasError = response.HasError;
                registerDto.Error = response.Error;
                return View(registerDto);
            }
            return RedirectToRoute(new { controller = "Account", action = "Index" });
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            string response = await _usuarioService.ConfirmEmailAsync(userId, token);
            return View("ConfirmEmail", response);
        }

        public async Task<IActionResult> LogOut()
        {
            await _usuarioService.SignOutAsync();
            HttpContext.Session.Remove("usuario");

            return RedirectToRoute(new { controller = "Account", action = "Index" });
        }

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordDto());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordDto);
            }

            var origin = Request.Headers["origin"];

            ForgotPasswordResponse response = await _usuarioService.ForgotPasswordAsync(forgotPasswordDto, origin);

            if (response.HasError)
            {
                forgotPasswordDto.HasError = response.HasError;
                forgotPasswordDto.Error = response.Error;
                return View(forgotPasswordDto);
            }

            return RedirectToRoute(new { controller = "Account", action = "Index" });
        }
        public IActionResult ResetPassword(string token)
        {
            return View(new ResetPasswordDto { Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordDto);
            }

            ResetPasswordResponse response = await _usuarioService.ResetPasswordAsync(resetPasswordDto);

            if (response.HasError)
            {
                resetPasswordDto.HasError = response.HasError;
                resetPasswordDto.Error = response.Error;
                return View(resetPasswordDto);
            }
            return RedirectToRoute(new { controller = "Account", action = "Index" });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            return View();
        }

    }
}
