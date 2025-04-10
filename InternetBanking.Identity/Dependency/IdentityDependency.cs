using InternetBanking.Application.Contracts.identity;
using InternetBanking.Identity.Helpers;
using InternetBanking.Identity.Seeds;
using InternetBanking.Identity.Services;
using InternetBanking.Identity.Shared.Context;
using InternetBanking.Identity.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InternetBanking.Identity.Dependency
{
    public static class IdentityDependency
    {
        public static void AddIdentityDependency(this IServiceCollection services, IConfiguration configuration)
        {
            #region Context
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityContext>(options => options.UseInMemoryDatabase("IdentityDb"));
            }
            else
            {
                services.AddDbContext<IdentityContext>(options =>
                {
                    options.EnableSensitiveDataLogging();
                    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
                        m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
                });
            }
            #endregion

            #region Identity (solo UNA VEZ)
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                // Configuración de seguridad
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Default");
            #endregion

            #region Token Lifetime
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromSeconds(300);
            });
            #endregion

            #region Auth
            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = IdentityConstants.ApplicationScheme;
                opt.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                opt.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
            }).AddCookie(IdentityConstants.ApplicationScheme, opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromHours(24);
                opt.LoginPath = "/Account";
                opt.AccessDeniedPath = "/Account/AccessDenied";
            });
            #endregion
        }

        public static async Task RunIdentitySeeds(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider;

                try
                {
                    var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

                    // Seeds
                    await DefaultRoles.SeedAsync(userManager, roleManager);
                    await DefaultBasicUser.SeedAsync(userManager, roleManager);
                    await DefaultAdminUser.SeedAsync(userManager, roleManager);
                    await SuperAdminUser.SeedAsync(userManager, roleManager);

                    Console.WriteLine("✔️ Seeds ejecutadas correctamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error en Seeds: {ex.Message}");
                }
            }
        }

        public static void AddIdentityService(this IServiceCollection services)
        {
            services.AddTransient<IAccountService, AccountService>();
            //services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<EmailHelper>();
            services.AddTransient<AccountGenerator>();
            
        }
    }
}
