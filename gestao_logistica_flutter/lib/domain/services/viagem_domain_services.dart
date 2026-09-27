import 'package:gestao_logistica/domain/entities/viagem.dart';
import 'package:gestao_logistica/domain/repository/viagem_repository.dart';
import 'package:gestao_logistica/service_locator.dart';

// serviço de domínio responsável pelas regras e operações de negócio de viagens e monitoramento
class ViagemDomainServices {
  // obtém a instância ativa do repositório de viagens no servicelocator
  final IViagemRepository _viagemRepository =
      ServiceLocator.instance.findRepository(ServiceLocator.viagemRepository);

  // cadastra uma nova viagem e despacha os itens associados
  Future<void> insert(Viagem viagem, List<Map<String, dynamic>> itensComQuantidade) async {
    await _viagemRepository.insert(viagem, itensComQuantidade);
  }

  // atualiza o status de execução da viagem
  Future<void> updateStatus(int id, String novoStatus) async {
    await _viagemRepository.updateStatus(id, novoStatus);
  }

  // atualiza o estado de faturamento financeiro
  Future<void> updateFaturado(int id, bool faturado) async {
    await _viagemRepository.updateFaturado(id, faturado);
  }

  // consulta todas as viagens cadastradas
  Future<List<Viagem>> findAll() async {
    return await _viagemRepository.findAll();
  }

  // busca os detalhes de uma viagem específica
  Future<Viagem?> findById(int id) async {
    return await _viagemRepository.findById(id);
  }

  // consulta as viagens de um determinado motorista
  Future<List<Viagem>> findByMotorista(int idMotorista) async {
    return await _viagemRepository.findByMotorista(idMotorista);
  }

  // obtém os itens e volumes associados a uma viagem
  Future<List<Map<String, dynamic>>> findItensByViagem(int idViagem) async {
    return await _viagemRepository.findItensByViagem(idViagem);
  }

  // consulta o faturamento financeiro total para os kpis
  Future<double> getFaturamentoTotal() async {
    return await _viagemRepository.getFaturamentoTotal();
  }

  // consulta a taxa de entregas realizadas no prazo
  Future<double> getEficienciaNoPrazo() async {
    return await _viagemRepository.getEficienciaNoPrazo();
  }

  // consulta o número de viagens atualmente em trânsito
  Future<int> getViagensEmTransito() async {
    return await _viagemRepository.getViagensEmTransito();
  }

  // consulta histórico de viagens concluídas de um motorista
  Future<List<Viagem>> findHistoryByMotorista(int idMotorista) async {
    return await _viagemRepository.findHistoryByMotorista(idMotorista);
  }

  // consulta histórico de viagens realizadas por um veículo
  Future<List<Viagem>> findHistoryByVeiculo(int idVeiculo) async {
    return await _viagemRepository.findHistoryByVeiculo(idVeiculo);
  }

  // consulta histórico de viagens de uma transportadora
  Future<List<Viagem>> findHistoryByTransportadora(int idTransportadora) async {
    return await _viagemRepository.findHistoryByTransportadora(idTransportadora);
  }
}