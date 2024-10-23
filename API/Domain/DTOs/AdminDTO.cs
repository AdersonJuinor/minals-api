using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Enuns;

namespace minimal_api.Domain.DTOs
{
    public struct AdminDTO
    {
        public string Email { get; set; }
        public string Senha { get; set;}
        public Perfil Perfil { get; set; }
    }
}