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
        public Admin Incluir(Admin admin)
        {
            _db.Admins.Add(admin);
            _db.SaveChanges();

            return admin;
        }

        public List<Admin> Admins(int? pagina)
        {
            var query = _db.Admins.AsQueryable();
            
            int ItensPag = 15;

            if (pagina.HasValue){
                query = query.Skip(((int)pagina--) * ItensPag).Take(ItensPag);
            }
            return query.ToList();
        }

        public Admin? LerId(int id)
        {
            return _db.Admins.Where(a => a.Id == id).FirstOrDefault();
        }
    }
}