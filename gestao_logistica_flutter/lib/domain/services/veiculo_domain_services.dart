import 'package:gestao_logistica/domain/entities/veiculo.dart';
import 'package:gestao_logistica/domain/repository/veiculo_repository.dart';
import 'package:gestao_logistica/service_locator.dart';

// serviço de domínio responsável pelas regras e operações de negócio de veículos
class VeiculoDomainServices {
  // obtém a instância ativa do repositório de veículos no servicelocator
  final IVeiculoRepository _veiculoRepository =
      ServiceLocator.instance.findRepository(ServiceLocator.veiculoRepository);

  // cadastra um novo veículo na frota
  Future<void> insert(Veiculo veiculo) async {
    await _veiculoRepository.insert(veiculo);
  }

  // atualiza dados de um veículo existente
  Future<void> update(Veiculo veiculo) async {
    await _veiculoRepository.update(veiculo);
  }

  // remove o veículo do sistema
  Future<void> delete(Veiculo veiculo) async {
    await _veiculoRepository.delete(veiculo);
  }

  // lista todos os veículos cadastrados
  Future<List<Veiculo>> findAll() async {
    return await _veiculoRepository.findAll();
  }

  // busca um veículo específico pelo id
  Future<Veiculo?> findById(int id) async {
    return await _veiculoRepository.findById(id);
  }

  // consulta veículos disponíveis para novas viagens
  Future<List<Veiculo>> findAvailable() async {
    return await _veiculoRepository.findAvailable();
  }
}