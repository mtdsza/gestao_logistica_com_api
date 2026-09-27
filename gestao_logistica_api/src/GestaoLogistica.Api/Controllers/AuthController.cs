using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Auth;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.Api.Controllers;

// controlador responsável pelos endpoints de autenticação, registro e administração de usuários
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // endpoint público para autenticação de usuários; retorna o token jwt e dados da sessão
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultWrapper<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return CustomResult(result);
    }

    // endpoint público para criação de novas contas com hash seguro de senha
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ResultWrapper<UsuarioDto>>> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return CustomResult(result);
    }

    // endpoint protegido restrito a administradores para listar todos os usuários
    [HttpGet("usuarios")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<List<UsuarioDto>>>> GetAllUsuarios()
    {
        var result = await _authService.GetAllUsuariosAsync();
        return CustomResult(result);
    }

    // endpoint protegido restrito a administradores para ativar ou desativar o acesso de um usuário
    [HttpPatch("usuarios/{id}/ativo")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultWrapper<bool>>> UpdateAtivo(int id, [FromBody] UpdateAtivoDto request)
    {
        var result = await _authService.UpdateAtivoAsync(id, request.Ativo);
        return CustomResult(result);
    }
}
