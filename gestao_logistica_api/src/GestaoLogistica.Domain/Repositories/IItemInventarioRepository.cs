using GestaoLogistica.Domain.Entities;

namespace GestaoLogistica.Domain.Repositories;

// contrato de persistência para gerenciamento do estoque e inventário
public interface IItemInventarioRepository
{
    // retorna todos os itens estocados
    Task<List<ItemInventario>> GetAllAsync();

    // busca item pelo identificador numérico
    Task<ItemInventario?> GetByIdAsync(int id);

    // insere um novo item no estoque
    Task AddAsync(ItemInventario item);

    // atualiza as informações do item
    Task UpdateAsync(ItemInventario item);

    // exclui um item do inventário
    Task DeleteAsync(int id);

    // ajusta pontualmente a quantidade em estoque de um item
    Task UpdateStockAsync(int id, int novaQuantidade);
}
