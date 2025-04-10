using InternetBanking.Application.Enum.identity;
using InternetBanking.Identity.Shared.Entities;
using Microsoft.AspNetCore.Identity;


namespace InternetBanking.Identity.Seeds
{
    public static class SuperAdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser applicationUser = new();

            applicationUser.UserName = "superadmin";
            applicationUser.Nombre = "SuperAdmin";
            applicationUser.Apellido = "User";
            applicationUser.Email = "superadminuser@gmail.com";
            applicationUser.EmailConfirmed = true;
            applicationUser.PhoneNumberConfirmed = true;

            if (userManager.Users.All(u => u.Id != applicationUser.Id))
            {
                var user = await userManager.FindByEmailAsync(applicationUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(applicationUser, "123Pa$$word!");
                    await userManager.AddToRoleAsync(applicationUser, Roles.Basic.ToString());
                    await userManager.AddToRoleAsync(applicationUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(applicationUser, Roles.SuperAdmin.ToString());

                }
            }
        }
    }
}
