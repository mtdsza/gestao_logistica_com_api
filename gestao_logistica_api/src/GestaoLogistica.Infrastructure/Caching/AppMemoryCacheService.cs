using GestaoLogistica.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GestaoLogistica.Infrastructure.Caching;

// implementação do serviço de cache utilizando o imemorycache nativo do asp.net core
public class AppMemoryCacheService : IAppCacheService
{
    private readonly IMemoryCache _memoryCache;

    public AppMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    // busca um dado tipado diretamente na memória ram
    public Task<T?> GetAsync<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T? value))
        {
            return Task.FromResult(value);
        }

        return Task.FromResult<T?>(default);
    }

    // armazena um valor na memória definindo tempos de vida absoluto e deslizante
    // - absolute expiration: limite máximo total de permanência do item em cache
    // - sliding expiration: estende o tempo de expiração se o item for consultado com frequência
    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var options = new MemoryCacheEntryOptions();

        if (absoluteExpiration.HasValue)
        {
            options.SetAbsoluteExpiration(absoluteExpiration.Value);
        }

        if (slidingExpiration.HasValue)
        {
            options.SetSlidingExpiration(slidingExpiration.Value);
        }

        _memoryCache.Set(key, value, options);
        return Task.CompletedTask;
    }

    // remove o valor correspondente à chave informada (invalidação programada)
    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}
