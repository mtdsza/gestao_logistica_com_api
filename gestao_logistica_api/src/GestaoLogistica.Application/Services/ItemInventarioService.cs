using GestaoLogistica.Application.Common.Interfaces;
using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.ItemInventario;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;

namespace GestaoLogistica.Application.Services;

// serviço de aplicação encarregado do inventário e estoque de mercadorias
// implementa o mecanismo obrigatório de cache em memória demonstrando cache hit, cache miss e invalidação reativa
public class ItemInventarioService : IItemInventarioService
{
    // chave única para armazenar a lista completa de produtos na memória cache
    private const string CacheKeyAllItens = "itens_inventario_all";

    // tempo máximo absoluto de permanência do dado em memória (5 minutos)
    private static readonly TimeSpan CacheAbsoluteExpiration = TimeSpan.FromMinutes(5);

    // tempo de renovação deslizante a cada acesso frequente (2 minutos)
    private static readonly TimeSpan CacheSlidingExpiration = TimeSpan.FromMinutes(2);

    private readonly IItemInventarioRepository _repository;
    private readonly IAppCacheService _cacheService;

    public ItemInventarioService(
        IItemInventarioRepository repository,
        IAppCacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    // consulta o catálogo de itens de inventário com ciclo de vida completo de cache:
    // 1. cache hit: se os dados já existirem em memória ram, devolve instantaneamente sem onerar o banco sqlite
    // 2. cache miss: caso contrário, busca a lista primária no sqlite e popula a memória cache com as políticas de ttl
    public async Task<ResultWrapper<List<ItemInventarioDto>>> GetAllAsync()
    {
        // 1. tenta recuperar do cache em memória (cenário de cache hit)
        var cached = await _cacheService.GetAsync<List<ItemInventarioDto>>(CacheKeyAllItens);
        if (cached != null)
        {
            return ResultWrapper<List<ItemInventarioDto>>.Ok(cached, "Dados obtidos com sucesso (via Cache).");
        }

        // 2. dado não encontrado na memória (cenário de cache miss): consulta a fonte primária no banco
        var list = await _repository.GetAllAsync();
        var dtos = list.Select(MapToDto).ToList();

        // 3. salva o resultado na memória cache definindo expiração absoluta e deslizante
        await _cacheService.SetAsync(CacheKeyAllItens, dtos, CacheAbsoluteExpiration, CacheSlidingExpiration);

        return ResultWrapper<List<ItemInventarioDto>>.Ok(dtos, "Dados obtidos com sucesso (via Banco de Dados).");
    }

    // busca item específico por identificador
    public async Task<ResultWrapper<ItemInventarioDto>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
        {
            return ResultWrapper<ItemInventarioDto>.Fail("Item não encontrado.", $"Nenhum item com ID {id}.");
        }

        return ResultWrapper<ItemInventarioDto>.Ok(MapToDto(item));
    }

    // cadastra um novo item e executa a invalidação reativa do cache
    public async Task<ResultWrapper<ItemInventarioDto>> CreateAsync(CreateItemInventarioDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Tipo) || string.IsNullOrWhiteSpace(request.Cidade))
        {
            return ResultWrapper<ItemInventarioDto>.Fail("Dados inválidos.", "Nome, Tipo e Cidade são obrigatórios.");
        }

        if (request.PesoUnitario <= 0)
        {
            return ResultWrapper<ItemInventarioDto>.Fail("Peso inválido.", "O peso unitário deve ser maior que zero.");
        }

        if (request.Quantidade < 0)
        {
            return ResultWrapper<ItemInventarioDto>.Fail("Quantidade inválida.", "A quantidade não pode ser negativa.");
        }

        var entity = new ItemInventario
        {
            Nome = request.Nome.Trim(),
            Tipo = request.Tipo.Trim(),
            PesoUnitario = request.PesoUnitario,
            Quantidade = request.Quantidade,
            Cidade = request.Cidade.Trim(),
            DataValidade = request.DataValidade,
            Localizacao = request.Localizacao
        };

        await _repository.AddAsync(entity);

        // invalidação reativa: remove a chave antiga da memória para que a próxima busca leia o dado novo
        await _cacheService.RemoveAsync(CacheKeyAllItens);

        return ResultWrapper<ItemInventarioDto>.Ok(MapToDto(entity), "Item de inventário cadastrado com sucesso.");
    }

    // edita um item e invalida o cache
    public async Task<ResultWrapper<ItemInventarioDto>> UpdateAsync(int id, UpdateItemInventarioDto request)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<ItemInventarioDto>.Fail("Item não encontrado.", $"Nenhum item com ID {id}.");
        }

        if (request.PesoUnitario <= 0)
        {
            return ResultWrapper<ItemInventarioDto>.Fail("Peso inválido.", "O peso unitário deve ser maior que zero.");
        }

        if (request.Quantidade < 0)
        {
            return ResultWrapper<ItemInventarioDto>.Fail("Quantidade inválida.", "A quantidade não pode ser negativa.");
        }

        entity.Nome = request.Nome.Trim();
        entity.Tipo = request.Tipo.Trim();
        entity.PesoUnitario = request.PesoUnitario;
        entity.Quantidade = request.Quantidade;
        entity.Cidade = request.Cidade.Trim();
        entity.DataValidade = request.DataValidade;
        entity.Localizacao = request.Localizacao;

        await _repository.UpdateAsync(entity);

        // invalidação reativa: expira os dados antigos em memória imediatamente
        await _cacheService.RemoveAsync(CacheKeyAllItens);

        return ResultWrapper<ItemInventarioDto>.Ok(MapToDto(entity), "Item atualizado com sucesso.");
    }

    // exclui um item e limpa a chave de cache
    public async Task<ResultWrapper<bool>> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<bool>.Fail("Item não encontrado.", $"Nenhum item com ID {id}.");
        }

        await _repository.DeleteAsync(id);

        // invalidação reativa
        await _cacheService.RemoveAsync(CacheKeyAllItens);

        return ResultWrapper<bool>.Ok(true, "Item excluído com sucesso.");
    }

    // ajusta o saldo em estoque e invalida o cache
    public async Task<ResultWrapper<bool>> UpdateStockAsync(int id, int novaQuantidade)
    {
        if (novaQuantidade < 0)
        {
            return ResultWrapper<bool>.Fail("Quantidade inválida.", "A quantidade não pode ser negativa.");
        }

        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            return ResultWrapper<bool>.Fail("Item não encontrado.", $"Nenhum item com ID {id}.");
        }

        await _repository.UpdateStockAsync(id, novaQuantidade);

        // invalidação reativa
        await _cacheService.RemoveAsync(CacheKeyAllItens);

        return ResultWrapper<bool>.Ok(true, "Estoque atualizado com sucesso.");
    }

    // conversão da entidade para dto
    private static ItemInventarioDto MapToDto(ItemInventario i)
    {
        return new ItemInventarioDto
        {
            Id = i.Id,
            Nome = i.Nome,
            Tipo = i.Tipo,
            PesoUnitario = i.PesoUnitario,
            Quantidade = i.Quantidade,
            Cidade = i.Cidade,
            DataValidade = i.DataValidade,
            Localizacao = i.Localizacao
        };
    }
}
