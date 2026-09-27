using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.ItemInventario;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.Api.Controllers;

// controlador restful para gerenciamento de itens de inventário e estoque
// integra a demonstração do requisito de cache em memória
[Authorize]
[Route("api/itens-inventario")]
[Route("api/itensinventario")]
public class ItensInventarioController : BaseApiController
{
    private readonly IItemInventarioService _service;

    public ItensInventarioController(IItemInventarioService service)
    {
        _service = service;
    }

    // lista todos os itens do inventário expondo o cabeçalho 'x-cache'
    // - x-cache: hit  -> o dado foi lido diretamente da memória ram do servidor
    // - x-cache: miss -> o dado foi lido do banco sqlite e salvo no cache
    [HttpGet]
    public async Task<ActionResult<ResultWrapper<List<ItemInventarioDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();

        // insere o cabeçalho http personalizado indicando se houve hit ou miss no cache em memória
        if (result.Message.Contains("via Cache", StringComparison.OrdinalIgnoreCase))
        {
            Response.Headers.Append("X-Cache", "HIT");
        }
        else
        {
            Response.Headers.Append("X-Cache", "MISS");
        }

        return CustomResult(result);
    }

    // busca item específico por id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResultWrapper<ItemInventarioDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return CustomResult(result);
    }

    // cadastra um novo produto no estoque e invalida o cache (restrito a administradores)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<ItemInventarioDto>>> Create([FromBody] CreateItemInventarioDto request)
    {
        var result = await _service.CreateAsync(request);
        return CustomResult(result);
    }

    // edita dados de um produto e invalida o cache (restrito a administradores)
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<ItemInventarioDto>>> Update(int id, [FromBody] UpdateItemInventarioDto request)
    {
        var result = await _service.UpdateAsync(id, request);
        return CustomResult(result);
    }

    // exclui um produto do inventário e invalida o cache (restrito a administradores)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<bool>>> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return CustomResult(result);
    }

    // atualiza rapidamente o saldo em estoque de um item e invalida o cache (restrito a administradores)
    [HttpPatch("{id:int}/estoque")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<bool>>> UpdateStock(int id, [FromBody] UpdateStockDto request)
    {
        var result = await _service.UpdateStockAsync(id, request.Quantidade);
        return CustomResult(result);
    }
}
