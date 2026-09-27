using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositories;

// implementação do repositório de viagens com entity framework core
// implementa transações acid para garantir integridade atômica na reserva de estoque e alocação de frota
public class ViagemRepository : IViagemRepository
{
    private readonly AppDbContext _context;

    public ViagemRepository(AppDbContext context)
    {
        _context = context;
    }

    // lista todas as viagens com motorista, veículo, transportadora e produtos embarcados
    public async Task<List<Viagem>> GetAllAsync()
    {
        return await _context.Viagens
            .Include(v => v.Motorista)
            .Include(v => v.Veiculo)
            .Include(v => v.Transportadora)
            .Include(v => v.Itens)
                .ThenInclude(vi => vi.Item)
            .OrderByDescending(v => v.Id)
            .ToListAsync();
    }

    // busca uma viagem detalhada pelo identificador numérico
    public async Task<Viagem?> GetByIdAsync(int id)
    {
        return await _context.Viagens
            .Include(v => v.Motorista)
            .Include(v => v.Veiculo)
            .Include(v => v.Transportadora)
            .Include(v => v.Itens)
                .ThenInclude(vi => vi.Item)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    // busca viagens ativas (pendentes ou em trânsito) de um motorista específico
    public async Task<List<Viagem>> GetByMotoristaAsync(int idMotorista)
    {
        return await _context.Viagens
            .Include(v => v.Motorista)
            .Include(v => v.Veiculo)
            .Include(v => v.Transportadora)
            .Include(v => v.Itens)
                .ThenInclude(vi => vi.Item)
            .Where(v => v.IdMotorista == idMotorista && v.Status != "Concluido")
            .OrderByDescending(v => v.Id)
            .ToListAsync();
    }

    // recupera o histórico completo de fretes do motorista ordenado do mais recente ao mais antigo
    public async Task<List<Viagem>> GetHistoryByMotoristaAsync(int idMotorista)
    {
        return await _context.Viagens
            .Include(v => v.Motorista)
            .Include(v => v.Veiculo)
            .Include(v => v.Transportadora)
            .Include(v => v.Itens)
                .ThenInclude(vi => vi.Item)
            .Where(v => v.IdMotorista == idMotorista)
            .OrderByDescending(v => v.Id)
            .ToListAsync();
    }

    // histórico de viagens realizadas pelo veículo especificado
    public async Task<List<Viagem>> GetHistoryByVeiculoAsync(int idVeiculo)
    {
        return await _context.Viagens
            .Include(v => v.Motorista)
            .Include(v => v.Veiculo)
            .Include(v => v.Transportadora)
            .Include(v => v.Itens)
                .ThenInclude(vi => vi.Item)
            .Where(v => v.IdVeiculo == idVeiculo)
            .OrderByDescending(v => v.Id)
            .ToListAsync();
    }

    // histórico de viagens atendidas pela transportadora
    public async Task<List<Viagem>> GetHistoryByTransportadoraAsync(int idTransportadora)
    {
        return await _context.Viagens
            .Include(v => v.Motorista)
            .Include(v => v.Veiculo)
            .Include(v => v.Transportadora)
            .Include(v => v.Itens)
                .ThenInclude(vi => vi.Item)
            .Where(v => v.IdTransportadora == idTransportadora)
            .OrderByDescending(v => v.Id)
            .ToListAsync();
    }

    // calcula o faturamento acumulado somando o valor de frete de viagens com status 'concluido'
    public async Task<decimal> GetFaturamentoTotalAsync()
    {
        return await _context.Viagens
            .Where(v => v.Status == "Concluido")
            .SumAsync(v => v.CustoTransporte);
    }

    // calcula o percentual de viagens concluídas com sucesso frente ao total geral
    public async Task<decimal> GetEficienciaNoPrazoAsync()
    {
        int total = await _context.Viagens.CountAsync();
        if (total == 0) return 100m;

        int concluidas = await _context.Viagens.CountAsync(v => v.Status == "Concluido");
        return Math.Round((decimal)concluidas / total * 100m, 2);
    }

    // conta o número de fretes ativos ('em transito' ou 'atrasado')
    public async Task<int> GetViagensEmTransitoAsync()
    {
        return await _context.Viagens
            .CountAsync(v => v.Status == "Em transito" || v.Status == "Atrasado");
    }

    // persiste a viagem em uma transação atômica do banco de dados (acid)
    // etapas transacionais garantidas:
    // 1. grava o registro principal da viagem e recupera o id gerado
    // 2. insere a relação na tabela de junção 'viagem_itens'
    // 3. debita a quantidade transportada do saldo disponível na tabela 'itens_inventario'
    // 4. atualiza o status do motorista e do veículo para 'em viagem'
    // caso ocorra qualquer falha, toda a transação sofre rollback, preservando a consistência dos dados
    public async Task AddAsync(Viagem viagem, List<(int ItemId, int Quantidade)> itens)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. salva a viagem principal
            await _context.Viagens.AddAsync(viagem);
            await _context.SaveChangesAsync();

            // 2. insere os itens associados e debita o estoque
            foreach (var (itemId, quantidade) in itens)
            {
                var viagemItem = new ViagemItem
                {
                    IdViagem = viagem.Id,
                    IdItem = itemId,
                    QuantidadeTransportada = quantidade
                };
                await _context.ViagemItens.AddAsync(viagemItem);

                // baixa a quantidade do inventário
                var itemEstoque = await _context.ItensInventario.FindAsync(itemId);
                if (itemEstoque != null)
                {
                    itemEstoque.Quantidade -= quantidade;
                }
            }

            // 3. altera os recursos alocados para o status 'em viagem'
            var motorista = await _context.Motoristas.FindAsync(viagem.IdMotorista);
            if (motorista != null)
            {
                motorista.Status = "Em Viagem";
            }

            var veiculo = await _context.Veiculos.FindAsync(viagem.IdVeiculo);
            if (veiculo != null)
            {
                veiculo.Status = "Em Viagem";
            }

            // persiste todas as alterações de uma única vez e confirma a transação
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            // desfaz qualquer alteração parcial em caso de erro
            await transaction.RollbackAsync();
            throw;
        }
    }

    // atualiza o status da viagem; se concluída, devolve motorista e veículo para 'disponivel'
    public async Task UpdateStatusAsync(int id, string novoStatus)
    {
        var viagem = await _context.Viagens.FindAsync(id);
        if (viagem == null) return;

        viagem.Status = novoStatus;

        // se a viagem for concluída, libera os recursos físicos para novas viagens
        if (novoStatus.Equals("Concluido", StringComparison.OrdinalIgnoreCase))
        {
            var motorista = await _context.Motoristas.FindAsync(viagem.IdMotorista);
            if (motorista != null)
            {
                motorista.Status = "Disponivel";
            }

            var veiculo = await _context.Veiculos.FindAsync(viagem.IdVeiculo);
            if (veiculo != null)
            {
                veiculo.Status = "Disponivel";
            }
        }

        await _context.SaveChangesAsync();
    }

    // marca ou desmarca a viagem como financeiramente faturada
    public async Task UpdateFaturadoAsync(int id, bool faturado)
    {
        var viagem = await _context.Viagens.FindAsync(id);
        if (viagem != null)
        {
            viagem.Faturado = faturado;
            await _context.SaveChangesAsync();
        }
    }
}
