using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Transportadora;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;

namespace GestaoLogistica.Application.Services;

// serviço de aplicação para orquestrar as regras de cadastro e consulta de transportadoras
public class TransportadoraService : ITransportadoraService
{
    private readonly ITransportadoraRepository _transportadoraRepository;

    public TransportadoraService(ITransportadoraRepository transportadoraRepository)
    {
        _transportadoraRepository = transportadoraRepository;
    }

    // lista todas as transportadoras ativas e converte para dto
    public async Task<ResultWrapper<List<TransportadoraDto>>> GetAllAsync()
    {
        var list = await _transportadoraRepository.GetAllAsync();
        var dtos = list.Select(t => new TransportadoraDto
        {
            Id = t.Id,
            Cnpj = t.Cnpj,
            NomeFantasia = t.NomeFantasia,
            CustoKm = t.CustoKm,
            CustoPeso = t.CustoPeso
        }).ToList();

        return ResultWrapper<List<TransportadoraDto>>.Ok(dtos);
    }

    // busca uma transportadora pelo identificador numérico
    public async Task<ResultWrapper<TransportadoraDto>> GetByIdAsync(int id)
    {
        var t = await _transportadoraRepository.GetByIdAsync(id);
        if (t == null)
        {
            return ResultWrapper<TransportadoraDto>.Fail("Transportadora não encontrada.", $"Nenhuma transportadora encontrada com o ID {id}.");
        }

        var dto = new TransportadoraDto
        {
            Id = t.Id,
            Cnpj = t.Cnpj,
            NomeFantasia = t.NomeFantasia,
            CustoKm = t.CustoKm,
            CustoPeso = t.CustoPeso
        };

        return ResultWrapper<TransportadoraDto>.Ok(dto);
    }

    // valida e cadastra uma nova transportadora
    public async Task<ResultWrapper<TransportadoraDto>> CreateAsync(CreateTransportadoraDto request)
    {
        // validações básicas de preenchimento
        if (string.IsNullOrWhiteSpace(request.Cnpj) || string.IsNullOrWhiteSpace(request.NomeFantasia))
        {
            return ResultWrapper<TransportadoraDto>.Fail("Dados inválidos.", "CNPJ e Nome Fantasia são obrigatórios.");
        }

        // verifica duplicidade de cnpj
        var existente = await _transportadoraRepository.GetByCnpjAsync(request.Cnpj.Trim());
        if (existente != null)
        {
            return ResultWrapper<TransportadoraDto>.Fail("CNPJ já cadastrado.", "Já existe uma transportadora com este CNPJ.");
        }

        var entity = new Transportadora
        {
            Cnpj = request.Cnpj.Trim(),
            NomeFantasia = request.NomeFantasia.Trim(),
            CustoKm = request.CustoKm,
            CustoPeso = request.CustoPeso
        };

        await _transportadoraRepository.AddAsync(entity);

        var dto = new TransportadoraDto
        {
            Id = entity.Id,
            Cnpj = entity.Cnpj,
            NomeFantasia = entity.NomeFantasia,
            CustoKm = entity.CustoKm,
            CustoPeso = entity.CustoPeso
        };

        return ResultWrapper<TransportadoraDto>.Ok(dto, "Transportadora criada com sucesso.");
    }

    // atualiza os dados e tarifas de uma transportadora existente
    public async Task<ResultWrapper<TransportadoraDto>> UpdateAsync(int id, UpdateTransportadoraDto request)
    {
        var entity = await _transportadoraRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<TransportadoraDto>.Fail("Transportadora não encontrada.", $"Nenhuma transportadora com ID {id}.");
        }

        entity.Cnpj = request.Cnpj.Trim();
        entity.NomeFantasia = request.NomeFantasia.Trim();
        entity.CustoKm = request.CustoKm;
        entity.CustoPeso = request.CustoPeso;

        await _transportadoraRepository.UpdateAsync(entity);

        var dto = new TransportadoraDto
        {
            Id = entity.Id,
            Cnpj = entity.Cnpj,
            NomeFantasia = entity.NomeFantasia,
            CustoKm = entity.CustoKm,
            CustoPeso = entity.CustoPeso
        };

        return ResultWrapper<TransportadoraDto>.Ok(dto, "Transportadora atualizada com sucesso.");
    }

    // exclui uma transportadora pelo id
    public async Task<ResultWrapper<bool>> DeleteAsync(int id)
    {
        var entity = await _transportadoraRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<bool>.Fail("Transportadora não encontrada.", $"Nenhuma transportadora com ID {id}.");
        }

        await _transportadoraRepository.DeleteAsync(id);
        return ResultWrapper<bool>.Ok(true, "Transportadora removida com sucesso.");
    }
}
