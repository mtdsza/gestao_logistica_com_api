import 'package:gestao_logistica/data/api/api_client.dart';
import 'package:gestao_logistica/domain/entities/motorista.dart';
import 'package:gestao_logistica/domain/repository/motorista_repository.dart';

// implementação do repositório de motoristas consumindo a api asp.net core
class MotoristaRepositoryApi implements IMotoristaRepository {
  // cliente http compartilhado
  final ApiClient _client = ApiClient();

  // desserializa o mapa retornado pela api para uma instância da entidade motorista
  Motorista _fromMap(Map<String, dynamic> map) {
    return Motorista(
      id: map['id'] as int?,
      nome: (map['nome'] ?? '') as String,
      cnh: (map['cnh'] ?? '') as String,
      status: (map['status'] ?? 'Disponivel') as String,
      idTransportadora: map['idTransportadora'] as int? ?? map['id_transportadora'] as int?,
    );
  }

  // recupera todos os motoristas cadastrados
  @override
  Future<List<Motorista>> findAll() async {
    final data = await _client.get('/motoristas');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // filtra e lista apenas motoristas com status 'disponivel' para alocação em viagens
  @override
  Future<List<Motorista>> findAvailable() async {
    final data = await _client.get('/motoristas/disponiveis');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // busca um motorista pelo seu identificador primário
  @override
  Future<Motorista?> findById(int id) async {
    final data = await _client.get('/motoristas/$id');
    if (data is Map<String, dynamic>) {
      return _fromMap(data);
    }
    return null;
  }

  // insere um novo motorista na base de dados
  @override
  Future<void> insert(Motorista motorista) async {
    await _client.post('/motoristas', {
      'nome': motorista.nome,
      'cnh': motorista.cnh,
      'status': motorista.status,
      'idTransportadora': motorista.idTransportadora,
    });
  }

  // atualiza as informações do motorista no servidor
  @override
  Future<void> update(Motorista motorista) async {
    await _client.put('/motoristas/${motorista.id}', {
      'nome': motorista.nome,
      'cnh': motorista.cnh,
      'status': motorista.status,
      'idTransportadora': motorista.idTransportadora,
    });
  }

  // remove o motorista do cadastro via api
  @override
  Future<void> delete(Motorista motorista) async {
    await _client.delete('/motoristas/${motorista.id}');
  }
}

