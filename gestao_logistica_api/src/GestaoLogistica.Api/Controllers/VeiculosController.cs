using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Veiculo;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.Api.Controllers;

// controlador restful para administração da frota de veículos
[Authorize]
public class VeiculosController : BaseApiController
{
    private readonly IVeiculoService _veiculoService;

    public VeiculosController(IVeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    // lista todos os veículos cadastrados
    [HttpGet]
    public async Task<ActionResult<ResultWrapper<List<VeiculoDto>>>> GetAll()
    {
        var result = await _veiculoService.GetAllAsync();
        return CustomResult(result);
    }

    // lista apenas os veículos livres com status 'disponivel'
    [HttpGet("disponiveis")]
    public async Task<ActionResult<ResultWrapper<List<VeiculoDto>>>> GetAvailable()
    {
        var result = await _veiculoService.GetAvailableAsync();
        return CustomResult(result);
    }

    // busca veículo pelo id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResultWrapper<VeiculoDto>>> GetById(int id)
    {
        var result = await _veiculoService.GetByIdAsync(id);
        return CustomResult(result);
    }

    // adiciona um novo veículo à frota (restrito a administradores)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<VeiculoDto>>> Create([FromBody] CreateVeiculoDto request)
    {
        var result = await _veiculoService.CreateAsync(request);
        return CustomResult(result);
    }

    // atualiza dados ou capacidade do veículo (restrito a administradores)
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<VeiculoDto>>> Update(int id, [FromBody] UpdateVeiculoDto request)
    {
        var result = await _veiculoService.UpdateAsync(id, request);
        return CustomResult(result);
    }

    // remove um veículo da frota (restrito a administradores)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<bool>>> Delete(int id)
    {
        var result = await _veiculoService.DeleteAsync(id);
        return CustomResult(result);
    }
}
