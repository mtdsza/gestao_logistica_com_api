import 'package:gestao_logistica/domain/entities/motorista.dart';

// contrato do repositório de motoristas definindo operações de persistência e consulta
abstract class IMotoristaRepository {
  // cadastra um novo motorista
  Future<void> insert(Motorista motorista);
  // atualiza dados cadastrais de um motorista existente
  Future<void> update(Motorista motorista);
  // exclui o registro do motorista
  Future<void> delete(Motorista motorista);
  // lista todos os motoristas cadastrados
  Future<List<Motorista>> findAll();
  // busca um motorista pelo identificador primário
  Future<Motorista?> findById(int id);
  // retorna apenas motoristas com status 'disponivel' para novas viagens
  Future<List<Motorista>> findAvailable();
}