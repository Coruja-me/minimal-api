using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using minimal_api.Domain.Entities;
using minimal_api.Infrastructure.DB;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Domain.Services
{
    public class VeiculoService(DBContext db) : iVeiculoService
    {
        private readonly DBContext _db = db;

        public Veiculo Atualizar(Veiculo veiculo)
        {
            throw new NotImplementedException();
        }

        public void DeletarId(int id)
        {
            var Veic = _db.Veiculos.Find(id);
                
            _db.Veiculos.Remove(Veic);
            _db.SaveChanges();
        }

        public void DeletarVeiculo(Veiculo veiculo)
        {
            _db.Veiculos.Remove(veiculo);
            _db.SaveChanges();
        }

        public void Incluir(Veiculo veiculo)
        {
            _db.Veiculos.Add(veiculo);
            _db.SaveChanges();
        }

        public Veiculo? LerId(int id)
        {
            return _db.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        }

        public List<Veiculo> Veiculos(int pagina = 1, string? nome = null, string? marca = null)
        {
            var query = _db.Veiculos.AsQueryable();
            if(!string.IsNullOrEmpty(nome))
                query = query.Where(v => v.Nome.ToLower().Contains(nome));
            
            int ItensPag = 15;

            query = query.Skip((pagina--) * ItensPag).Take(ItensPag);

            return query.ToList();
        }

        void iVeiculoService.Atualizar(Veiculo veiculo)
        {

        }

        void iVeiculoService.Incluir(Veiculo veiculo)
        {
            throw new NotImplementedException();
        }
    }
}