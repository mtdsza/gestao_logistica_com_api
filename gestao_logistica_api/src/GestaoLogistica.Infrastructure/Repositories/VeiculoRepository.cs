using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositories;

// implementação do repositório de veículos com entity framework core
public class VeiculoRepository : IVeiculoRepository
{
    private readonly AppDbContext _context;

    public VeiculoRepository(AppDbContext context)
    {
        _context = context;
    }

    // lista todos os veículos cadastrados incluindo os dados da transportadora parceira
    public async Task<List<Veiculo>> GetAllAsync()
    {
        return await _context.Veiculos
            .Include(v => v.Transportadora)
            .OrderBy(v => v.Placa)
            .ToListAsync();
    }

    // lista apenas os veículos que estão atualmente livres ('disponivel')
    public async Task<List<Veiculo>> GetAvailableAsync()
    {
        return await _context.Veiculos
            .Include(v => v.Transportadora)
            .Where(v => v.Status == "Disponivel")
            .OrderBy(v => v.Placa)
            .ToListAsync();
    }

    // busca veículo por id
    public async Task<Veiculo?> GetByIdAsync(int id)
    {
        return await _context.Veiculos
            .Include(v => v.Transportadora)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    // busca veículo pela placa única
    public async Task<Veiculo?> GetByPlacaAsync(string placa)
    {
        return await _context.Veiculos
            .Include(v => v.Transportadora)
            .FirstOrDefaultAsync(v => v.Placa == placa);
    }

    // persiste um novo veículo
    public async Task AddAsync(Veiculo veiculo)
    {
        await _context.Veiculos.AddAsync(veiculo);
        await _context.SaveChangesAsync();
    }

    // atualiza dados do veículo
    public async Task UpdateAsync(Veiculo veiculo)
    {
        _context.Veiculos.Update(veiculo);
        await _context.SaveChangesAsync();
    }

    // remove o veículo da frota
    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Veiculos.FindAsync(id);
        if (entity != null)
        {
            _context.Veiculos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
