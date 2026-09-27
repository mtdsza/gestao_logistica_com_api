using GestaoLogistica.Application.Common.Interfaces;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Application.Services;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Infrastructure.Caching;
using GestaoLogistica.Infrastructure.Data;
using GestaoLogistica.Infrastructure.Repositories;
using GestaoLogistica.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoLogistica.Infrastructure;

// método de extensão para registrar todas as dependências das camadas de infraestrutura e aplicação no contêiner ioc nativo
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. configuração do banco de dados sqlite com ef core
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=gestao_logistica.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // 2. registro do serviço de cache em memória
        services.AddMemoryCache();
        services.AddSingleton<IAppCacheService, AppMemoryCacheService>();

        // 3. registro dos utilitários de segurança, hashing e geração de jwt
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // 4. registro dos repositórios de dados com escopo scoped (por requisição)
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITransportadoraRepository, TransportadoraRepository>();
        services.AddScoped<IMotoristaRepository, MotoristaRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<IItemInventarioRepository, ItemInventarioRepository>();
        services.AddScoped<IViagemRepository, ViagemRepository>();

        // 5. registro dos serviços de aplicação que implementam os casos de uso
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITransportadoraService, TransportadoraService>();
        services.AddScoped<IMotoristaService, MotoristaService>();
        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<IItemInventarioService, ItemInventarioService>();
        services.AddScoped<IViagemService, ViagemService>();

        return services;
    }
}
