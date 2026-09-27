// entidade de domínio que representa mercadorias e produtos no estoque dos centros de distribuição
class ItemInventario {
  // identificador único do item
  final int? id;
  // nome descritivo do produto ou material
  final String nome;
  // categoria ou classificação do produto (ex: Perecivel, Eletronico, Quimico)
  final String tipo;
  // peso individual unitário em quilogramas (kg)
  final double pesoUnitario;
  // saldo atual de unidades disponíveis em estoque
  final int quantidade;
  // centro de distribuição ou cidade onde o item está estocado
  final String cidade;
  // data limite de validade do produto (para itens perecíveis)
  final String? dataValidade;
  // corredor, estante ou endereço físico no armazém
  final String? localizacao;

  ItemInventario({
    this.id,
    required this.nome,
    required this.tipo,
    required this.pesoUnitario,
    required this.quantidade,
    required this.cidade,
    this.dataValidade,
    this.localizacao,
  });

  // converte a entidade para mapa compatível com persistência local em sqlite
  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'nome': nome,
      'tipo': tipo,
      'peso_unitario': pesoUnitario,
      'quantidade': quantidade,
      'cidade': cidade,
      'data_validade': dataValidade,
      'localizacao': localizacao,
    };
  }

  // instancia a entidade a partir de um mapa de dados
  factory ItemInventario.fromMap(Map<String, dynamic> map) {
    return ItemInventario(
      id: map['id'] as int?,
      nome: map['nome'] as String,
      tipo: map['tipo'] as String,
      pesoUnitario: (map['peso_unitario'] as num).toDouble(),
      quantidade: map['quantidade'] as int,
      cidade: map['cidade'] as String,
      dataValidade: map['data_validade'] as String?,
      localizacao: map['localizacao'] as String?,
    );
  }
}