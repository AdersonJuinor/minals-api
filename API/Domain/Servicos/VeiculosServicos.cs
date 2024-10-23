using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entity;
using minimal_api.Domain.Interfaces;
using minimal_api.Infrastructure.Db;

namespace minimal_api.Domain.Servicos
{
    public class VeiculosServicos : IVeiculosServicos
    {
        private readonly Contexto _contexto;
        public VeiculosServicos(Contexto contexto)
        {
            _contexto = contexto;
        }

        public void Apagar(Veiculos veiculos)
        {
            _contexto.veiculos.Remove(veiculos);
            _contexto.SaveChanges();
        }

        public void Atualizar(Veiculos veiculos)
        {
            _contexto.veiculos.Update(veiculos);
            _contexto.SaveChanges();
        }

        public Veiculos BuscarPorId(int Id)
        {
            return _contexto.veiculos.Where(v => v.Id == Id).FirstOrDefault();
        }

        public void Incluir(Veiculos veiculos)
        {
            _contexto.veiculos.Add(veiculos);
            _contexto.SaveChanges();
        }

        public List<Veiculos> Todos(int? pagina = 1, string nome = null, string marca = null)
        {
            var query = _contexto.veiculos.AsQueryable();
            if(!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%") );
            }

            int itensPaginas = 10;

            if(pagina != null)
            {
            query = query.Skip(((int)pagina - 1) * itensPaginas).Take(itensPaginas);
            }
            return query.ToList();
        }
    }
}