using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entity;

namespace minimal_api.Domain.Interfaces
{
    public interface IAdminServicos
    {
        Admin Login(LoginDTO loginDTO);
        Admin Incluir(Admin admin);
        Admin BuscarPorId(int Id);
        List<Admin> Todos(int? pagina);
    }
}