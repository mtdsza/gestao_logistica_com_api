using GestaoLogistica.Domain.Services;
using Xunit;

namespace GestaoLogistica.Tests;

// testes unitários para as regras matemáticas de distância e frete
public class FreightCalculationTests
{
    // valida se a distância geodésica entre curitiba e são paulo condiz com a realidade (~338 km)
    [Fact]
    public void Distancia_Curitiba_SaoPaulo_ShouldBeApproximately338Km()
    {
        double distancia = DistanciaService.CalcularDistanciaEmKm("Curitiba", "Sao Paulo");

        // distância em linha reta na superfície da terra calculada pela fórmula de haversine
        Assert.InRange(distancia, 330.0, 350.0);
    }

    // valida a fórmula de precificação do frete: (distância * tarifa_km) + (peso * tarifa_peso)
    [Fact]
    public void FreteSugerido_ShouldCalculateBasedOnDistanceAndWeight()
    {
        string origem = "Curitiba";
        string destino = "Sao Paulo";
        decimal pesoTotal = 1000m; // 1000 kg de carga

        decimal frete = DistanciaService.CalcularFreteSugerido(origem, destino, pesoTotal, custoKm: 3.00m, custoPeso: 0.15m);

        double distancia = DistanciaService.CalcularDistanciaEmKm(origem, destino);
        decimal esperado = Math.Round((decimal)distancia * 3.00m + 1000m * 0.15m, 2);

        Assert.Equal(esperado, frete);
        Assert.True(frete > 0);
    }
}
