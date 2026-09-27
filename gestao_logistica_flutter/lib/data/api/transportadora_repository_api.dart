import 'package:gestao_logistica/data/api/api_client.dart';
import 'package:gestao_logistica/domain/entities/transportadora.dart';
import 'package:gestao_logistica/domain/repository/transportadora_repository.dart';

// implementação do repositório de transportadoras integrado à api asp.net core
class TransportadoraRepositoryApi implements ITransportadoraRepository {
  // cliente http compartilhado
  final ApiClient _client = ApiClient();

  // converte o mapa json retornado pela api em uma entidade transportadora de domínio
  Transportadora _fromMap(Map<String, dynamic> map) {
    return Transportadora(
      id: map['id'] as int?,
      cnpj: (map['cnpj'] ?? '') as String,
      nomeFantasia: (map['nomeFantasia'] ?? map['nome_fantasia'] ?? '') as String,
      custoKm: ((map['custoKm'] ?? map['custo_km'] ?? 3.00) as num).toDouble(),
      custoPeso: ((map['custoPeso'] ?? map['custo_peso'] ?? 0.15) as num).toDouble(),
    );
  }

  // busca todas as transportadoras cadastradas no backend
  @override
  Future<List<Transportadora>> findAll() async {
    final data = await _client.get('/transportadoras');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // busca uma transportadora específica pelo seu identificador primário
  @override
  Future<Transportadora?> findById(int id) async {
    final data = await _client.get('/transportadoras/$id');
    if (data is Map<String, dynamic>) {
      return _fromMap(data);
    }
    return null;
  }

  // cadastra uma nova transportadora enviando os parâmetros para o endpoint post
  @override
  Future<void> insert(Transportadora transportadora) async {
    await _client.post('/transportadoras', {
      'cnpj': transportadora.cnpj,
      'nomeFantasia': transportadora.nomeFantasia,
      'custoKm': transportadora.custoKm,
      'custoPeso': transportadora.custoPeso,
    });
  }

  // atualiza os dados cadastrais da transportadora via endpoint put
  @override
  Future<void> update(Transportadora transportadora) async {
    await _client.put('/transportadoras/${transportadora.id}', {
      'cnpj': transportadora.cnpj,
      'nomeFantasia': transportadora.nomeFantasia,
      'custoKm': transportadora.custoKm,
      'custoPeso': transportadora.custoPeso,
    });
  }

  // remove a transportadora do banco através do endpoint delete
  @override
  Future<void> delete(Transportadora transportadora) async {
    await _client.delete('/transportadoras/${transportadora.id}');
  }
}

