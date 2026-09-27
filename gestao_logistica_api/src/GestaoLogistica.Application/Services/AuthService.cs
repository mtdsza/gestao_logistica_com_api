using GestaoLogistica.Application.Common.Interfaces;
using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Auth;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;

namespace GestaoLogistica.Application.Services;

// serviço de aplicação encarregado das regras de autenticação, emissão de credenciais e controle de usuários
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    // injeção de dependências das abstrações de persistência, criptografia e geração de tokens
    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // autentica o usuário conferindo a senha com hash seguro e gera o token jwt
    public async Task<ResultWrapper<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        // valida o preenchimento obrigatório dos campos de identificação
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ResultWrapper<LoginResponseDto>.Fail("Credenciais inválidas.", "Usuário e senha são obrigatórios.");
        }

        // busca o usuário no repositório ignorando sensibilidade de caixa
        var usuario = await _usuarioRepository.GetByUsernameAsync(request.Username.Trim());
        if (usuario == null)
        {
            return ResultWrapper<LoginResponseDto>.Fail("Usuário ou senha inválidos.", "Credenciais não conferem.");
        }

        // verifica se a conta do usuário está habilitada no sistema
        if (!usuario.Ativo)
        {
            return ResultWrapper<LoginResponseDto>.Fail("Acesso negado.", "Usuário inativo no sistema.");
        }

        // verifica a senha digitada contra o hash bcrypt com salt armazenado no banco
        bool senhaValida = _passwordHasher.VerifyPassword(request.Password, usuario.PasswordHash);
        if (!senhaValida)
        {
            return ResultWrapper<LoginResponseDto>.Fail("Usuário ou senha inválidos.", "Credenciais não conferem.");
        }

        // gera o token jwt assinado contendo as permissões (claims) do usuário
        var (token, expiration) = _jwtTokenGenerator.GenerateToken(usuario);

        // monta o dto de resposta com o token e dados essenciais da sessão
        var response = new LoginResponseDto
        {
            Token = token,
            Id = usuario.Id,
            Username = usuario.Username,
            IsAdmin = usuario.IsAdmin,
            IdMotorista = usuario.IdMotorista,
            Expiration = expiration
        };

        return ResultWrapper<LoginResponseDto>.Ok(response, "Login realizado com sucesso.");
    }

    // cadastra um novo usuário gerando hash criptográfico para a senha informada
    public async Task<ResultWrapper<UsuarioDto>> RegisterAsync(RegisterRequestDto request)
    {
        // valida se usuário e senha foram fornecidos
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ResultWrapper<UsuarioDto>.Fail("Dados inválidos.", "Usuário e senha são obrigatórios.");
        }

        // garante a unicidade do nome de usuário no sistema
        var existente = await _usuarioRepository.GetByUsernameAsync(request.Username.Trim());
        if (existente != null)
        {
            return ResultWrapper<UsuarioDto>.Fail("Conflito de cadastro.", "Já existe um usuário com este nome.");
        }

        // monta a entidade aplicando hash seguro na senha
        var usuario = new Usuario
        {
            Username = request.Username.Trim(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsAdmin = request.IsAdmin,
            IdMotorista = request.IdMotorista,
            Ativo = true
        };

        await _usuarioRepository.AddAsync(usuario);

        // projeta o resultado em dto de saída sem expor o hash da senha
        var dto = new UsuarioDto
        {
            Id = usuario.Id,
            Username = usuario.Username,
            IsAdmin = usuario.IsAdmin,
            IdMotorista = usuario.IdMotorista,
            Ativo = usuario.Ativo
        };

        return ResultWrapper<UsuarioDto>.Ok(dto, "Usuário cadastrado com sucesso.");
    }

    // retorna a lista de todos os usuários para gerenciamento administrativo
    public async Task<ResultWrapper<List<UsuarioDto>>> GetAllUsuariosAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        var dtos = usuarios.Select(u => new UsuarioDto
        {
            Id = u.Id,
            Username = u.Username,
            IsAdmin = u.IsAdmin,
            IdMotorista = u.IdMotorista,
            Ativo = u.Ativo
        }).ToList();

        return ResultWrapper<List<UsuarioDto>>.Ok(dtos);
    }

    // ativa ou desativa o acesso de um usuário
    public async Task<ResultWrapper<bool>> UpdateAtivoAsync(int id, bool ativo)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
        {
            return ResultWrapper<bool>.Fail("Usuário não encontrado.", $"Nenhum usuário com o ID {id}.");
        }

        usuario.Ativo = ativo;
        await _usuarioRepository.UpdateAsync(usuario);

        return ResultWrapper<bool>.Ok(true, "Status do usuário atualizado com sucesso.");
    }
}
