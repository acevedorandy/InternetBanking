

using InternetBanking.Infraestructure.Interfaces;
using InternetBanking.Infraestructure.Services;
using InternetBanking.Infraestructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InternetBanking.IOC.Dependencies.infraestructure
{
    public static class EmailDependency
    {
        public static void AddEmailDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
