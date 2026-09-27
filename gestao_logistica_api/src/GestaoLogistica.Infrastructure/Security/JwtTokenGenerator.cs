using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestaoLogistica.Application.Common.Interfaces;
using GestaoLogistica.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestaoLogistica.Infrastructure.Security;

// serviço de infraestrutura responsável por compor, assinar e emitir tokens jwt
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // gera o token jwt com base nas propriedades e perfil do usuário autenticado
    // ciclo de emissão:
    // 1. recupera chave secreta, emissor (issuer), audiência e tempo de expiração das configurações
    // 2. monta a chave simétrica e as credenciais de assinatura com algoritmo hmac sha-256
    // 3. adiciona as declarações (claims) do usuário: id, nome, perfil ('admin' ou 'motorista') e id do motorista
    // 4. assina e devolve o token em formato de string codificada em base64url junto à data de expiração
    public (string Token, DateTime Expiration) GenerateToken(Usuario usuario)
    {
        var secret = _configuration["JwtSettings:Secret"] ?? "super_secret_jwt_key_gestao_logistica_2026_eng_soft_key_long_enough!";
        var issuer = _configuration["JwtSettings:Issuer"] ?? "GestaoLogisticaApi";
        var audience = _configuration["JwtSettings:Audience"] ?? "GestaoLogisticaFlutter";
        var expirationHours = int.TryParse(_configuration["JwtSettings:ExpirationHours"], out var hours) ? hours : 8;

        // cria a chave simétrica para assinatura
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // calcula a data limite de validade do token
        var expiration = DateTime.UtcNow.AddHours(expirationHours);

        // define as claims (reivindicações de identidade) que trafegarão no payload do token
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Username),
            new(ClaimTypes.Role, usuario.IsAdmin ? "Admin" : "Motorista"),
            new("is_admin", usuario.IsAdmin ? "true" : "false")
        };

        // inclui claim customizada com o id do motorista quando aplicável
        if (usuario.IdMotorista.HasValue)
        {
            claims.Add(new Claim("id_motorista", usuario.IdMotorista.Value.ToString()));
        }

        // descreve as características do token a ser gerado
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // retorna o token serializado e a expiração calculada
        return (tokenHandler.WriteToken(token), expiration);
    }
}
