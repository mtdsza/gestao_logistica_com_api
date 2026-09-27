using System.Net;
using System.Text.Json;
using GestaoLogistica.Application.Common.Models;

namespace GestaoLogistica.Api.Middlewares;

// middleware interceptor global para captura e padronização de exceções não tratadas
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // intercepta a requisição e captura qualquer falha inesperada em qualquer camada
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado capturado pelo middleware global: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    // formata a resposta no padrão resultwrapper com status http 500 (internal server error)
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // monta a resposta no formato ResultWrapper.Fail garantindo a regra de 100% de conformidade
        var response = ResultWrapper.Fail(
            "Ocorreu um erro interno inesperado no servidor.",
            new List<string> { exception.Message }
        );

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
