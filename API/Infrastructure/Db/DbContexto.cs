using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entity; 

namespace minimal_api.Infrastructure.Db
{   
    public class Contexto : DbContext 
    {
        private readonly IConfiguration _configuracaoAppSettings;

        public Contexto(IConfiguration configuracaoAppSettings)
        {
            _configuracaoAppSettings = configuracaoAppSettings;
        }
        public DbSet<Admin> admins {get; set;}
        public DbSet<Veiculos> veiculos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin {
                    Id = 1,
                    Email = "admin@teste.com",
                    Senha = "12345678",
                    Perfil = "adm"
                }
            );
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("ConexaoPadrao").ToString();
            if(!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseSqlServer(stringConexao);
            }
    
        }
    }

}