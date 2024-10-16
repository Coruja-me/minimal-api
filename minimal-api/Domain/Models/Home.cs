using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.Models
{
    public struct Home
    {
        public readonly string Msg{ get => "Bem vindo a API!";}
        public string Doc { get => "/swagger"; }
    }
}