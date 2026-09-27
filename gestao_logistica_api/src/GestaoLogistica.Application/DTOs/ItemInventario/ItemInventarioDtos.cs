namespace GestaoLogistica.Application.DTOs.ItemInventario;

// representação de um item de estoque nas respostas da api
public class ItemInventarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal PesoUnitario { get; set; }
    public int Quantidade { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string? DataValidade { get; set; }
    public string? Localizacao { get; set; }
}

// dados de entrada para inclusão de novo produto no estoque
public class CreateItemInventarioDto
{
    // descrição do produto
    public string Nome { get; set; } = string.Empty;

    // tipo da mercadoria ('perecivel', 'eletronico', 'fragil', 'geral')
    public string Tipo { get; set; } = string.Empty;

    // peso de uma unidade em quilogramas (maior que zero)
    public decimal PesoUnitario { get; set; }

    // quantidade de peças inicialmente estocadas
    public int Quantidade { get; set; }

    // cidade do galpão ou centro de distribuição
    public string Cidade { get; set; } = string.Empty;

    // data de validade opcional
    public string? DataValidade { get; set; }

    // identificador do local físico no galpão
    public string? Localizacao { get; set; }
}

// dados para edição completa do produto
public class UpdateItemInventarioDto
{
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal PesoUnitario { get; set; }
    public int Quantidade { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string? DataValidade { get; set; }
    public string? Localizacao { get; set; }
}

// requisição de ajuste rápido de quantidade em estoque (patch)
public class UpdateStockDto
{
    // nova quantidade total disponível em estoque
    public int Quantidade { get; set; }
}
