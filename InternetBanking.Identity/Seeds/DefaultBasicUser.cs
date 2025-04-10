

using InternetBanking.Application.Enum.identity;
using InternetBanking.Identity.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace InternetBanking.Identity.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser applicationUser = new ApplicationUser();

            applicationUser.Nombre = "basic";
            applicationUser.Apellido = "user";
            applicationUser.UserName = "basicuser";
            applicationUser.Email = "basicuser@gmail.com";
            applicationUser.EmailConfirmed = true;
            applicationUser.PhoneNumberConfirmed = true;

            if (userManager.Users.All(u => u.Id != applicationUser.Id))
            {
                var user = await userManager.FindByEmailAsync(applicationUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(applicationUser, "123Pa$$word!");
                    await userManager.AddToRoleAsync(applicationUser, Roles.Basic.ToString());
                }
            }
        }
    }
}
