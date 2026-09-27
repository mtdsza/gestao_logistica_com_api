using GestaoLogistica.Application.Common.Interfaces;
using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Viagem;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Domain.Services;

namespace GestaoLogistica.Application.Services;

// serviço de aplicação central do sistema: orquestra as regras de negócio operacionais de fretes e viagens
public class ViagemService : IViagemService
{
    private readonly IViagemRepository _viagemRepository;
    private readonly IMotoristaRepository _motoristaRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly ITransportadoraRepository _transportadoraRepository;
    private readonly IItemInventarioRepository _itemRepository;
    private readonly IAppCacheService _cacheService;

    public ViagemService(
        IViagemRepository viagemRepository,
        IMotoristaRepository motoristaRepository,
        IVeiculoRepository veiculoRepository,
        ITransportadoraRepository transportadoraRepository,
        IItemInventarioRepository itemRepository,
        IAppCacheService cacheService)
    {
        _viagemRepository = viagemRepository;
        _motoristaRepository = motoristaRepository;
        _veiculoRepository = veiculoRepository;
        _transportadoraRepository = transportadoraRepository;
        _itemRepository = itemRepository;
        _cacheService = cacheService;
    }

    // lista todas as viagens com motorista, veículo e itens relacionados
    public async Task<ResultWrapper<List<ViagemDto>>> GetAllAsync()
    {
        var list = await _viagemRepository.GetAllAsync();
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<ViagemDto>>.Ok(dtos);
    }

    // busca uma viagem por id
    public async Task<ResultWrapper<ViagemDto>> GetByIdAsync(int id)
    {
        var v = await _viagemRepository.GetByIdAsync(id);
        if (v == null)
        {
            return ResultWrapper<ViagemDto>.Fail("Viagem não encontrada.", $"Nenhuma viagem com ID {id}.");
        }

        return ResultWrapper<ViagemDto>.Ok(MapToDto(v));
    }

    // retorna viagens ativas de um determinado motorista
    public async Task<ResultWrapper<List<ViagemDto>>> GetByMotoristaAsync(int idMotorista)
    {
        var list = await _viagemRepository.GetByMotoristaAsync(idMotorista);
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<ViagemDto>>.Ok(dtos);
    }

    // consulta o histórico completo de fretes do motorista
    public async Task<ResultWrapper<List<ViagemDto>>> GetHistoryByMotoristaAsync(int idMotorista)
    {
        var list = await _viagemRepository.GetHistoryByMotoristaAsync(idMotorista);
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<ViagemDto>>.Ok(dtos);
    }

    // consulta o histórico de viagens executadas por um veículo
    public async Task<ResultWrapper<List<ViagemDto>>> GetHistoryByVeiculoAsync(int idVeiculo)
    {
        var list = await _viagemRepository.GetHistoryByVeiculoAsync(idVeiculo);
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<ViagemDto>>.Ok(dtos);
    }

    // consulta o histórico de viagens realizadas com uma transportadora
    public async Task<ResultWrapper<List<ViagemDto>>> GetHistoryByTransportadoraAsync(int idTransportadora)
    {
        var list = await _viagemRepository.GetHistoryByTransportadoraAsync(idTransportadora);
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<ViagemDto>>.Ok(dtos);
    }

    // consolida os indicadores operacionais exibidos no painel gerencial do sistema
    public async Task<ResultWrapper<DashboardMetricasDto>> GetDashboardMetricasAsync()
    {
        var faturamento = await _viagemRepository.GetFaturamentoTotalAsync();
        var eficiencia = await _viagemRepository.GetEficienciaNoPrazoAsync();
        var emTransito = await _viagemRepository.GetViagensEmTransitoAsync();
        var todas = await _viagemRepository.GetAllAsync();

        var metricas = new DashboardMetricasDto
        {
            FaturamentoTotal = faturamento,
            EficienciaNoPrazo = Math.Round(eficiencia, 2),
            ViagensEmTransito = emTransito,
            TotalViagens = todas.Count
        };

        return ResultWrapper<DashboardMetricasDto>.Ok(metricas);
    }

