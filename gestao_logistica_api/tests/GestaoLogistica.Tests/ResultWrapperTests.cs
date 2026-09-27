using GestaoLogistica.Application.Common.Models;
using Xunit;

namespace GestaoLogistica.Tests;

// testes unitários para validação do envelope padronizado de resposta (resultwrapper)
public class ResultWrapperTests
{
    // valida se o método estático ok gera um resultado bem-sucedido com dados e lista de erros vazia
    [Fact]
    public void ResultWrapper_Ok_ShouldHaveCorrectStructure()
    {
        // cria um resultado positivo contendo payload e mensagem
        var result = ResultWrapper<string>.Ok("meus_dados", "Operação concluída com sucesso.");

        // verifica os campos essenciais do contrato de resposta
        Assert.True(result.Success);
        Assert.Equal("Operação concluída com sucesso.", result.Message);
        Assert.Equal("meus_dados", result.Data);
        Assert.NotNull(result.Errors);
        Assert.Empty(result.Errors);
    }

    // valida se o método estático fail gera um resultado com sucesso falso e erros anexados
    [Fact]
    public void ResultWrapper_Fail_ShouldContainErrorsAndNoData()
    {
        // cria um resultado de falha com lista de erros operacionais
        var result = ResultWrapper<string>.Fail("Erro ao processar.", "Campo obrigatório ausente.");

        // assegura que o payload é nulo e os erros estão acessíveis
        Assert.False(result.Success);
        Assert.Equal("Erro ao processar.", result.Message);
        Assert.Null(result.Data);
        Assert.Single(result.Errors);
        Assert.Equal("Campo obrigatório ausente.", result.Errors[0]);
    }
}

