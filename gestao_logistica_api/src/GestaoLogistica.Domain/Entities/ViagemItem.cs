namespace GestaoLogistica.Domain.Entities;

// entidade de junção com chave composta representando a quantidade de um item transportada em uma viagem
public class ViagemItem
{
    // chave primária composta: identificador da viagem
    public int IdViagem { get; set; }

    // chave primária composta: identificador do item de estoque transportado
    public int IdItem { get; set; }

    // quantidade de unidades do produto que foram embarcadas nesta viagem
    public int QuantidadeTransportada { get; set; }

    // navegações relacionais para a viagem e para o item de estoque correspondente
    public Viagem Viagem { get; set; } = null!;
    public ItemInventario Item { get; set; } = null!;
}
