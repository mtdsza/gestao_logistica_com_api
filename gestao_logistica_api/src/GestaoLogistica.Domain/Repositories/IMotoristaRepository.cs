using GestaoLogistica.Domain.Entities;

namespace GestaoLogistica.Domain.Repositories;

// contrato de persistência para gerenciamento de motoristas
public interface IMotoristaRepository
{
    // lista todos os motoristas cadastrados
    Task<List<Motorista>> GetAllAsync();

    // busca motorista pelo identificador numérico
    Task<Motorista?> GetByIdAsync(int id);

    // busca motorista pelo número de cnh
    Task<Motorista?> GetByCnhAsync(string cnh);

    // lista apenas motoristas que estão atualmente livres ('disponivel')
    Task<List<Motorista>> GetAvailableAsync();

    // cadastra um novo motorista
    Task AddAsync(Motorista motorista);

    // atualiza dados cadastrais ou status de um motorista
    Task UpdateAsync(Motorista motorista);

    // remove o motorista do sistema
    Task DeleteAsync(int id);
}
