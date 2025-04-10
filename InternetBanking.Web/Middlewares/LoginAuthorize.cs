using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Web.Middlewares
{
    public class LoginAuthorize : IAsyncActionFilter
    {
        private ValidateUserSesion _userSesion;

        public LoginAuthorize(ValidateUserSesion validateUserSesion)
        {
            _userSesion = validateUserSesion;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_userSesion.HasUser())
            {
                var controller = (Controller)context.Controller;
                context.Result = controller.RedirectToAction("Home", "Account");
                return;
            }

            await next();
        }

    }
}
