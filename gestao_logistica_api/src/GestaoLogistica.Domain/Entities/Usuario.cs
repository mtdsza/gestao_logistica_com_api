namespace GestaoLogistica.Domain.Entities;

// representa o usuário do sistema para autenticação e autorização
public class Usuario
{
    // identificador único do usuário gerado pelo banco
    public int Id { get; set; }

    // nome de login único utilizado para autenticação no sistema
    public string Username { get; set; } = string.Empty;

    // hash da senha criptografada com salt seguro via bcrypt (nunca salvo em texto puro)
    public string PasswordHash { get; set; } = string.Empty;

    // indica se o usuário possui privilégios de administrador ou apenas de motorista
    public bool IsAdmin { get; set; } = true;

    // chave estrangeira opcional associando o usuário a um motorista cadastrado
    public int? IdMotorista { get; set; }

    // indica se a conta do usuário está ativa para realizar login
    public bool Ativo { get; set; } = true;

    // referência de navegação para o perfil do motorista vinculado
    public Motorista? Motorista { get; set; }
}
