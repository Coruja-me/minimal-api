using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minimal_api.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class AdminTest
    {
        [TestMethod]
        public void TestGetSetProp()
        {
            //Arranges
            var adm = new Admin
            {
                //Actions
                Id = 1,
                Email = "teste@teste.com",
                Senha = "teste",
                Perfil = "Admin"
            };
            //Asserts
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("teste@teste.com", adm.Email);
            Assert.AreEqual("teste", adm.Senha);
            Assert.AreEqual("Admin", adm.Perfil);
        }
    }
}