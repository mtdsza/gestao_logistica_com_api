import 'dart:math';

// estrutura de dados imutável para representar coordenadas geográficas (latitude e longitude em graus)
class Coordenada {
  final double latitude;
  final double longitude;

  const Coordenada(this.latitude, this.longitude);
}

// classe utilitária responsável pelo cálculo geodésico de distâncias e estimativa de frete
class DistanciaHelper {
  // taxa padrão cobrada por quilômetro percorrido
  static const double custoPorKm = 3.00; 
  // taxa padrão cobrada por quilograma de carga transportada
  static const double custoPorKg = 0.15;

  // dicionário com as coordenadas geográficas dos principais polos logísticos suportados
  static const Map<String, Coordenada> cidades = {
    'Belo Horizonte': Coordenada(-19.9167, -43.9345),
    'Brasilia': Coordenada(-15.7938, -47.8827),
    'Curitiba': Coordenada(-25.4284, -49.2733),
    'Porto Alegre': Coordenada(-30.0346, -51.2177),
    'Rio de Janeiro': Coordenada(-22.9068, -43.1729),
    'Sao Paulo': Coordenada(-23.5505, -46.6333),
  };

  // calcula a distância em quilômetros entre duas cidades utilizando a fórmula de haversine:
  // d = 2 * r * asin(sqrt(sin²(Δlat/2) + cos(lat1) * cos(lat2) * sin²(Δlon/2)))
  // onde r = 6371 km representa o raio médio da terra. converte graus em radianos multiplicando por pi/180
  static double calcularDistanciaEmKm(String origem, String destino) {
    final coordOrigem = cidades[origem];
    final coordDestino = cidades[destino];

    // se alguma das cidades não estiver mapeada, retorna zero
    if (coordOrigem == null || coordDestino == null) return 0.0;

    // raio médio da terra em quilômetros
    const r = 6371;

    // converte a diferença angular de latitude e longitude para radianos
    final dLat = (coordDestino.latitude - coordOrigem.latitude) * pi / 180;
    final dLon = (coordDestino.longitude - coordOrigem.longitude) * pi / 180;

    // converte as latitudes de origem e destino para radianos
    final lat1Rad = coordOrigem.latitude * pi / 180;
    final lat2Rad = coordDestino.latitude * pi / 180;

    // calcula o semi-seno verso da distância angular central (termo 'a' de haversine)
    final a = sin(dLat / 2) * sin(dLat / 2) +
        cos(lat1Rad) * cos(lat2Rad) *
        sin(dLon / 2) * sin(dLon / 2);
    
    // calcula a distância angular 'c' em radianos
    final c = 2 * atan2(sqrt(a), sqrt(1 - a));
    
    // multiplica pelo raio terrestre para obter a distância final em km
    return r * c;
  }

  // calcula o custo de frete sugerido considerando a distância percorrida e o peso total da carga
  static double calcularFreteSugerido(String origem, String destino, double pesoTotal) {
    final distancia = calcularDistanciaEmKm(origem, destino);
    if (distancia == 0.0) return 0.0;
    
    // parcela calculada sobre a distância total da rota
    final custoDistancia = distancia * custoPorKm;
    // parcela calculada sobre o peso dos itens embarcados
    final custoCarga = pesoTotal * custoPorKg;

    return custoDistancia + custoCarga;
  }
}