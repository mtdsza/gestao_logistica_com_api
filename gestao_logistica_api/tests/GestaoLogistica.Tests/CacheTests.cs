using GestaoLogistica.Application.DTOs.ItemInventario;
using GestaoLogistica.Application.Services;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace GestaoLogistica.Tests;

// testes automatizados comprovando os cenários de cache hit, cache miss e invalidação reativa
public class CacheTests
{
    // repositório simulado em memória que contabiliza o número de idas ao banco de dados
    private class FakeItemInventarioRepository : IItemInventarioRepository
    {
        public int QueryCount { get; private set; }
        public List<ItemInventario> Items { get; } = new()
        {
            new ItemInventario { Id = 1, Nome = "Item A", Tipo = "Geral", PesoUnitario = 10m, Quantidade = 50, Cidade = "Curitiba" },
            new ItemInventario { Id = 2, Nome = "Item B", Tipo = "Eletronico", PesoUnitario = 5m, Quantidade = 20, Cidade = "Sao Paulo" }
        };

        public Task<List<ItemInventario>> GetAllAsync()
        {
            QueryCount++; // incrementa a cada acesso direto ao "banco"
            return Task.FromResult(Items.ToList());
        }

        public Task<ItemInventario?> GetByIdAsync(int id) => Task.FromResult(Items.FirstOrDefault(i => i.Id == id));
        public Task AddAsync(ItemInventario item) { Items.Add(item); return Task.CompletedTask; }
        public Task UpdateAsync(ItemInventario item) => Task.CompletedTask;
        public Task DeleteAsync(int id) { Items.RemoveAll(i => i.Id == id); return Task.CompletedTask; }
        public Task UpdateStockAsync(int id, int novaQuantidade) => Task.CompletedTask;
    }

    // validação do ciclo de vida:
    // 1. primeira leitura: cache miss (busca no banco e popula cache)
    // 2. segunda leitura: cache hit (busca da memória sem consultar o banco)
    // 3. inclusão de item: invalidação reativa (remove chave do cache)
    // 4. terceira leitura: novo cache miss (busca dados atualizados no banco)
    [Fact]
    public async Task Cache_ShouldHitOnSecondCallAndInvalidateOnMutation()
    {
        var repo = new FakeItemInventarioRepository();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cacheService = new AppMemoryCacheService(memoryCache);
        var service = new ItemInventarioService(repo, cacheService);

        // 1ª chamada: Cache Miss (deve ir ao repositório)
        var result1 = await service.GetAllAsync();
        Assert.True(result1.Success);
        Assert.Contains("Banco de Dados", result1.Message);
        Assert.Equal(1, repo.QueryCount);

        // 2ª chamada: Cache Hit (deve vir da memória ram, sem tocar no repositório)
        var result2 = await service.GetAllAsync();
        Assert.True(result2.Success);
        Assert.Contains("Cache", result2.Message);
        Assert.Equal(1, repo.QueryCount); // contagem permaneceu 1 comprovando cache hit

        // 3ª chamada: criação de novo item -> deve invalidar o cache reativamente
        await service.CreateAsync(new CreateItemInventarioDto
        {
            Nome = "Item C",
            Tipo = "Geral",
            PesoUnitario = 2m,
            Quantidade = 10,
            Cidade = "Belo Horizonte"
        });

        // 4ª chamada: como o cache foi invalidado, deve ocorrer um novo Cache Miss
        var result3 = await service.GetAllAsync();
        Assert.True(result3.Success);
        Assert.Contains("Banco de Dados", result3.Message);
        Assert.Equal(2, repo.QueryCount); // foi ao repositório novamente
        Assert.Equal(3, result3.Data!.Count);
    }
}
