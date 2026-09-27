namespace GestaoLogistica.Domain.Entities;

// representa uma operação de viagem de frete e transporte de mercadorias
public class Viagem
{
    // identificador único da viagem
    public int Id { get; set; }

    // chave estrangeira do motorista escalado para conduzir o transporte
    public int IdMotorista { get; set; }

    // chave estrangeira do veículo selecionado para carregar os itens
    public int IdVeiculo { get; set; }

    // chave estrangeira opcional da transportadora responsável pela execução
    public int? IdTransportadora { get; set; }

    // cidade de origem do frete
    public string Origem { get; set; } = string.Empty;

    // cidade de destino final da entrega
    public string Destino { get; set; } = string.Empty;

    // custo financeiro total acordado para o transporte (em reais)
    public decimal CustoTransporte { get; set; }

    // estado do ciclo de vida da viagem: 'pendente', 'em transito', 'atrasado' ou 'concluido'
    public string Status { get; set; } = "Pendente";

    // somatório do peso em kg de todas as mercadorias transportadas
    public decimal PesoTotal { get; set; }

    // data prevista para a saída ou conclusão da viagem (yyyy-mm-dd)
    public string DataPlanejada { get; set; } = string.Empty;

    // indica se o frete já foi financeiramente faturado pela empresa
    public bool Faturado { get; set; }

    // navegações relacionais para entidades do domínio
    public Motorista Motorista { get; set; } = null!;
    public Veiculo Veiculo { get; set; } = null!;
    public Transportadora? Transportadora { get; set; }

    // itens de estoque vinculados a esta viagem
    public ICollection<ViagemItem> Itens { get; set; } = new List<ViagemItem>();
}
