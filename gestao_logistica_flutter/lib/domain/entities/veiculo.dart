// entidade de domínio que representa os veículos da frota de transporte
class Veiculo {
  // identificador único do veículo
  final int? id;
  // placa de identificação veicular
  final String placa;
  // categoria ou modelo do veículo (ex: Carreta, Toco, Van, VUC)
  final String tipo;
  // capacidade máxima de carga suportada em kg
  final double capacidadeCarga;
  // disponibilidade operacional do veículo ('Disponivel', 'Em Manutencao', 'Em Viagem')
  final String status;
  // chave estrangeira referenciando a transportadora proprietária
  final int? idTransportadora;

  Veiculo({
    this.id,
    required this.placa,
    required this.tipo,
    required this.capacidadeCarga,
    required this.status,
    this.idTransportadora,
  });

  // converte a entidade para mapa compatível com persistência local em sqlite
  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'placa': placa,
      'tipo': tipo,
      'capacidade_carga': capacidadeCarga,
      'status': status,
      'id_transportadora': idTransportadora,
    };
  }

  // instancia a entidade a partir de um mapa de dados
  factory Veiculo.fromMap(Map<String, dynamic> map) {
    return Veiculo(
      id: map['id'] as int?,
      placa: map['placa'] as String,
      tipo: map['tipo'] as String,
      capacidadeCarga: (map['capacidade_carga'] as num).toDouble(),
      status: map['status'] as String,
      idTransportadora: map['id_transportadora'] as int?,
    );
  }
}