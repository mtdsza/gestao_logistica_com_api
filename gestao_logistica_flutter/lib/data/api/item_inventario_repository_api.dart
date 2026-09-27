import 'package:gestao_logistica/data/api/api_client.dart';
import 'package:gestao_logistica/domain/entities/item_inventario.dart';
import 'package:gestao_logistica/domain/repository/item_inventario_repository.dart';

// implementação do repositório de estoque e inventário consumindo a api asp.net core
// comunica-se com os endpoints que possuem política de cache em memória e invalidação reativa
class ItemInventarioRepositoryApi implements IItemInventarioRepository {
  // cliente http compartilhado
  final ApiClient _client = ApiClient();

  // mapeia os campos retornados em json para a entidade iteminventario de domínio
  ItemInventario _fromMap(Map<String, dynamic> map) {
    return ItemInventario(
      id: map['id'] as int?,
      nome: (map['nome'] ?? '') as String,
      tipo: (map['tipo'] ?? '') as String,
      pesoUnitario: ((map['pesoUnitario'] ?? map['peso_unitario'] ?? 0.0) as num).toDouble(),
      quantidade: ((map['quantidade'] ?? 0) as num).toInt(),
      cidade: (map['cidade'] ?? '') as String,
      dataValidade: map['dataValidade'] as String? ?? map['data_validade'] as String?,
      localizacao: map['localizacao'] as String?,
    );
  }

  // recupera todos os itens do inventário (com benefício de resposta via cache hit no backend)
  @override
  Future<List<ItemInventario>> findAll() async {
    final data = await _client.get('/itens-inventario');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // busca um item de inventário pelo seu identificador primário
  @override
  Future<ItemInventario?> findById(int id) async {
    final data = await _client.get('/itens-inventario/$id');
    if (data is Map<String, dynamic>) {
      return _fromMap(data);
    }
    return null;
  }

  // cadastra um novo item no inventário (disparando invalidação do cache de listagem na api)
  @override
  Future<void> insert(ItemInventario item) async {
    await _client.post('/itens-inventario', {
      'nome': item.nome,
      'tipo': item.tipo,
      'pesoUnitario': item.pesoUnitario,
      'quantidade': item.quantidade,
      'cidade': item.cidade,
      'dataValidade': item.dataValidade,
      'localizacao': item.localizacao,
    });
  }

  // atualiza dados de um item existente (invalidando o cache correspondente no backend)
  @override
  Future<void> update(ItemInventario item) async {
    await _client.put('/itens-inventario/${item.id}', {
      'nome': item.nome,
      'tipo': item.tipo,
      'pesoUnitario': item.pesoUnitario,
      'quantidade': item.quantidade,
      'cidade': item.cidade,
      'dataValidade': item.dataValidade,
      'localizacao': item.localizacao,
    });
  }

  // remove o item do estoque na api
  @override
  Future<void> delete(ItemInventario item) async {
    await _client.delete('/itens-inventario/${item.id}');
  }

  // realiza atualização pontual de quantidade em estoque via endpoint patch
  @override
  Future<void> updateStock(int id, int novaQuantidade) async {
    await _client.patch('/itens-inventario/$id/estoque', {
      'quantidade': novaQuantidade,
    });
  }
}

