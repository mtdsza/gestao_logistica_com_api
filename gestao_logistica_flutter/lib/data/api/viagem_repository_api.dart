import 'package:gestao_logistica/data/api/api_client.dart';
import 'package:gestao_logistica/domain/entities/viagem.dart';
import 'package:gestao_logistica/domain/repository/viagem_repository.dart';

// implementação do repositório de viagens consumindo a api asp.net core
// orquestra o planejamento, despacho, execução, faturamento e dashboards operacionais
class ViagemRepositoryApi implements IViagemRepository {
  // cliente http compartilhado para comunicação com o backend
  final ApiClient _client = ApiClient();

  // converte o mapa json em entidade viagem de domínio
  Viagem _fromMap(Map<String, dynamic> map) {
    return Viagem(
      id: map['id'] as int?,
      idMotorista: ((map['idMotorista'] ?? map['id_motorista'] ?? 0) as num).toInt(),
      idVeiculo: ((map['idVeiculo'] ?? map['id_veiculo'] ?? 0) as num).toInt(),
      idTransportadora: map['idTransportadora'] as int? ?? map['id_transportadora'] as int?,
      origem: (map['origem'] ?? '') as String,
      destino: (map['destino'] ?? '') as String,
      custoTransporte: ((map['custoTransporte'] ?? map['custo_transporte'] ?? 0.0) as num).toDouble(),
      status: (map['status'] ?? 'Pendente') as String,
      pesoTotal: ((map['pesoTotal'] ?? map['peso_total'] ?? 0.0) as num).toDouble(),
      dataPlanejada: (map['dataPlanejada'] ?? map['data_planejada'] ?? '') as String,
      faturado: map['faturado'] == true || map['faturado'] == 1,
    );
  }

  // recupera todas as viagens cadastradas na api
  @override
  Future<List<Viagem>> findAll() async {
    final data = await _client.get('/viagens');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // busca uma viagem pelo seu identificador primário
  @override
  Future<Viagem?> findById(int id) async {
    final data = await _client.get('/viagens/$id');
    if (data is Map<String, dynamic>) {
      return _fromMap(data);
    }
    return null;
  }

  // consulta as viagens ativas atribuídas a um motorista específico
  @override
  Future<List<Viagem>> findByMotorista(int idMotorista) async {
    final data = await _client.get('/viagens/motorista/$idMotorista');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // busca a lista de itens e volumes transportados em uma viagem específica
  @override
  Future<List<Map<String, dynamic>>> findItensByViagem(int idViagem) async {
    final data = await _client.get('/viagens/$idViagem');
    if (data is Map<String, dynamic> && data['itens'] is List) {
      final itensList = data['itens'] as List;
      return itensList.map((i) {
        final itemMap = i as Map<String, dynamic>;
        return {
          'id_item': itemMap['idItem'] ?? itemMap['id_item'],
          'quantidade_transportada': itemMap['quantidadeTransportada'] ?? itemMap['quantidade_transportada'],
          'nome': itemMap['nomeItem'] ?? itemMap['nome_item'] ?? '',
          'tipo': itemMap['tipoItem'] ?? itemMap['tipo_item'] ?? '',
          'peso_unitario': (itemMap['pesoUnitario'] ?? itemMap['peso_unitario'] as num?)?.toDouble() ?? 0.0,
        };
      }).toList();
    }
    return [];
  }

  // cria uma nova viagem com decremento atômico de estoque e cálculo de frete no backend
  @override
  Future<void> insert(Viagem viagem, List<Map<String, dynamic>> itensComQuantidade) async {
    final itensBody = itensComQuantidade.map((i) => {
      'idItem': i['id_item'] ?? i['idItem'],
      'quantidadeTransportada': i['quantidade_transportada'] ?? i['quantidadeTransportada'],
    }).toList();

    await _client.post('/viagens', {
      'idMotorista': viagem.idMotorista,
      'idVeiculo': viagem.idVeiculo,
      'idTransportadora': viagem.idTransportadora,
      'origem': viagem.origem,
      'destino': viagem.destino,
      'custoTransporte': viagem.custoTransporte,
      'status': viagem.status,
      'pesoTotal': viagem.pesoTotal,
      'dataPlanejada': viagem.dataPlanejada,
      'faturado': viagem.faturado,
      'itens': itensBody,
    });
  }

  // atualiza o status de execução da viagem (ex: Pendente -> Em Transito -> Concluida)
  @override
  Future<void> updateStatus(int id, String novoStatus) async {
    await _client.patch('/viagens/$id/status', {
      'status': novoStatus,
    });
  }

  // atualiza a flag de faturamento financeiro da viagem
  @override
  Future<void> updateFaturado(int id, bool faturado) async {
    await _client.patch('/viagens/$id/faturado', {
      'faturado': faturado,
    });
  }

  // obtém o faturamento total acumulado através do endpoint de kpis da api
  @override
  Future<double> getFaturamentoTotal() async {
    final data = await _client.get('/viagens/dashboard');
    if (data is Map<String, dynamic>) {
      return (data['faturamentoTotal'] as num?)?.toDouble() ?? 0.0;
    }
    return 0.0;
  }

  // obtém a taxa percentual de entregas no prazo calculada pela api
  @override
  Future<double> getEficienciaNoPrazo() async {
    final data = await _client.get('/viagens/dashboard');
    if (data is Map<String, dynamic>) {
      return (data['eficienciaNoPrazo'] as num?)?.toDouble() ?? 100.0;
    }
    return 100.0;
  }

  // obtém a contagem de viagens atualmente em trânsito
  @override
  Future<int> getViagensEmTransito() async {
    final data = await _client.get('/viagens/dashboard');
    if (data is Map<String, dynamic>) {
      return (data['viagensEmTransito'] as num?)?.toInt() ?? 0;
    }
    return 0;
  }

  // consulta histórico de viagens concluídas de um determinado motorista
  @override
  Future<List<Viagem>> findHistoryByMotorista(int idMotorista) async {
    final data = await _client.get('/viagens/historico/motorista/$idMotorista');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // consulta histórico de viagens vinculadas a um veículo específico
  @override
  Future<List<Viagem>> findHistoryByVeiculo(int idVeiculo) async {
    final data = await _client.get('/viagens/historico/veiculo/$idVeiculo');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // consulta histórico de viagens operadas por uma transportadora
  @override
  Future<List<Viagem>> findHistoryByTransportadora(int idTransportadora) async {
    final data = await _client.get('/viagens/historico/transportadora/$idTransportadora');
    if (data is List) {
      return data.map((item) => _fromMap(item as Map<String, dynamic>)).toList();
    }
    return [];
  }
}

