using GestaoLogistica.Domain.Entities;

namespace GestaoLogistica.Domain.Repositories;

// contrato de persistência para as operações centrais de viagens e fretes
public interface IViagemRepository
{
    // retorna todas as viagens cadastradas com seus dados relacionados
    Task<List<Viagem>> GetAllAsync();

    // busca uma viagem pelo id com motorista, veículo e itens transportados
    Task<Viagem?> GetByIdAsync(int id);

    // lista as viagens ativas (não concluídas) de um motorista específico
    Task<List<Viagem>> GetByMotoristaAsync(int idMotorista);

    // histórico completo de viagens de um motorista (ordenado das mais recentes para as antigas)
    Task<List<Viagem>> GetHistoryByMotoristaAsync(int idMotorista);

    // histórico de viagens realizadas por determinado veículo
    Task<List<Viagem>> GetHistoryByVeiculoAsync(int idVeiculo);

    // histórico de viagens executadas por uma transportadora
    Task<List<Viagem>> GetHistoryByTransportadoraAsync(int idTransportadora);

    // calcula o faturamento total somando o custo das viagens com status 'concluido'
    Task<decimal> GetFaturamentoTotalAsync();

    // calcula a taxa percentual de viagens concluídas em relação ao total
    Task<decimal> GetEficienciaNoPrazoAsync();

    // conta quantas viagens estão atualmente em andamento ('em transito' ou 'atrasado')
    Task<int> GetViagensEmTransitoAsync();

    // persiste uma nova viagem executando em transação a baixa de estoque e o bloqueio de motorista/veículo
    Task AddAsync(Viagem viagem, List<(int ItemId, int Quantidade)> itens);

    // altera o status da viagem (ao concluir, libera o motorista e o veículo de volta para 'disponivel')
    Task UpdateStatusAsync(int id, string novoStatus);

    // atualiza a marcação financeira de faturamento da viagem
    Task UpdateFaturadoAsync(int id, bool faturado);
}
