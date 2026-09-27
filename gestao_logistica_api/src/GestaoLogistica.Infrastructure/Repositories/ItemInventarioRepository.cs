using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositories;

// implementação do repositório de inventário e estoque
public class ItemInventarioRepository : IItemInventarioRepository
{
    private readonly AppDbContext _context;

    public ItemInventarioRepository(AppDbContext context)
    {
        _context = context;
    }

    // lista todos os itens cadastrados em ordem alfabética
    public async Task<List<ItemInventario>> GetAllAsync()
    {
        return await _context.ItensInventario
            .OrderBy(i => i.Nome)
            .ToListAsync();
    }

    // busca um item pelo id
    public async Task<ItemInventario?> GetByIdAsync(int id)
    {
        return await _context.ItensInventario.FindAsync(id);
    }

    // persiste novo produto no estoque
    public async Task AddAsync(ItemInventario item)
    {
        await _context.ItensInventario.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    // atualiza os dados do produto
    public async Task UpdateAsync(ItemInventario item)
    {
        _context.ItensInventario.Update(item);
        await _context.SaveChangesAsync();
    }

    // remove o item do estoque
    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ItensInventario.FindAsync(id);
        if (entity != null)
        {
            _context.ItensInventario.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    // altera a quantidade em estoque de um produto de forma atômica
    public async Task UpdateStockAsync(int id, int novaQuantidade)
    {
        var item = await _context.ItensInventario.FindAsync(id);
        if (item != null)
        {
            item.Quantidade = novaQuantidade;
            await _context.SaveChangesAsync();
        }
    }
}
