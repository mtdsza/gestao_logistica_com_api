using GestaoLogistica.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.Api.Controllers;

// controlador base abstrato que padroniza o mapeamento de ResultWrapper para os códigos de status http correspondentes
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    // mapeia o ResultWrapper<T> para as respostas http adequadas (200 ok, 404 not found, 403 forbidden ou 400 bad request)
    protected ActionResult<ResultWrapper<T>> CustomResult<T>(ResultWrapper<T> result)
    {
        // se a operação foi bem-sucedida, retorna http 200 ok
        if (result.Success)
        {
            return Ok(result);
        }

        // se o recurso não foi encontrado, retorna http 404 not found
        if (result.Message.Contains("não encontrad", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(result);
        }

        // se o acesso foi negado ou a conta estiver inativa, retorna http 403 forbidden
        if (result.Message.Contains("Acesso negado", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, result);
        }

        // para erros de validação ou regras de negócio violadas, retorna http 400 bad request
        return BadRequest(result);
    }

    // sobrecarga para ResultWrapper sem dados genéricos
    protected ActionResult<ResultWrapper> CustomResult(ResultWrapper result)
    {
        if (result.Success)
        {
            return Ok(result);
        }

        if (result.Message.Contains("não encontrad", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(result);
        }

        return BadRequest(result);
    }
}
