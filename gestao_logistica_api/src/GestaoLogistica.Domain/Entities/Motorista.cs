namespace GestaoLogistica.Domain.Entities;

// representa o motorista responsável pelo transporte das cargas
public class Motorista
{
    // identificador único do motorista
    public int Id { get; set; }

    // nome completo do motorista
    public string Nome { get; set; } = string.Empty;

    // número da carteira nacional de habilitação (cnh única no sistema)
    public string Cnh { get; set; } = string.Empty;

    // situação operacional atual do motorista: 'disponivel', 'em viagem' ou 'inativo'
    public string Status { get; set; } = "Disponivel";

    // chave estrangeira opcional indicando a transportadora vinculada (nulo se for frota própria)
    public int? IdTransportadora { get; set; }

    // dados da transportadora parceira associada
    public Transportadora? Transportadora { get; set; }

    // conta de usuário utilizada pelo motorista para acessar o aplicativo
    public Usuario? Usuario { get; set; }

    // histórico e viagens atribuídas ao motorista
    public ICollection<Viagem> Viagens { get; set; } = new List<Viagem>();
}
