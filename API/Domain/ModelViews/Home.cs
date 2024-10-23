using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ModelViews
{
    public struct Home
    {
        public string Mensagem { get => "Bem Vindo";}
        public string Documentacao { get => "/swagger"; } 
    }
}