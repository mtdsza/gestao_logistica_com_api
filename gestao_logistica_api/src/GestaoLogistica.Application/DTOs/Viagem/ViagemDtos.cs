namespace GestaoLogistica.Application.DTOs.Viagem;

// detalhe de um item vinculado a uma viagem na resposta
public class ViagemItemDto
{
    public int IdItem { get; set; }
    public string? NomeItem { get; set; }
    public string? TipoItem { get; set; }
    public decimal PesoUnitario { get; set; }
    public int QuantidadeTransportada { get; set; }
}

// dados detalhados da viagem retornados para o cliente
public class ViagemDto
{
    public int Id { get; set; }
    public int IdMotorista { get; set; }
    public string? NomeMotorista { get; set; }
    public int IdVeiculo { get; set; }
    public string? PlacaVeiculo { get; set; }
    public int? IdTransportadora { get; set; }
    public string? NomeTransportadora { get; set; }
    public string Origem { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public decimal CustoTransporte { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal PesoTotal { get; set; }
    public string DataPlanejada { get; set; } = string.Empty;
    public bool Faturado { get; set; }

    // lista de itens embarcados nesta viagem com suas respectivas quantidades
    public List<ViagemItemDto> Itens { get; set; } = new();
}

// especificação de um item a ser embarcado na criação da viagem
public class CreateViagemItemDto
{
    public int IdItem { get; set; }
    public int QuantidadeTransportada { get; set; }
}

// dados de entrada para criação de uma nova viagem
public class CreateViagemDto
{
    public int IdMotorista { get; set; }
    public int IdVeiculo { get; set; }
    public int? IdTransportadora { get; set; }
    public string Origem { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public decimal CustoTransporte { get; set; }
    public string Status { get; set; } = "Pendente";
    public decimal PesoTotal { get; set; }
    public string DataPlanejada { get; set; } = string.Empty;
    public bool Faturado { get; set; } = false;

    // itens do inventário que terão baixa de estoque reservada nesta viagem
    public List<CreateViagemItemDto> Itens { get; set; } = new();
}

// requisição para avançar ou alterar a etapa de uma viagem
public class UpdateViagemStatusDto
{
    // novo status desejado ('pendente', 'em transito', 'atrasado', 'concluido')
    public string Status { get; set; } = string.Empty;
}

// requisição para atualizar o estado financeiro de faturamento
public class UpdateViagemFaturadoDto
{
    public bool Faturado { get; set; }
}

// dados de entrada para calcular previamente o frete antes de salvar
public class CalcularFreteRequestDto
{
    public string Origem { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public decimal PesoTotal { get; set; }
    public int? IdTransportadora { get; set; }
}

// resposta contendo a quilometragem calculada e o valor sugerido do frete
public class CalcularFreteResponseDto
{
    public double DistanciaKm { get; set; }
    public decimal CustoSugerido { get; set; }
    public decimal CustoKmUtilizado { get; set; }
    public decimal CustoPesoUtilizado { get; set; }
}

// métricas gerenciais consolidadas para a tela principal (dashboard)
public class DashboardMetricasDto
{
    public decimal FaturamentoTotal { get; set; }
    public decimal EficienciaNoPrazo { get; set; }
    public int ViagensEmTransito { get; set; }
    public int TotalViagens { get; set; }
}