    // executa o cálculo prévio do frete utilizando coordenadas geodésicas e tarifas por peso
    public async Task<ResultWrapper<CalcularFreteResponseDto>> CalcularFreteAsync(CalcularFreteRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Origem) || string.IsNullOrWhiteSpace(request.Destino))
        {
            return ResultWrapper<CalcularFreteResponseDto>.Fail("Dados incompletos.", "Origem e destino são obrigatórios.");
        }

        // calcula a distância em km pela fórmula de haversine
        double distancia = DistanciaService.CalcularDistanciaEmKm(request.Origem.Trim(), request.Destino.Trim());

        decimal custoKm = DistanciaService.CustoPadraoPorKm;
        decimal custoPeso = DistanciaService.CustoPadraoPorKg;

        // se uma transportadora foi selecionada, aplica suas tarifas negociadas em contrato
        if (request.IdTransportadora.HasValue)
        {
            var transp = await _transportadoraRepository.GetByIdAsync(request.IdTransportadora.Value);
            if (transp != null)
            {
                custoKm = transp.CustoKm;
                custoPeso = transp.CustoPeso;
            }
        }

        // calcula o custo total sugerido do frete
        decimal custoTotal = DistanciaService.CalcularFreteSugerido(
            request.Origem.Trim(),
            request.Destino.Trim(),
            request.PesoTotal,
            custoKm,
            custoPeso);

        var response = new CalcularFreteResponseDto
        {
            DistanciaKm = Math.Round(distancia, 2),
            CustoSugerido = custoTotal,
            CustoKmUtilizado = custoKm,
            CustoPesoUtilizado = custoPeso
        };

        return ResultWrapper<CalcularFreteResponseDto>.Ok(response);
    }

    // cria a viagem validando regras de capacidade de carga, disponibilidade de motorista e veículo,
    // e executa a reserva atômica de estoque em bloco transacional
    public async Task<ResultWrapper<ViagemDto>> CreateAsync(CreateViagemDto request)
    {
        // 1. validações de preenchimento obrigatório
        if (string.IsNullOrWhiteSpace(request.Origem) || string.IsNullOrWhiteSpace(request.Destino))
        {
            return ResultWrapper<ViagemDto>.Fail("Dados incompletos.", "Origem e Destino são obrigatórios.");
        }

        // 2. valida existência e disponibilidade do motorista
        var motorista = await _motoristaRepository.GetByIdAsync(request.IdMotorista);
        if (motorista == null)
        {
            return ResultWrapper<ViagemDto>.Fail("Motorista inválido.", $"Motorista ID {request.IdMotorista} não encontrado.");
        }
        if (!string.Equals(motorista.Status, "Disponivel", StringComparison.OrdinalIgnoreCase))
        {
            return ResultWrapper<ViagemDto>.Fail("Motorista indisponível.", $"O motorista '{motorista.Nome}' está com status '{motorista.Status}'.");
        }

        // 3. valida existência e disponibilidade do veículo
        var veiculo = await _veiculoRepository.GetByIdAsync(request.IdVeiculo);
        if (veiculo == null)
        {
            return ResultWrapper<ViagemDto>.Fail("Veículo inválido.", $"Veículo ID {request.IdVeiculo} não encontrado.");
        }
        if (!string.Equals(veiculo.Status, "Disponivel", StringComparison.OrdinalIgnoreCase))
        {
            return ResultWrapper<ViagemDto>.Fail("Veículo indisponível.", $"O veículo '{veiculo.Placa}' está com status '{veiculo.Status}'.");
        }

        // 4. valida se o peso total da carga não ultrapassa a capacidade máxima suportada pelo veículo
        if (request.PesoTotal > veiculo.CapacidadeCarga)
        {
            return ResultWrapper<ViagemDto>.Fail("Excesso de carga.",
                $"O peso total ({request.PesoTotal} kg) excede a capacidade máxima do veículo ({veiculo.CapacidadeCarga} kg).");
        }

        // 5. valida os itens e verifica se há quantidade suficiente em estoque para cada um
        var itensTuplas = new List<(int ItemId, int Quantidade)>();
        foreach (var itemReq in request.Itens)
        {
            if (itemReq.QuantidadeTransportada <= 0)
            {
                return ResultWrapper<ViagemDto>.Fail("Quantidade inválida.", "A quantidade transportada de cada item deve ser maior que zero.");
            }

            var itemEstoque = await _itemRepository.GetByIdAsync(itemReq.IdItem);
            if (itemEstoque == null)
            {
                return ResultWrapper<ViagemDto>.Fail("Item não encontrado.", $"Item de estoque ID {itemReq.IdItem} não existe.");
            }

            // bloqueia a operação se a quantidade em estoque for insuficiente
            if (itemEstoque.Quantidade < itemReq.QuantidadeTransportada)
            {
                return ResultWrapper<ViagemDto>.Fail("Estoque insuficiente.",
                    $"Item '{itemEstoque.Nome}' possui apenas {itemEstoque.Quantidade} em estoque (solicitado: {itemReq.QuantidadeTransportada}).");
            }

            itensTuplas.Add((itemReq.IdItem, itemReq.QuantidadeTransportada));
        }

        // 6. instancia a entidade viagem com os dados validados
        var viagem = new Viagem
        {
            IdMotorista = request.IdMotorista,
            IdVeiculo = request.IdVeiculo,
            IdTransportadora = request.IdTransportadora,
            Origem = request.Origem.Trim(),
            Destino = request.Destino.Trim(),
            CustoTransporte = request.CustoTransporte,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Pendente" : request.Status.Trim(),
            PesoTotal = request.PesoTotal,
            DataPlanejada = string.IsNullOrWhiteSpace(request.DataPlanejada)
                ? DateTime.UtcNow.ToString("yyyy-MM-dd")
                : request.DataPlanejada.Trim(),
            Faturado = request.Faturado
        };

        // persiste a viagem em transação atômica (salva viagem, insere itens, debita estoque e bloqueia veículo/motorista)
        await _viagemRepository.AddAsync(viagem, itensTuplas);

        // como houve baixa de estoque, invalida reativamente o cache da listagem de itens
        if (itensTuplas.Count > 0)
        {
            await _cacheService.RemoveAsync("itens_inventario_all");
        }

        var criado = await _viagemRepository.GetByIdAsync(viagem.Id);
        return ResultWrapper<ViagemDto>.Ok(MapToDto(criado ?? viagem), "Viagem criada e estoque reservado com sucesso.");
    }

    // atualiza o status da viagem; ao concluir ('concluido'), o repositório libera o veículo e o motorista
    public async Task<ResultWrapper<bool>> UpdateStatusAsync(int id, string novoStatus)
    {
        var viagem = await _viagemRepository.GetByIdAsync(id);
        if (viagem == null)
        {
            return ResultWrapper<bool>.Fail("Viagem não encontrada.", $"Nenhuma viagem com ID {id}.");
        }

        await _viagemRepository.UpdateStatusAsync(id, novoStatus.Trim());
        return ResultWrapper<bool>.Ok(true, $"Status da viagem alterado para '{novoStatus}'.");
    }

    // atualiza o indicador financeiro de faturamento
    public async Task<ResultWrapper<bool>> UpdateFaturadoAsync(int id, bool faturado)
    {
        var viagem = await _viagemRepository.GetByIdAsync(id);
        if (viagem == null)
        {
            return ResultWrapper<bool>.Fail("Viagem não encontrada.", $"Nenhuma viagem com ID {id}.");
        }

        await _viagemRepository.UpdateFaturadoAsync(id, faturado);
        return ResultWrapper<bool>.Ok(true, "Status de faturamento atualizado com sucesso.");
    }

    // conversão da entidade de viagem para dto com seus relacionamentos
    private static ViagemDto MapToDto(Viagem v)
    {
        return new ViagemDto
        {
            Id = v.Id,
            IdMotorista = v.IdMotorista,
            NomeMotorista = v.Motorista?.Nome,
            IdVeiculo = v.IdVeiculo,
            PlacaVeiculo = v.Veiculo?.Placa,
            IdTransportadora = v.IdTransportadora,
            NomeTransportadora = v.Transportadora?.NomeFantasia,
            Origem = v.Origem,
            Destino = v.Destino,
            CustoTransporte = v.CustoTransporte,
            Status = v.Status,
            PesoTotal = v.PesoTotal,
            DataPlanejada = v.DataPlanejada,
            Faturado = v.Faturado,
            Itens = v.Itens?.Select(i => new ViagemItemDto
            {
                IdItem = i.IdItem,
                NomeItem = i.Item?.Nome,
                TipoItem = i.Item?.Tipo,
                PesoUnitario = i.Item?.PesoUnitario ?? 0m,
                QuantidadeTransportada = i.QuantidadeTransportada
            }).ToList() ?? new List<ViagemItemDto>()
        };
    }
}
