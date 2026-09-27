using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Transportadora;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.Api.Controllers;

// controlador restful para gerenciamento de transportadoras parceiras
[Authorize]
public class TransportadorasController : BaseApiController
{
    private readonly ITransportadoraService _transportadoraService;

    public TransportadorasController(ITransportadoraService transportadoraService)
    {
        _transportadoraService = transportadoraService;
    }

    // lista todas as transportadoras cadastradas
    [HttpGet]
    public async Task<ActionResult<ResultWrapper<List<TransportadoraDto>>>> GetAll()
    {
        var result = await _transportadoraService.GetAllAsync();
        return CustomResult(result);
    }

    // consulta detalhes de uma transportadora por id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResultWrapper<TransportadoraDto>>> GetById(int id)
    {
        var result = await _transportadoraService.GetByIdAsync(id);
        return CustomResult(result);
    }

    // cadastra uma nova transportadora com tarifas (restrito a administradores)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<TransportadoraDto>>> Create([FromBody] CreateTransportadoraDto request)
    {
        var result = await _transportadoraService.CreateAsync(request);
        return CustomResult(result);
    }

    // atualiza dados e tarifas contratuais de uma transportadora (restrito a administradores)
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<TransportadoraDto>>> Update(int id, [FromBody] UpdateTransportadoraDto request)
    {
        var result = await _transportadoraService.UpdateAsync(id, request);
        return CustomResult(result);
    }

    // remove uma transportadora do sistema (restrito a administradores)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<bool>>> Delete(int id)
    {
        var result = await _transportadoraService.DeleteAsync(id);
        return CustomResult(result);
    }
}
