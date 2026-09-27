import 'package:gestao_logistica/domain/entities/viagem.dart';

// contrato do repositório de viagens definindo operações de ciclo de vida e kpis
abstract class IViagemRepository {
  // cadastra uma nova viagem e associa a lista de itens transportados
  Future<void> insert(Viagem viagem, List<Map<String, dynamic>> itensComQuantidade);
  // atualiza a situação operacional da viagem
  Future<void> updateStatus(int id, String novoStatus);
  // atualiza a situação de faturamento financeiro
  Future<void> updateFaturado(int id, bool faturado);
  // lista todas as viagens cadastradas
  Future<List<Viagem>> findAll();
  // busca os detalhes de uma viagem específica
  Future<Viagem?> findById(int id);
  // lista as viagens ativas sob responsabilidade de um motorista
  Future<List<Viagem>> findByMotorista(int idMotorista);
  // recupera os itens transportados em uma determinada viagem
  Future<List<Map<String, dynamic>>> findItensByViagem(int idViagem);
  // calcula o somatório financeiro faturado
  Future<double> getFaturamentoTotal();
  // calcula o percentual de viagens entregues dentro do prazo
  Future<double> getEficienciaNoPrazo();
  // contabiliza quantas viagens estão em trânsito no momento
  Future<int> getViagensEmTransito();
  
  // consulta o histórico de viagens executadas por um motorista
  Future<List<Viagem>> findHistoryByMotorista(int idMotorista);
  // consulta o histórico de viagens realizadas com um veículo
  Future<List<Viagem>> findHistoryByVeiculo(int idVeiculo);
  // consulta o histórico de viagens operadas por uma transportadora
  Future<List<Viagem>> findHistoryByTransportadora(int idTransportadora);
}