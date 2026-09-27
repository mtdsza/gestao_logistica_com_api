import 'package:gestao_logistica/domain/entities/motorista.dart';
import 'package:gestao_logistica/domain/repository/motorista_repository.dart';
import 'package:gestao_logistica/service_locator.dart';

// serviço de domínio responsável pelas regras e operações de negócio de motoristas
class MotoristaDomainServices {
  // obtém a instância ativa do repositório de motoristas no servicelocator
  final IMotoristaRepository _motoristaRepository =
      ServiceLocator.instance.findRepository(ServiceLocator.motoristaRepository);

  // cadastra um novo motorista
  Future<void> insert(Motorista motorista) async {
    await _motoristaRepository.insert(motorista);
  }

  // atualiza dados de um motorista existente
  Future<void> update(Motorista motorista) async {
    await _motoristaRepository.update(motorista);
  }

  // exclui o motorista do cadastro
  Future<void> delete(Motorista motorista) async {
    await _motoristaRepository.delete(motorista);
  }

  // consulta a lista com todos os motoristas
  Future<List<Motorista>> findAll() async {
    return await _motoristaRepository.findAll();
  }

  // busca um motorista pelo identificador primário
  Future<Motorista?> findById(int id) async {
    return await _motoristaRepository.findById(id);
  }

  // consulta motoristas disponíveis para alocação em novas viagens
  Future<List<Motorista>> findAvailable() async {
    return await _motoristaRepository.findAvailable();
  }
}