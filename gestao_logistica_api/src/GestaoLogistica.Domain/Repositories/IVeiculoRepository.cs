using GestaoLogistica.Domain.Entities;

namespace GestaoLogistica.Domain.Repositories;

// contrato de persistência para gerenciamento da frota de veículos
public interface IVeiculoRepository
{
    // lista todos os veículos cadastrados
    Task<List<Veiculo>> GetAllAsync();

    // busca veículo pelo identificador numérico
    Task<Veiculo?> GetByIdAsync(int id);

    // busca veículo pela placa
    Task<Veiculo?> GetByPlacaAsync(string placa);

    // lista apenas veículos desimpedidos com status 'disponivel'
    Task<List<Veiculo>> GetAvailableAsync();

    // cadastra um novo veículo
    Task AddAsync(Veiculo veiculo);

    // atualiza dados ou status operacional do veículo
    Task UpdateAsync(Veiculo veiculo);

    // remove um veículo da frota
    Task DeleteAsync(int id);
}
