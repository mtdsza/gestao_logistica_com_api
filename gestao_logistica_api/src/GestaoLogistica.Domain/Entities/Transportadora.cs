namespace GestaoLogistica.Domain.Entities;

// representa uma empresa transportadora parceira com tarifas contratuais customizadas
public class Transportadora
{
    // identificador único autoincrementado da transportadora
    public int Id { get; set; }

    // número de cnpj único da transportadora
    public string Cnpj { get; set; } = string.Empty;

    // nome fantasia ou razão social da empresa transportadora
    public string NomeFantasia { get; set; } = string.Empty;

    // tarifa cobrada por cada quilômetro rodado para cálculo do frete (padrão: r$ 3,00)
    public decimal CustoKm { get; set; } = 3.00m;

    // tarifa cobrada por cada quilograma de peso transportado (padrão: r$ 0,15)
    public decimal CustoPeso { get; set; } = 0.15m;

    // coleções de navegação representando motoristas, veículos e viagens vinculados
    public ICollection<Motorista> Motoristas { get; set; } = new List<Motorista>();
    public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
    public ICollection<Viagem> Viagens { get; set; } = new List<Viagem>();
}
