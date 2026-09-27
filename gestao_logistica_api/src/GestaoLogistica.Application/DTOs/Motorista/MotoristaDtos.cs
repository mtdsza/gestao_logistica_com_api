namespace GestaoLogistica.Application.DTOs.Motorista;

// representação de um motorista nas respostas da api
public class MotoristaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cnh { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? IdTransportadora { get; set; }
    public string? NomeTransportadora { get; set; }
}

// dados de entrada para cadastro de motorista
public class CreateMotoristaDto
{
    // nome completo do motorista
    public string Nome { get; set; } = string.Empty;

    // número de cnh válido e único
    public string Cnh { get; set; } = string.Empty;

    // status operacional inicial (padrão: 'disponivel')
    public string Status { get; set; } = "Disponivel";

    // transportadora parceira vinculada ou nulo para autônomo/frota própria
    public int? IdTransportadora { get; set; }
}

// dados de entrada para alteração de motorista
public class UpdateMotoristaDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cnh { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? IdTransportadora { get; set; }
}
