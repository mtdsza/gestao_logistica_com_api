using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositories;

// implementação do repositório de transportadoras com entity framework core
public class TransportadoraRepository : ITransportadoraRepository
{
    private readonly AppDbContext _context;

    public TransportadoraRepository(AppDbContext context)
    {
        _context = context;
    }

    // lista todas as transportadoras em ordem alfabética
    public async Task<List<Transportadora>> GetAllAsync()
    {
        return await _context.Transportadoras
            .OrderBy(t => t.NomeFantasia)
            .ToListAsync();
    }

    // busca transportadora pelo id
    public async Task<Transportadora?> GetByIdAsync(int id)
    {
        return await _context.Transportadoras.FindAsync(id);
    }

    // busca transportadora pelo cnpj
    public async Task<Transportadora?> GetByCnpjAsync(string cnpj)
    {
        return await _context.Transportadoras
            .FirstOrDefaultAsync(t => t.Cnpj == cnpj);
    }

    // persiste uma nova transportadora
    public async Task AddAsync(Transportadora transportadora)
    {
        await _context.Transportadoras.AddAsync(transportadora);
        await _context.SaveChangesAsync();
    }

    // atualiza transportadora existente
    public async Task UpdateAsync(Transportadora transportadora)
    {
        _context.Transportadoras.Update(transportadora);
        await _context.SaveChangesAsync();
    }

    // remove transportadora do banco
    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Transportadoras.FindAsync(id);
        if (entity != null)
        {
            _context.Transportadoras.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
