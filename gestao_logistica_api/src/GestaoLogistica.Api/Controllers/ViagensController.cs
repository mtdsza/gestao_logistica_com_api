using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Viagem;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.Api.Controllers;

// controlador restful principal para as operações e consultas de viagens e fretes
[Authorize]
public class ViagensController : BaseApiController
{
    private readonly IViagemService _viagemService;

    public ViagensController(IViagemService viagemService)
    {
        _viagemService = viagemService;
    }

    // lista todas as viagens cadastradas
    [HttpGet]
    public async Task<ActionResult<ResultWrapper<List<ViagemDto>>>> GetAll()
    {
        var result = await _viagemService.GetAllAsync();
        return CustomResult(result);
    }

    // consulta detalhes de uma viagem específica com itens embarcados
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResultWrapper<ViagemDto>>> GetById(int id)
    {
        var result = await _viagemService.GetByIdAsync(id);
        return CustomResult(result);
    }

    // consulta viagens ativas/em andamento de um motorista específico
    [HttpGet("motorista/{idMotorista:int}")]
    public async Task<ActionResult<ResultWrapper<List<ViagemDto>>>> GetByMotorista(int idMotorista)
    {
        var result = await _viagemService.GetByMotoristaAsync(idMotorista);
        return CustomResult(result);
    }

    // consulta o histórico de fretes de um motorista
    [HttpGet("historico/motorista/{idMotorista:int}")]
    public async Task<ActionResult<ResultWrapper<List<ViagemDto>>>> GetHistoryByMotorista(int idMotorista)
    {
        var result = await _viagemService.GetHistoryByMotoristaAsync(idMotorista);
        return CustomResult(result);
    }

    // consulta o histórico de viagens executadas por determinado veículo
    [HttpGet("historico/veiculo/{idVeiculo:int}")]
    public async Task<ActionResult<ResultWrapper<List<ViagemDto>>>> GetHistoryByVeiculo(int idVeiculo)
    {
        var result = await _viagemService.GetHistoryByVeiculoAsync(idVeiculo);
        return CustomResult(result);
    }

    // consulta o histórico de viagens realizadas por uma transportadora
    [HttpGet("historico/transportadora/{idTransportadora:int}")]
    public async Task<ActionResult<ResultWrapper<List<ViagemDto>>>> GetHistoryByTransportadora(int idTransportadora)
    {
        var result = await _viagemService.GetHistoryByTransportadoraAsync(idTransportadora);
        return CustomResult(result);
    }

    // retorna indicadores de faturamento, eficiência e viagens em trânsito para o dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<ResultWrapper<DashboardMetricasDto>>> GetDashboard()
    {
        var result = await _viagemService.GetDashboardMetricasAsync();
        return CustomResult(result);
    }

    // calcula a quilometragem pela fórmula de haversine e sugere o valor de frete
    [HttpPost("calcular-frete")]
    public async Task<ActionResult<ResultWrapper<CalcularFreteResponseDto>>> CalcularFrete([FromBody] CalcularFreteRequestDto request)
    {
        var result = await _viagemService.CalcularFreteAsync(request);
        return CustomResult(result);
    }

    // cria uma nova viagem, validando peso máximo e alocando recursos em transação atômica (restrito a admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<ViagemDto>>> Create([FromBody] CreateViagemDto request)
    {
        var result = await _viagemService.CreateAsync(request);
        return CustomResult(result);
    }

    // atualiza o status de uma viagem (ao concluir, libera o motorista e o veículo)
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ResultWrapper<bool>>> UpdateStatus(int id, [FromBody] UpdateViagemStatusDto request)
    {
        var result = await _viagemService.UpdateStatusAsync(id, request.Status);
        return CustomResult(result);
    }

    // atualiza a marcação financeira de faturamento (restrito a administradores)
    [HttpPatch("{id:int}/faturado")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<bool>>> UpdateFaturado(int id, [FromBody] UpdateViagemFaturadoDto request)
    {
        var result = await _viagemService.UpdateFaturadoAsync(id, request.Faturado);
        return CustomResult(result);
    }
}
