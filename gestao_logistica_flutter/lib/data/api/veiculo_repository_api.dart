import 'package:gestao_logistica/data/api/api_client.dart';
import 'package:gestao_logistica/domain/entities/veiculo.dart';
import 'package:gestao_logistica/domain/repository/veiculo_repository.dart';

// implementação do repositório de veículos integrado à api asp.net core
class VeiculoRepositoryApi implements IVeiculoRepository {
  // cliente http compartilhado
  final ApiClient _client = ApiClient();

  // converte o mapa json retornado pela api para a entidade de domínio veiculo
  Veiculo _fromMap(Map<String, dynamic> map) {
    return Veiculo(
      id: map['id'] as int?,
      placa: (map['placa'] ?? '') as String,
      tipo: (map['tipo'] ?? '') as String,
      capacidadeCarga: ((map['capacidadeCarga'] ?? map['capacidade_carga'] ?? 0.0) as num).toDouble(),
      status: (map['status'] ?? 'Disponivel') as String,
      idTransportadora: map['idTransportadora'] as int? ?? map['id_transportadora'] as int?,
    );
  }

  // obtém a lista com todos os veículos cadastrados
  @override
  Future<List<Veiculo>> findAll() async {
    final data = await _client.get('/veiculos');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // consulta apenas veículos com status 'disponivel' para escalonamento
  @override
  Future<List<Veiculo>> findAvailable() async {
    final data = await _client.get('/veiculos/disponiveis');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // busca um veículo específico pelo id
  @override
  Future<Veiculo?> findById(int id) async {
    final data = await _client.get('/veiculos/$id');
    if (data is Map<String, dynamic>) {
      return _fromMap(data);
    }
    return null;
  }

  // insere um novo veículo na frota
  @override
  Future<void> insert(Veiculo veiculo) async {
    await _client.post('/veiculos', {
      'placa': veiculo.placa,
      'tipo': veiculo.tipo,
      'capacidadeCarga': veiculo.capacidadeCarga,
      'status': veiculo.status,
      'idTransportadora': veiculo.idTransportadora,
    });
  }

  // atualiza dados de cadastro e capacidade do veículo
  @override
  Future<void> update(Veiculo veiculo) async {
    await _client.put('/veiculos/${veiculo.id}', {
      'placa': veiculo.placa,
      'tipo': veiculo.tipo,
      'capacidadeCarga': veiculo.capacidadeCarga,
      'status': veiculo.status,
      'idTransportadora': veiculo.idTransportadora,
    });
  }

  // exclui o veículo do cadastro via api
  @override
  Future<void> delete(Veiculo veiculo) async {
    await _client.delete('/veiculos/${veiculo.id}');
  }
}

