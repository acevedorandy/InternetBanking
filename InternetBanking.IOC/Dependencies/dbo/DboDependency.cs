

using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Helpers;
using InternetBanking.Application.Mapping.dbo;
using InternetBanking.Application.Services.dbo;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Persistance.Repositories.Dbo;
using Microsoft.Extensions.DependencyInjection;

namespace InternetBanking.IOC.Dependencies.dbo
{
    public static class DboDependency
    {
        public static void AddDboDependency(this IServiceCollection services)
        {
            // Repositorios
            services.AddTransient<IBeneficiariosRepository, BeneficiariosRepository>();
            services.AddTransient<ICuentasAhorroRepository, CuentasAhorroRepository>();
            services.AddTransient<IHistorialTransaccionesRepository, HistorialTransaccionesRepository>();
            services.AddTransient<IPrestamosRepository, PrestamosRepository>();
            services.AddTransient<ITarjetasCreditoRepository, TarjetasCreditoRepository>();
            services.AddTransient<ITransaccionesRepository, TransaccionesRepository>();
            services.AddTransient<IUsuariosRepository, UsuariosRepository>();

            // Servicios
            services.AddScoped<IBeneficiariosService, BeneficiariosService>();
            services.AddScoped<ICuentasAhorroService, CuentasAhorroService>();
            services.AddScoped<IHistorialTransaccionesService, HistorialTransaccionesService>();
            services.AddScoped<IPrestamosService, PrestamosService>();
            services.AddScoped<ITarjetasCreditoService, TarjetasCreditoService>();
            services.AddScoped<ITransaccionesService, TransaccionesService>();
            services.AddScoped<IUsuarioService, UsuariosService>();

            // AutoMaper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(typeof(DboMapping));

            // Helpers
            services.AddScoped<GeneradosNumeros>();
        }
    }
}
