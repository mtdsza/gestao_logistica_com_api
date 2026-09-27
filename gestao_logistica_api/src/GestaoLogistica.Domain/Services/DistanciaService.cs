namespace GestaoLogistica.Domain.Services;

// representa as coordenadas geográficas de latitude e longitude de um ponto no mapa
public record Coordenada(double Latitude, double Longitude);

// serviço de domínio responsável pelos cálculos geodésicos e precificação sugerida de fretes
public static class DistanciaService
{
    // custo base padrão cobrado por quilômetro percorrido (r$ 3,00/km)
    public const decimal CustoPadraoPorKm = 3.00m;

    // custo base padrão cobrado por quilograma transportado (r$ 0,15/kg)
    public const decimal CustoPadraoPorKg = 0.15m;

    // catálogo fixo com as coordenadas geográficas das capitais operadas pelo sistema
    public static readonly IReadOnlyDictionary<string, Coordenada> Cidades = new Dictionary<string, Coordenada>(StringComparer.OrdinalIgnoreCase)
    {
        { "Belo Horizonte", new Coordenada(-19.9167, -43.9345) },
        { "Brasilia", new Coordenada(-15.7938, -47.8827) },
        { "Curitiba", new Coordenada(-25.4284, -49.2733) },
        { "Porto Alegre", new Coordenada(-30.0346, -51.2177) },
        { "Rio de Janeiro", new Coordenada(-22.9068, -43.1729) },
        { "Sao Paulo", new Coordenada(-23.5505, -46.6333) }
    };

    // calcula a distância em linha reta entre duas capitais usando a fórmula geodésica de haversine
    // a fórmula considera a curvatura da terra com raio médio r = 6371 km
    // etapas do cálculo:
    // 1. converte as variações de latitude e longitude de graus para radianos
    // 2. calcula o termo 'a' (seno ao quadrado da metade das variações angulares somado ao produto dos cossenos)
    // 3. calcula a distância angular 'c' em radianos usando arco-tangente 2
    // 4. multiplica o raio terrestre pelo arco 'c' para obter a distância final em quilômetros
    public static double CalcularDistanciaEmKm(string origem, string destino)
    {
        // verifica se ambas as cidades possuem coordenadas cadastradas no catálogo
        if (!Cidades.TryGetValue(origem, out var coordOrigem) ||
            !Cidades.TryGetValue(destino, out var coordDestino))
        {
            return 0.0;
        }

        const double r = 6371; // raio esférico médio da terra em km

        // converte as variações angulares para radianos
        double dLat = (coordDestino.Latitude - coordOrigem.Latitude) * Math.PI / 180.0;
        double dLon = (coordDestino.Longitude - coordOrigem.Longitude) * Math.PI / 180.0;

        // converte as latitudes dos dois pontos para radianos
        double lat1Rad = coordOrigem.Latitude * Math.PI / 180.0;
        double lat2Rad = coordDestino.Latitude * Math.PI / 180.0;

        // aplica a fórmula de haversine para calcular o quadrado da metade da distância da corda
        double a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2.0) * Math.Sin(dLon / 2.0);

        // calcula a distância angular em radianos
        double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

        // retorna a distância real na superfície terrestre em quilômetros
        return r * c;
    }

    // calcula o valor monetário sugerido para o frete com base na distância percorrida e no peso da carga
    // fórmula: (distancia_km * tarifa_km) + (peso_kg * tarifa_peso)
    // permite customizar as tarifas caso a viagem utilize uma transportadora específica
    public static decimal CalcularFreteSugerido(
        string origem,
        string destino,
        decimal pesoTotal,
        decimal? custoKm = null,
        decimal? custoPeso = null)
    {
        double distancia = CalcularDistanciaEmKm(origem, destino);
        if (distancia <= 0) return 0m;

        // define as tarifas aplicáveis (customizadas da transportadora ou padrão do sistema)
        decimal kmRate = custoKm ?? CustoPadraoPorKm;
        decimal pesoRate = custoPeso ?? CustoPadraoPorKg;

        // calcula a parcela da distância e a parcela do peso
        decimal custoDistancia = (decimal)distancia * kmRate;
        decimal custoCarga = pesoTotal * pesoRate;

        // arredonda para duas casas decimais
        return Math.Round(custoDistancia + custoCarga, 2);
    }
}
