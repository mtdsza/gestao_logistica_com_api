// entidade de domínio que representa uma transportadora parceira ou própria
class Transportadora {
  // identificador único da transportadora
  final int? id;
  // cadastro nacional da pessoa jurídica (cnpj)
  final String cnpj;
  // razão social ou nome comercial da transportadora
  final String nomeFantasia;
  // custo cobrado por quilômetro rodado
  final double custoKm;
  // custo cobrado por quilograma de carga transportada
  final double custoPeso;

  Transportadora({
    this.id,
    required this.cnpj,
    required this.nomeFantasia,
    this.custoKm = 3.00,
    this.custoPeso = 0.15,
  });

  // converte a entidade para mapa compatível com persistência local em sqlite
  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'cnpj': cnpj,
      'nome_fantasia': nomeFantasia,
      'custo_km': custoKm,
      'custo_peso': custoPeso,
    };
  }

  // instancia a entidade a partir de um mapa de dados
  factory Transportadora.fromMap(Map<String, dynamic> map) {
    return Transportadora(
      id: map['id'] as int?,
      cnpj: map['cnpj'] as String,
      nomeFantasia: map['nome_fantasia'] as String,
      custoKm: (map['custo_km'] as num?)?.toDouble() ?? 3.00,
      custoPeso: (map['custo_peso'] as num?)?.toDouble() ?? 0.15,
    );
  }
}