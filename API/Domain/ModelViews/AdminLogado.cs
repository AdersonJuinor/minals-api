using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ModelViews
{
    public record AdminLogado
    {
        public string Email { get; set; }
        public string Perfil { get; set; }
        public string Token { get; set; }
    }
}