using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Enuns;

namespace minimal_api.Domain.DTO
{
    public record AdminDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public Perfil Perfil { get; set; } = default!;
    }
}