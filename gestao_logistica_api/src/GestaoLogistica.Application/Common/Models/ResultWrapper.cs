using System.Text.Json.Serialization;

namespace GestaoLogistica.Application.Common.Models;

// estrutura padrão unificada para encapsular todas as respostas da api (sucesso e falha)
// atende ao requisito arquitetural obrigatório de padronização global com ResultWrapper
public class ResultWrapper<T>
{
    // indica se a operação foi executada com êxito (true) ou se houve erro/validação (false)
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    // mensagem textual explicativa e amigável sobre o desfecho da requisição
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    // carga útil (payload) contendo os dados retornados na operação; nulo em caso de falha
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    // coleção contendo detalhes de validações não atendidas ou motivos específicos de erro
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();

    // fábrica para criar uma resposta padronizada de sucesso com dados
    public static ResultWrapper<T> Ok(T data, string message = "Operação realizada com sucesso.")
    {
        return new ResultWrapper<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = new List<string>()
        };
    }

    // fábrica para criar uma resposta padronizada de falha com lista de erros
    public static ResultWrapper<T> Fail(string message, List<string>? errors = null)
    {
        return new ResultWrapper<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors ?? new List<string>()
        };
    }

    // fábrica para criar uma resposta de falha com uma mensagem de erro pontual
    public static ResultWrapper<T> Fail(string message, string error)
    {
        return new ResultWrapper<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = new List<string> { error }
        };
    }
}

// versão não-genérica para operações que não retornam dados no payload (ex.: delete, update status)
public class ResultWrapper : ResultWrapper<object>
{
    // fábrica para resposta de sucesso sem carga útil de dados
    public static ResultWrapper Ok(string message = "Operação realizada com sucesso.")
    {
        return new ResultWrapper
        {
            Success = true,
            Message = message,
            Data = null,
            Errors = new List<string>()
        };
    }

    // fábrica para resposta de falha sem dados e com lista de erros
    public static new ResultWrapper Fail(string message, List<string>? errors = null)
    {
        return new ResultWrapper
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors ?? new List<string>()
        };
    }

    // fábrica para resposta de falha sem dados e com erro único
    public static new ResultWrapper Fail(string message, string error)
    {
        return new ResultWrapper
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = new List<string> { error }
        };
    }
}
