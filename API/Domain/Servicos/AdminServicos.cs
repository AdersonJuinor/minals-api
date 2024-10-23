using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entity;
using minimal_api.Domain.Interfaces;
using minimal_api.Infrastructure.Db;

namespace minimal_api.Domain.Servicos
{
    public class AdminServicos : IAdminServicos
    {
        private readonly Contexto _contexto;
        public AdminServicos(Contexto contexto)
        {
            _contexto = contexto;
        }

        public Admin BuscarPorId(int Id)
        {
            return _contexto.admins.Where(v => v.Id == Id).FirstOrDefault();
        }

        public Admin Incluir(Admin admin)
        {
            _contexto.admins.Add(admin);
            _contexto.SaveChanges();

            return admin;
        }

        public Admin Login(LoginDTO loginDTO)
        {
            var adm = _contexto.admins.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }

        public List<Admin> Todos(int? pagina)
        {
            var query = _contexto.admins.AsQueryable();
            int itensPaginas = 10;

            if(pagina != null)
            {
            query = query.Skip(((int)pagina - 1) * itensPaginas).Take(itensPaginas);
            }
            return query.ToList();
        }
    }
}