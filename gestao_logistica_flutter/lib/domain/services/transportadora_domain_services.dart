import 'package:gestao_logistica/domain/entities/transportadora.dart';
import 'package:gestao_logistica/domain/repository/transportadora_repository.dart';
import 'package:gestao_logistica/service_locator.dart';

// serviço de domínio responsável pelas regras e operações de negócio de transportadoras
class TransportadoraDomainServices {
  // obtém a instância ativa do repositório de transportadoras no servicelocator
  final ITransportadoraRepository _transportadoraRepository =
      ServiceLocator.instance.findRepository(ServiceLocator.transportadoraRepository);

  // cadastra uma nova transportadora no sistema
  Future<void> insert(Transportadora transportadora) async {
    await _transportadoraRepository.insert(transportadora);
  }

  // atualiza dados de uma transportadora existente
  Future<void> update(Transportadora transportadora) async {
    await _transportadoraRepository.update(transportadora);
  }

  // remove o cadastro da transportadora
  Future<void> delete(Transportadora transportadora) async {
    await _transportadoraRepository.delete(transportadora);
  }

  // consulta todas as transportadoras cadastradas
  Future<List<Transportadora>> findAll() async {
    return await _transportadoraRepository.findAll();
  }

  // busca uma transportadora pelo identificador primário
  Future<Transportadora?> findById(int id) async {
    return await _transportadoraRepository.findById(id);
  }
}