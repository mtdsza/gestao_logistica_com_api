using System.Text;
using GestaoLogistica.Api.Middlewares;
using GestaoLogistica.Application.Common.Interfaces;
using GestaoLogistica.Infrastructure;
using GestaoLogistica.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. injeção de dependências dos serviços das camadas de aplicação e infraestrutura
builder.Services.AddInfrastructureServices(builder.Configuration);

// 2. registro dos controladores da api
builder.Services.AddControllers();

// 3. configuração de política de cors liberando acesso para o aplicativo flutter (web, emulador e desktop)
// expõe explicitamente o cabeçalho 'x-cache' para que o cliente consiga inspecionar o status do cache
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Cache");
    });
});

// 4. configuração do middleware de autenticação e autorização via token jwt
var secret = builder.Configuration["JwtSettings:Secret"] ?? "super_secret_jwt_key_gestao_logistica_2026_eng_soft_key_long_enough!";
var issuer = builder.Configuration["JwtSettings:Issuer"] ?? "GestaoLogisticaApi";
var audience = builder.Configuration["JwtSettings:Audience"] ?? "GestaoLogisticaFlutter";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // em ambiente de testes/desenvolvimento permite conexões http locais
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    // parâmetros para validação estrita da assinatura e prazo do token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // expiração imediata sem tolerância de atraso de relógio
    };
});

builder.Services.AddAuthorization();

// 5. configuração do swagger/openapi com documentação interativa e suporte a autenticação por bearer token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gestão Logística API",
        Version = "v1",
        Description = "API RESTful em ASP.NET Core (.NET 9) com Clean Architecture, JWT, Cache e ResultWrapper."
    });

    // adiciona o botão 'authorize' no topo da interface do swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // exige a inclusão do token nas requisições autenticadas testadas pelo swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 6. pipeline de execução das requisições http
// middleware global para tratamento de exceções (converte qualquer erro inesperado em resultwrapper)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// disponibiliza a interface do swagger na raiz da aplicação ('/') para facilitar a exploração interativa
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestão Logística API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");

// habilita os middlewares de identificação de identidade e aplicação de permissões de acesso
app.UseAuthentication();
app.UseAuthorization();

// mapeia as rotas dos controllers
app.MapControllers();

// 7. execução do seeder para inicialização e povoamento automático do banco sqlite
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        await DbSeeder.SeedAsync(dbContext, passwordHasher);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao popular o banco de dados inicial.");
    }
}

app.Run();
