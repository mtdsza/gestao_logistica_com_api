namespace GestaoLogistica.Domain.Entities;

// representa um produto ou mercadoria armazenada no inventário/estoque
public class ItemInventario
{
    // identificador único do produto no estoque
    public int Id { get; set; }

    // nome descritivo do produto
    public string Nome { get; set; } = string.Empty;

    // classificação da mercadoria: 'perecivel', 'eletronico', 'fragil' ou 'geral'
    public string Tipo { get; set; } = string.Empty;

    // peso unitário do item em quilogramas (usado para calcular o peso total da carga)
    public decimal PesoUnitario { get; set; }

    // quantidade de unidades atualmente disponíveis no estoque
    public int Quantidade { get; set; }

    // cidade/centro de distribuição onde o produto está fisicamente estocado
    public string Cidade { get; set; } = string.Empty;

    // data de validade opcional para produtos perecíveis (formato yyyy-mm-dd)
    public string? DataValidade { get; set; }

    // descrição física do local de armazenagem (ex.: prateleira, setor ou galpão)
    public string? Localizacao { get; set; }

    // histórico de vinculação deste item a viagens de entrega
    public ICollection<ViagemItem> ViagemItens { get; set; } = new List<ViagemItem>();
}
