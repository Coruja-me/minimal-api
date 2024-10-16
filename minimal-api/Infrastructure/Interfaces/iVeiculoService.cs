using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface iVeiculoService
    {
        List<Veiculo> Veiculos(int? pagina = 1, string? nome = null, string? marca = null);
        Veiculo? LerId(int id);
        void Incluir(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void Deletar(Veiculo veiculo);
    }
}