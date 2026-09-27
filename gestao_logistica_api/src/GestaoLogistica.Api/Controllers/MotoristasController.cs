using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Motorista;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.Api.Controllers;

// controlador restful para gerenciamento da equipe de motoristas
[Authorize]
public class MotoristasController : BaseApiController
{
    private readonly IMotoristaService _motoristaService;

    public MotoristasController(IMotoristaService motoristaService)
    {
        _motoristaService = motoristaService;
    }

    // lista todos os motoristas
    [HttpGet]
    public async Task<ActionResult<ResultWrapper<List<MotoristaDto>>>> GetAll()
    {
        var result = await _motoristaService.GetAllAsync();
        return CustomResult(result);
    }

    // lista apenas motoristas com status 'disponivel' para alocação em fretes
    [HttpGet("disponiveis")]
    public async Task<ActionResult<ResultWrapper<List<MotoristaDto>>>> GetAvailable()
    {
        var result = await _motoristaService.GetAvailableAsync();
        return CustomResult(result);
    }

    // busca motorista por id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResultWrapper<MotoristaDto>>> GetById(int id)
    {
        var result = await _motoristaService.GetByIdAsync(id);
        return CustomResult(result);
    }

    // cadastra um novo motorista (restrito a administradores)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<MotoristaDto>>> Create([FromBody] CreateMotoristaDto request)
    {
        var result = await _motoristaService.CreateAsync(request);
        return CustomResult(result);
    }

    // atualiza dados de um motorista existente (restrito a administradores)
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<MotoristaDto>>> Update(int id, [FromBody] UpdateMotoristaDto request)
    {
        var result = await _motoristaService.UpdateAsync(id, request);
        return CustomResult(result);
    }

    // remove o motorista do sistema (restrito a administradores)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<bool>>> Delete(int id)
    {
        var result = await _motoristaService.DeleteAsync(id);
        return CustomResult(result);
    }
}
