namespace GestaoLogistica.Application.DTOs.Veiculo;

// representação de um veículo nas respostas da api
public class VeiculoDto
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal CapacidadeCarga { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? IdTransportadora { get; set; }
    public string? NomeTransportadora { get; set; }
}

// dados de entrada para registrar um novo veículo
public class CreateVeiculoDto
{
    // placa do veículo (convertida para maiúsculas)
    public string Placa { get; set; } = string.Empty;

    // tipo do veículo ('carreta', 'caminhao bau', 'fiorino')
    public string Tipo { get; set; } = string.Empty;

    // capacidade máxima em kg suportada pela caçamba/baú
    public decimal CapacidadeCarga { get; set; }

    // status inicial do veículo (padrão: 'disponivel')
    public string Status { get; set; } = "Disponivel";

    // transportadora proprietária (nulo se pertencer à empresa central)
    public int? IdTransportadora { get; set; }
}

// dados de entrada para atualizar veículo
public class UpdateVeiculoDto
{
    public string Placa { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal CapacidadeCarga { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? IdTransportadora { get; set; }
}
