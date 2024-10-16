using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Infrastructure.DB;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Domain.Services
{
    public class AdminService(DBContext db) : iAdminService
    {
        private readonly DBContext _db = db;

        public Admin? Login(LoginDTO log)
        { 
            var Adm = _db.Admins.Where(a => a.Email == log.Email && a.Senha == log.Senha).FirstOrDefault();
            return Adm;
        }
    }
}