// entidade de domínio que representa a ordem de transporte e viagem de carga
class Viagem {
  // identificador único da viagem
  final int? id;
  // chave estrangeira do motorista escalado
  final int idMotorista;
  // chave estrangeira do veículo alocado
  final int idVeiculo;
  // chave estrangeira da transportadora operadora (se terceirizada)
  final int? idTransportadora;
  // centro de distribuição ou cidade de origem da viagem
  final String origem;
  // cidade de destino final da entrega
  final String destino;
  // valor total de frete calculado para a viagem
  final double custoTransporte;
  // situação do ciclo de vida ('Pendente', 'Em Transito', 'Concluida', 'Cancelada')
  final String status;
  // soma do peso de todas as mercadorias embarcadas em kg
  final double pesoTotal;
  // data prevista para a realização do transporte (yyyy-MM-dd)
  final String dataPlanejada;
  // indica se a viagem já teve sua cobrança faturada financeiramente
  final bool faturado;

  Viagem({
    this.id,
    required this.idMotorista,
    required this.idVeiculo,
    this.idTransportadora,
    required this.origem,
    required this.destino,
    required this.custoTransporte,
    required this.status,
    required this.pesoTotal,
    required this.dataPlanejada,
    required this.faturado,
  });

  // converte a entidade para mapa compatível com persistência local em sqlite
  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'id_motorista': idMotorista,
      'id_veiculo': idVeiculo,
      'id_transportadora': idTransportadora,
      'origem': origem,
      'destino': destino,
      'custo_transporte': custoTransporte,
      'status': status,
      'peso_total': pesoTotal,
      'data_planejada': dataPlanejada,
      'faturado': faturado ? 1 : 0,
    };
  }

  // instancia a entidade a partir de um mapa de dados
  factory Viagem.fromMap(Map<String, dynamic> map) {
    return Viagem(
      id: map['id'] as int?,
      idMotorista: map['id_motorista'] as int,
      idVeiculo: map['id_veiculo'] as int,
      idTransportadora: map['id_transportadora'] as int?,
      origem: map['origem'] as String,
      destino: map['destino'] as String,
      custoTransporte: (map['custo_transporte'] as num).toDouble(),
      status: map['status'] as String,
      pesoTotal: (map['peso_total'] as num).toDouble(),
      dataPlanejada: map['data_planejada'] as String,
      faturado: (map['faturado'] as int) == 1,
    );
  }
}