using GestaoLogistica.Application.Common.Interfaces;
using GestaoLogistica.Application.DTOs.Viagem;
using GestaoLogistica.Application.Services;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using Xunit;

namespace GestaoLogistica.Tests;

// testes automatizados para validar as regras operacionais de viagens
public class ViagemBusinessRulesTests
{
    private class FakeCacheService : IAppCacheService
    {
        public Task<T?> GetAsync<T>(string key) => Task.FromResult<T?>(default);
        public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null) => Task.CompletedTask;
        public Task RemoveAsync(string key) => Task.CompletedTask;
    }

    private class FakeMotoristaRepo : IMotoristaRepository
    {
        public Motorista Motorista { get; set; } = new() { Id = 1, Nome = "Motorista 1", Cnh = "111", Status = "Disponivel" };
        public Task<List<Motorista>> GetAllAsync() => Task.FromResult(new List<Motorista> { Motorista });
        public Task<Motorista?> GetByIdAsync(int id) => Task.FromResult(id == Motorista.Id ? Motorista : null);
        public Task<Motorista?> GetByCnhAsync(string cnh) => Task.FromResult<Motorista?>(null);
        public Task<List<Motorista>> GetAvailableAsync() => Task.FromResult(Motorista.Status == "Disponivel" ? new List<Motorista> { Motorista } : new List<Motorista>());
        public Task AddAsync(Motorista m) => Task.CompletedTask;
        public Task UpdateAsync(Motorista m) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }

    private class FakeVeiculoRepo : IVeiculoRepository
    {
        public Veiculo Veiculo { get; set; } = new() { Id = 1, Placa = "AAA-1111", Tipo = "Fiorino", CapacidadeCarga = 500m, Status = "Disponivel" };
        public Task<List<Veiculo>> GetAllAsync() => Task.FromResult(new List<Veiculo> { Veiculo });
        public Task<Veiculo?> GetByIdAsync(int id) => Task.FromResult(id == Veiculo.Id ? Veiculo : null);
        public Task<Veiculo?> GetByPlacaAsync(string placa) => Task.FromResult<Veiculo?>(null);
        public Task<List<Veiculo>> GetAvailableAsync() => Task.FromResult(Veiculo.Status == "Disponivel" ? new List<Veiculo> { Veiculo } : new List<Veiculo>());
        public Task AddAsync(Veiculo v) => Task.CompletedTask;
        public Task UpdateAsync(Veiculo v) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }

    private class FakeTransportadoraRepo : ITransportadoraRepository
    {
        public Task<List<Transportadora>> GetAllAsync() => Task.FromResult(new List<Transportadora>());
        public Task<Transportadora?> GetByIdAsync(int id) => Task.FromResult<Transportadora?>(null);
        public Task<Transportadora?> GetByCnpjAsync(string cnpj) => Task.FromResult<Transportadora?>(null);
        public Task AddAsync(Transportadora t) => Task.CompletedTask;
        public Task UpdateAsync(Transportadora t) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }

    private class FakeItemRepo : IItemInventarioRepository
    {
        public ItemInventario Item { get; set; } = new() { Id = 1, Nome = "Item 1", Tipo = "Geral", PesoUnitario = 10m, Quantidade = 10, Cidade = "Curitiba" };
        public Task<List<ItemInventario>> GetAllAsync() => Task.FromResult(new List<ItemInventario> { Item });
        public Task<ItemInventario?> GetByIdAsync(int id) => Task.FromResult(id == Item.Id ? Item : null);
        public Task AddAsync(ItemInventario i) => Task.CompletedTask;
        public Task UpdateAsync(ItemInventario i) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
        public Task UpdateStockAsync(int id, int novaQuantidade) { Item.Quantidade = novaQuantidade; return Task.CompletedTask; }
    }

    private class FakeViagemRepo : IViagemRepository
    {
        public List<Viagem> Viagens { get; } = new();
        public Task<List<Viagem>> GetAllAsync() => Task.FromResult(Viagens.ToList());
        public Task<Viagem?> GetByIdAsync(int id) => Task.FromResult(Viagens.FirstOrDefault(v => v.Id == id));
        public Task<List<Viagem>> GetByMotoristaAsync(int idMotorista) => Task.FromResult(Viagens.Where(v => v.IdMotorista == idMotorista).ToList());
        public Task<List<Viagem>> GetHistoryByMotoristaAsync(int idMotorista) => Task.FromResult(Viagens.Where(v => v.IdMotorista == idMotorista).ToList());
        public Task<List<Viagem>> GetHistoryByVeiculoAsync(int idVeiculo) => Task.FromResult(Viagens.Where(v => v.IdVeiculo == idVeiculo).ToList());
        public Task<List<Viagem>> GetHistoryByTransportadoraAsync(int idTransportadora) => Task.FromResult(Viagens.Where(v => v.IdTransportadora == idTransportadora).ToList());
        public Task<decimal> GetFaturamentoTotalAsync() => Task.FromResult(Viagens.Where(v => v.Status == "Concluido").Sum(v => v.CustoTransporte));
        public Task<decimal> GetEficienciaNoPrazoAsync() => Task.FromResult(100m);
        public Task<int> GetViagensEmTransitoAsync() => Task.FromResult(Viagens.Count(v => v.Status == "Em transito"));
        public Task AddAsync(Viagem viagem, List<(int ItemId, int Quantidade)> itens)
        {
            viagem.Id = Viagens.Count + 1;
            Viagens.Add(viagem);
            return Task.CompletedTask;
        }
        public Task UpdateStatusAsync(int id, string novoStatus) => Task.CompletedTask;
        public Task UpdateFaturadoAsync(int id, bool faturado) => Task.CompletedTask;
    }

    // valida que a viagem é rejeitada quando a carga excede a capacidade do veículo
    [Fact]
    public async Task CreateViagem_ShouldFail_WhenCargoWeightExceedsVehicleCapacity()
    {
        var motoristaRepo = new FakeMotoristaRepo();
        var veiculoRepo = new FakeVeiculoRepo(); // capacidade do veículo = 500 kg
        var transpRepo = new FakeTransportadoraRepo();
        var itemRepo = new FakeItemRepo();
        var viagemRepo = new FakeViagemRepo();
        var cache = new FakeCacheService();

        var service = new ViagemService(viagemRepo, motoristaRepo, veiculoRepo, transpRepo, itemRepo, cache);

        var request = new CreateViagemDto
        {
            IdMotorista = 1,
            IdVeiculo = 1,
            Origem = "Curitiba",
            Destino = "Sao Paulo",
            CustoTransporte = 1000m,
            PesoTotal = 800m // 800 kg excede a capacidade de 500 kg
        };

        var result = await service.CreateAsync(request);

        // a operação deve falhar e informar mensagem clara
        Assert.False(result.Success);
        Assert.Contains("capacidade máxima", result.Errors.First());
    }

    // valida que a viagem é rejeitada quando o motorista já está em outra viagem
    [Fact]
    public async Task CreateViagem_ShouldFail_WhenDriverIsUnavailable()
    {
        var motoristaRepo = new FakeMotoristaRepo();
        motoristaRepo.Motorista.Status = "Em Viagem"; // motorista ocupado

        var veiculoRepo = new FakeVeiculoRepo();
        var transpRepo = new FakeTransportadoraRepo();
        var itemRepo = new FakeItemRepo();
        var viagemRepo = new FakeViagemRepo();
        var cache = new FakeCacheService();

        var service = new ViagemService(viagemRepo, motoristaRepo, veiculoRepo, transpRepo, itemRepo, cache);

        var request = new CreateViagemDto
        {
            IdMotorista = 1,
            IdVeiculo = 1,
            Origem = "Curitiba",
            Destino = "Sao Paulo",
            CustoTransporte = 1000m,
            PesoTotal = 300m
        };

        var result = await service.CreateAsync(request);

        Assert.False(result.Success);
        Assert.Contains("indisponível", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
