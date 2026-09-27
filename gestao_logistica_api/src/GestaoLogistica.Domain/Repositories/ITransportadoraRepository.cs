using GestaoLogistica.Domain.Entities;

namespace GestaoLogistica.Domain.Repositories;

// contrato de persistência para gerenciamento de transportadoras
public interface ITransportadoraRepository
{
    // retorna todas as transportadoras cadastradas
    Task<List<Transportadora>> GetAllAsync();

    // busca transportadora pelo identificador numérico
    Task<Transportadora?> GetByIdAsync(int id);

    // busca transportadora pelo número de cnpj
    Task<Transportadora?> GetByCnpjAsync(string cnpj);

    // cadastra uma nova transportadora
    Task AddAsync(Transportadora transportadora);

    // atualiza dados ou tarifas de uma transportadora
    Task UpdateAsync(Transportadora transportadora);

    // remove uma transportadora pelo id
    Task DeleteAsync(int id);
}
