namespace GestaoLogistica.Application.DTOs.Auth;

// dados recebidos na requisição de login
public class LoginRequestDto
{
    // nome de usuário digitado
    public string Username { get; set; } = string.Empty;

    // senha em texto puro digitada pelo usuário (será comparada com o hash bcrypt no servidor)
    public string Password { get; set; } = string.Empty;
}

// dados devolvidos após autenticação bem-sucedida
public class LoginResponseDto
{
    // token de acesso assinado contendo as claims do usuário (bearer token)
    public string Token { get; set; } = string.Empty;

    // identificador do usuário no sistema
    public int Id { get; set; }

    // nome de login do usuário autenticado
    public string Username { get; set; } = string.Empty;

    // perfil administrativo (true: administrador com acesso total, false: motorista)
    public bool IsAdmin { get; set; }

    // identificador do motorista vinculado, quando o perfil for motorista
    public int? IdMotorista { get; set; }

    // data e hora utc de expiração do token jwt emitido
    public DateTime Expiration { get; set; }
}

// dados recebidos para cadastro de novo usuário
public class RegisterRequestDto
{
    // nome de login desejado (único no sistema)
    public string Username { get; set; } = string.Empty;

    // senha em texto puro que será criptografada com salt seguro antes de persistir
    public string Password { get; set; } = string.Empty;

    // perfil de acesso inicial do usuário
    public bool IsAdmin { get; set; } = true;

    // chave opcional para vincular o novo usuário a um motorista existente
    public int? IdMotorista { get; set; }
}

// representação de um usuário em consultas administrativas
public class UsuarioDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public int? IdMotorista { get; set; }
    public bool Ativo { get; set; }
}

// requisição para alterar a situação de ativação de uma conta
public class UpdateAtivoDto
{
    // novo estado: true para habilitar login, false para suspender o acesso
    public bool Ativo { get; set; }
}
