namespace GestaoLogistica.Application.DTOs.Transportadora;

// dados de saída representando uma transportadora cadastrada
public class TransportadoraDto
{
    public int Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public decimal CustoKm { get; set; }
    public decimal CustoPeso { get; set; }
}

// dados de entrada para registrar uma nova transportadora
public class CreateTransportadoraDto
{
    // cnpj obrigatório da empresa
    public string Cnpj { get; set; } = string.Empty;

    // nome de exibição ou razão social
    public string NomeFantasia { get; set; } = string.Empty;

    // tarifa por quilômetro (padrão r$ 3,00)
    public decimal CustoKm { get; set; } = 3.00m;

    // tarifa por quilograma de carga (padrão r$ 0,15)
    public decimal CustoPeso { get; set; } = 0.15m;
}

// dados de entrada para editar uma transportadora existente
public class UpdateTransportadoraDto
{
    public string Cnpj { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public decimal CustoKm { get; set; }
    public decimal CustoPeso { get; set; }
}
