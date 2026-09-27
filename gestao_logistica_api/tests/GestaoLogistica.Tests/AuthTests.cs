using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GestaoLogistica.Tests;

// testes unitários para os componentes de segurança, hashing e emissão de jwt
public class AuthTests
{
    private readonly BcryptPasswordHasher _hasher = new();
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthTests()
    {
        // configura valores simulados de configuração para emissão do token nos testes
        var configValues = new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", "super_secret_jwt_key_gestao_logistica_2026_eng_soft_key_long_enough!" },
            { "JwtSettings:Issuer", "GestaoLogisticaApi" },
            { "JwtSettings:Audience", "GestaoLogisticaFlutter" },
            { "JwtSettings:ExpirationHours", "8" }
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        _tokenGenerator = new JwtTokenGenerator(config);
    }

    // valida se o bcrypt gera hashes diferentes do texto puro e valida senhas corretas e incorretas
    [Fact]
    public void Bcrypt_ShouldHashAndVerifyPasswordCorrectly()
    {
        string rawPassword = "minhasenhasegura123";
        string hash = _hasher.HashPassword(rawPassword);

        // o hash deve ser criptografado e diferente do texto original
        Assert.NotEqual(rawPassword, hash);

        // a validação deve aceitar a senha correta e rejeitar senhas incorretas
        Assert.True(_hasher.VerifyPassword(rawPassword, hash));
        Assert.False(_hasher.VerifyPassword("senhaerrada", hash));
    }

    // valida se o token gerado contém as claims de identidade, perfil de acesso e prazo de validade
    [Fact]
    public void JwtTokenGenerator_ShouldIncludeExpectedClaims()
    {
        var usuario = new Usuario
        {
            Id = 1,
            Username = "admin",
            IsAdmin = true,
            IdMotorista = null,
            Ativo = true
        };

        var (token, expiration) = _tokenGenerator.GenerateToken(usuario);

        Assert.NotNull(token);
        Assert.True(expiration > DateTime.UtcNow);

        // lê as claims descriptografadas do token
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Contains(jwt.Claims, c => (c.Type == ClaimTypes.Name || c.Type == "unique_name") && c.Value == "admin");
        Assert.Contains(jwt.Claims, c => (c.Type == ClaimTypes.Role || c.Type == "role") && c.Value == "Admin");
        Assert.Contains(jwt.Claims, c => (c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid") && c.Value == "1");
        Assert.Contains(jwt.Claims, c => c.Type == "is_admin" && c.Value == "true");
    }
}
