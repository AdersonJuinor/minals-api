using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.DTOs
{
    public record VeiculosDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Marca { get; set; }
        public int Ano { get; set; }
    }

}