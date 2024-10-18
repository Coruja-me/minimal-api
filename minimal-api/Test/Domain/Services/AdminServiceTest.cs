using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.DB;

namespace Test.Domain.Services
{
    [TestClass]
    public class AdminServiceTest
    {
        private DBContext CreateContext(){

            //Builder
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            var config = builder.Build();

            return new DBContext(config);
        }
        [TestMethod]
        public void SalvarAdmin()
        {
            //Arranges
            var ctxt = CreateContext();
            ctxt.Database.ExecuteSqlRaw("TRUNCATE TABLE Admins");

            var adm = new Admin {
                Id = 1,
                Email = "teste@teste.com",
                Senha = "teste",
                Perfil = "Admin"
            };
            
            var aService = new AdminService(ctxt);

            //Actions
            aService.Incluir(adm);

            //Asserts
            Assert.AreEqual(1, aService.Admins(0).Count());
        }
        [TestMethod]
        public void CriarEBuscarAdmin()
        {
            //Arranges
            var ctxt = CreateContext();
            ctxt.Database.ExecuteSqlRaw("TRUNCATE TABLE Admins");

            var adm = new Admin {
                Id = 1,
                Email = "teste@teste.com",
                Senha = "teste",
                Perfil = "Admin"
            };
            
            var aService = new AdminService(ctxt);

            //Actions
            aService.Incluir(adm);
            adm = aService.LerId(adm.Id);

            //Asserts
            Assert.AreEqual(1, adm.Id);
        }
    }
}