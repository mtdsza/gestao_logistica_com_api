// entidade de domínio que representa o motorista responsável pelas viagens
class Motorista {
  // identificador único do motorista
  final int? id;
  // nome completo do condutor
  final String nome;
  // número da carteira nacional de habilitação (cnh)
  final String cnh;
  // situação cadastral e operacional ('Disponivel', 'Em Viagem', 'Inativo')
  final String status;
  // chave estrangeira referenciando a transportadora vinculada
  final int? idTransportadora;

  Motorista({
    this.id,
    required this.nome,
    required this.cnh,
    required this.status,
    this.idTransportadora,
  });

  // converte a entidade para mapa compatível com persistência local em sqlite
  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'nome': nome,
      'cnh': cnh,
      'status': status,
      'id_transportadora': idTransportadora,
    };
  }

  // instancia a entidade a partir de um mapa de dados
  factory Motorista.fromMap(Map<String, dynamic> map) {
    return Motorista(
      id: map['id'] as int?,
      nome: map['nome'] as String,
      cnh: map['cnh'] as String,
      status: map['status'] as String,
      idTransportadora: map['id_transportadora'] as int?,
    );
  }
}