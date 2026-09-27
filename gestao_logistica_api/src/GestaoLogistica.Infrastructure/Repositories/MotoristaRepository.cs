using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositories;

// implementação do repositório de motoristas
public class MotoristaRepository : IMotoristaRepository
{
    private readonly AppDbContext _context;

    public MotoristaRepository(AppDbContext context)
    {
        _context = context;
    }

    // lista todos os motoristas trazendo os dados da transportadora associada
    public async Task<List<Motorista>> GetAllAsync()
    {
        return await _context.Motoristas
            .Include(m => m.Transportadora)
            .OrderBy(m => m.Nome)
            .ToListAsync();
    }

    // lista apenas os motoristas que estão com status 'disponivel'
    public async Task<List<Motorista>> GetAvailableAsync()
    {
        return await _context.Motoristas
            .Include(m => m.Transportadora)
            .Where(m => m.Status == "Disponivel")
            .OrderBy(m => m.Nome)
            .ToListAsync();
    }

    // busca motorista por id
    public async Task<Motorista?> GetByIdAsync(int id)
    {
        return await _context.Motoristas
            .Include(m => m.Transportadora)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    // busca motorista pela cnh única
    public async Task<Motorista?> GetByCnhAsync(string cnh)
    {
        return await _context.Motoristas
            .Include(m => m.Transportadora)
            .FirstOrDefaultAsync(m => m.Cnh == cnh);
    }

    // insere novo motorista
    public async Task AddAsync(Motorista motorista)
    {
        await _context.Motoristas.AddAsync(motorista);
        await _context.SaveChangesAsync();
    }

    // atualiza dados do motorista
    public async Task UpdateAsync(Motorista motorista)
    {
        _context.Motoristas.Update(motorista);
        await _context.SaveChangesAsync();
    }

    // remove o motorista do banco
    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Motoristas.FindAsync(id);
        if (entity != null)
        {
            _context.Motoristas.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
