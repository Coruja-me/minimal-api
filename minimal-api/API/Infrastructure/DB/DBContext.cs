using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.DB
{
    public class DBContext(IConfiguration config) : DbContext
    {
        private readonly IConfiguration _config = config;

        public DbSet<Admin> Admins { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder mBuilder)
        {
            mBuilder.Entity<Admin>().HasData(
                new Admin {
                    Id = 1,
                    Email = "adm@teste.com",
                    Senha = "admin",
                    Perfil = "Admin"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){

            if(!optionsBuilder.IsConfigured){

                var strConnection = _config.GetConnectionString("MySql")?.ToString();

                if(!string.IsNullOrEmpty(strConnection)){
                    optionsBuilder.UseMySql(
                        strConnection, 
                        ServerVersion.AutoDetect(strConnection));
                }
            }
        }
    }
}