using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Motorista;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;

namespace GestaoLogistica.Application.Services;

// serviço de aplicação encarregado do ciclo de vida e das regras de motoristas
public class MotoristaService : IMotoristaService
{
    private readonly IMotoristaRepository _motoristaRepository;
    private readonly ITransportadoraRepository _transportadoraRepository;

    public MotoristaService(
        IMotoristaRepository motoristaRepository,
        ITransportadoraRepository transportadoraRepository)
    {
        _motoristaRepository = motoristaRepository;
        _transportadoraRepository = transportadoraRepository;
    }

    // lista todos os motoristas cadastrados
    public async Task<ResultWrapper<List<MotoristaDto>>> GetAllAsync()
    {
        var list = await _motoristaRepository.GetAllAsync();
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<MotoristaDto>>.Ok(dtos);
    }

    // lista apenas motoristas livres ('disponivel') para alocação em novas viagens
    public async Task<ResultWrapper<List<MotoristaDto>>> GetAvailableAsync()
    {
        var list = await _motoristaRepository.GetAvailableAsync();
        var dtos = list.Select(MapToDto).ToList();
        return ResultWrapper<List<MotoristaDto>>.Ok(dtos);
    }

    // busca motorista por id
    public async Task<ResultWrapper<MotoristaDto>> GetByIdAsync(int id)
    {
        var m = await _motoristaRepository.GetByIdAsync(id);
        if (m == null)
        {
            return ResultWrapper<MotoristaDto>.Fail("Motorista não encontrado.", $"Nenhum motorista com ID {id}.");
        }

        return ResultWrapper<MotoristaDto>.Ok(MapToDto(m));
    }

    // cadastra um novo motorista validando cnh e transportadora
    public async Task<ResultWrapper<MotoristaDto>> CreateAsync(CreateMotoristaDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Cnh))
        {
            return ResultWrapper<MotoristaDto>.Fail("Dados inválidos.", "Nome e CNH são obrigatórios.");
        }

        // verifica unicidade da cnh
        var existente = await _motoristaRepository.GetByCnhAsync(request.Cnh.Trim());
        if (existente != null)
        {
            return ResultWrapper<MotoristaDto>.Fail("CNH já cadastrada.", "Já existe um motorista com esta CNH.");
        }

        // valida se a transportadora informada existe
        if (request.IdTransportadora.HasValue)
        {
            var transp = await _transportadoraRepository.GetByIdAsync(request.IdTransportadora.Value);
            if (transp == null)
            {
                return ResultWrapper<MotoristaDto>.Fail("Transportadora inválida.", "A transportadora informada não existe.");
            }
        }

        var entity = new Motorista
        {
            Nome = request.Nome.Trim(),
            Cnh = request.Cnh.Trim(),
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Disponivel" : request.Status.Trim(),
            IdTransportadora = request.IdTransportadora
        };

        await _motoristaRepository.AddAsync(entity);
        return ResultWrapper<MotoristaDto>.Ok(MapToDto(entity), "Motorista cadastrado com sucesso.");
    }

    // edita os dados do motorista
    public async Task<ResultWrapper<MotoristaDto>> UpdateAsync(int id, UpdateMotoristaDto request)
    {
        var entity = await _motoristaRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<MotoristaDto>.Fail("Motorista não encontrado.", $"Nenhum motorista com ID {id}.");
        }

        if (request.IdTransportadora.HasValue)
        {
            var transp = await _transportadoraRepository.GetByIdAsync(request.IdTransportadora.Value);
            if (transp == null)
            {
                return ResultWrapper<MotoristaDto>.Fail("Transportadora inválida.", "A transportadora informada não existe.");
            }
        }

        entity.Nome = request.Nome.Trim();
        entity.Cnh = request.Cnh.Trim();
        entity.Status = request.Status.Trim();
        entity.IdTransportadora = request.IdTransportadora;

        await _motoristaRepository.UpdateAsync(entity);
        return ResultWrapper<MotoristaDto>.Ok(MapToDto(entity), "Motorista atualizado com sucesso.");
    }

    // remove o motorista do sistema
    public async Task<ResultWrapper<bool>> DeleteAsync(int id)
    {
        var entity = await _motoristaRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<bool>.Fail("Motorista não encontrado.", $"Nenhum motorista com ID {id}.");
        }

        await _motoristaRepository.DeleteAsync(id);
        return ResultWrapper<bool>.Ok(true, "Motorista excluído com sucesso.");
    }

    // método auxiliar para mapeamento de entidade para dto
    private static MotoristaDto MapToDto(Motorista m)
    {
        return new MotoristaDto
        {
            Id = m.Id,
            Nome = m.Nome,
            Cnh = m.Cnh,
            Status = m.Status,
            IdTransportadora = m.IdTransportadora,
            NomeTransportadora = m.Transportadora?.NomeFantasia
        };
    }
}
