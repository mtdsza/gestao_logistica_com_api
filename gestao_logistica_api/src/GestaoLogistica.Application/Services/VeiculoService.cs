using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Veiculo;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;

namespace GestaoLogistica.Application.Services;

// serviço de aplicação encarregado da gestão da frota de veículos
public class VeiculoService : IVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly ITransportadoraRepository _transportadoraRepository;

    public VeiculoService(
        IVeiculoRepository veiculoRepository,
        ITransportadoraRepository transportadoraRepository)
    {
        _veiculoRepository = veiculoRepository;
        _transportadoraRepository = transportadoraRepository;
    }

    // lista todos os veículos cadastrados
    public async Task<ResultWrapper<List<VeiculoDto>>> GetAllAsync()
    {
        var list = await _veiculoRepository.GetAllAsync();
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<VeiculoDto>>.Ok(dtos);
    }

    // lista apenas veículos livres com status 'disponivel'
    public async Task<ResultWrapper<List<VeiculoDto>>> GetAvailableAsync()
    {
        var list = await _veiculoRepository.GetAvailableAsync();
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<VeiculoDto>>.Ok(dtos);
    }

    // busca veículo por id
    public async Task<ResultWrapper<VeiculoDto>> GetByIdAsync(int id)
    {
        var v = await _veiculoRepository.GetByIdAsync(id);
        if (v == null)
        {
            return ResultWrapper<VeiculoDto>.Fail("Veículo não encontrado.", $"Nenhum veículo com ID {id}.");
        }

        return ResultWrapper<VeiculoDto>.Ok(MapToDto(v));
    }

    // cadastra um novo veículo com validação de placa única e capacidade
    public async Task<ResultWrapper<VeiculoDto>> CreateAsync(CreateVeiculoDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Placa) || string.IsNullOrWhiteSpace(request.Tipo))
        {
            return ResultWrapper<VeiculoDto>.Fail("Dados inválidos.", "Placa e Tipo são obrigatórios.");
        }

        // verifica duplicidade de placa
        var existente = await _veiculoRepository.GetByPlacaAsync(request.Placa.Trim());
        if (existente != null)
        {
            return ResultWrapper<VeiculoDto>.Fail("Placa já cadastrada.", "Já existe um veículo com esta placa.");
        }

        if (request.IdTransportadora.HasValue)
        {
            var transp = await _transportadoraRepository.GetByIdAsync(request.IdTransportadora.Value);
            if (transp == null)
            {
                return ResultWrapper<VeiculoDto>.Fail("Transportadora inválida.", "A transportadora informada não existe.");
            }
        }

        var entity = new Veiculo
        {
            Placa = request.Placa.Trim().ToUpperInvariant(),
            Tipo = request.Tipo.Trim(),
            CapacidadeCarga = request.CapacidadeCarga,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Disponivel" : request.Status.Trim(),
            IdTransportadora = request.IdTransportadora
        };

        await _veiculoRepository.AddAsync(entity);
        return ResultWrapper<VeiculoDto>.Ok(MapToDto(entity), "Veículo cadastrado com sucesso.");
    }

    // atualiza dados cadastrais ou status operacional do veículo
    public async Task<ResultWrapper<VeiculoDto>> UpdateAsync(int id, UpdateVeiculoDto request)
    {
        var entity = await _veiculoRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<VeiculoDto>.Fail("Veículo não encontrado.", $"Nenhum veículo com ID {id}.");
        }

        if (request.IdTransportadora.HasValue)
        {
            var transp = await _transportadoraRepository.GetByIdAsync(request.IdTransportadora.Value);
            if (transp == null)
            {
                return ResultWrapper<VeiculoDto>.Fail("Transportadora inválida.", "A transportadora informada não existe.");
            }
        }

        entity.Placa = request.Placa.Trim().ToUpperInvariant();
        entity.Tipo = request.Tipo.Trim();
        entity.CapacidadeCarga = request.CapacidadeCarga;
        entity.Status = request.Status.Trim();
        entity.IdTransportadora = request.IdTransportadora;

        await _veiculoRepository.UpdateAsync(entity);
        return ResultWrapper<VeiculoDto>.Ok(MapToDto(entity), "Veículo atualizado com sucesso.");
    }

    // remove o veículo do sistema
    public async Task<ResultWrapper<bool>> DeleteAsync(int id)
    {
        var entity = await _veiculoRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<bool>.Fail("Veículo não encontrado.", $"Nenhum veículo com ID {id}.");
        }

        await _veiculoRepository.DeleteAsync(id);
        return ResultWrapper<bool>.Ok(true, "Veículo excluído com sucesso.");
    }

    // método auxiliar de conversão para dto
    private static VeiculoDto MapToDto(Veiculo v)
    {
        return new VeiculoDto
        {
            Id = v.Id,
            Placa = v.Placa,
            Tipo = v.Tipo,
            CapacidadeCarga = v.CapacidadeCarga,
            Status = v.Status,
            IdTransportadora = v.IdTransportadora,
            NomeTransportadora = v.Transportadora?.NomeFantasia
        };
    }
}
