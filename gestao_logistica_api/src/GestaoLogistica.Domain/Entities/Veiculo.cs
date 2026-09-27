namespace GestaoLogistica.Domain.Entities;

// representa um veículo de transporte da frota própria ou terceirizada
public class Veiculo
{
    // identificador único do veículo
    public int Id { get; set; }

    // placa de identificação veicular única
    public string Placa { get; set; } = string.Empty;

    // categoria do veículo: 'carreta', 'caminhao bau' ou 'fiorino'
    public string Tipo { get; set; } = string.Empty;

    // peso máximo de carga suportado pelo veículo em quilogramas (usado na validação de excesso)
    public decimal CapacidadeCarga { get; set; }

    // situação operacional do veículo: 'disponivel', 'manutencao' ou 'em viagem'
    public string Status { get; set; } = "Disponivel";

    // chave estrangeira indicando se o veículo pertence a uma transportadora parceira
    public int? IdTransportadora { get; set; }

    // dados da transportadora parceira proprietária do veículo
    public Transportadora? Transportadora { get; set; }

    // histórico de viagens realizadas pelo veículo
    public ICollection<Viagem> Viagens { get; set; } = new List<Viagem>();
}
