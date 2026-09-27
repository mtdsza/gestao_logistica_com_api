using GestaoLogistica.Domain.Entities;

namespace GestaoLogistica.Application.Common.Interfaces;

// abstração para o serviço emissor de tokens jwt (implementado na camada de infraestrutura)
public interface IJwtTokenGenerator
{
    // gera o token jwt codificado e a data/hora exata de expiração a partir dos dados do usuário
    (string Token, DateTime Expiration) GenerateToken(Usuario usuario);
}

// abstração para o serviço de hashing seguro e verificação de senhas com salt
public interface IPasswordHasher
{
    // gera o hash criptografado da senha usando algoritmo seguro (bcrypt)
    string HashPassword(string password);

    // verifica se a senha digitada em texto puro confere com o hash armazenado
    bool VerifyPassword(string password, string passwordHash);
}

// abstração para o serviço de cache em memória ou distribuído
public interface IAppCacheService
{
    // tenta recuperar um valor armazenado no cache pela chave identificadora
    Task<T?> GetAsync<T>(string key);

    // armazena um valor no cache configurando tempos de expiração absoluta e deslizante
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);

    // remove imediatamente uma chave do cache (invalidação reativa quando os dados sofrem mutação)
    Task RemoveAsync(string key);
}
