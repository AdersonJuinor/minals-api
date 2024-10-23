using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Entity;

namespace minimal_api.Domain.Interfaces
{
    public interface IVeiculosServicos
    {
        List<Veiculos> Todos(int? pagina = 1, string nome = null, string marca = null);

        Veiculos BuscarPorId(int Id);

        void Incluir(Veiculos veiculos);

        void Atualizar(Veiculos veiculos);

        void Apagar(Veiculos veiculos);
    }
}