using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface iAdminService
    {
        Admin? Login(LoginDTO log);
        Admin Incluir(Admin admin);
        Admin? LerId(int id);
        List<Admin> Admins(int? pagina);
    }
}