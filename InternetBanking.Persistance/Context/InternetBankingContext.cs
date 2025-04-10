

using InternetBanking.Domain.Entities.dbo;
using Microsoft.EntityFrameworkCore;

namespace InternetBanking.Persistance.Context
{
    public partial class InternetBankingContext : DbContext
    {
        public InternetBankingContext(DbContextOptions<InternetBankingContext> options) : base(options)
        {
            
        }

        #region

        public DbSet<Beneficiarios> Beneficiarios { get; set; }
        public DbSet<CuentasAhorro> CuentasAhorros { get; set; }
        public DbSet<HistorialTransacciones> HistorialTransacciones { get; set; }
        public DbSet<Prestamos> Prestamos { get; set; }
        public DbSet<TarjetasCredito> TarjetasCreditos { get; set; }
        public DbSet<Transacciones> Transacciones { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }

        #endregion
    }
}
