import 'package:gestao_logistica/domain/entities/veiculo.dart';

// contrato do repositório de veículos definindo operações de persistência e consulta
abstract class IVeiculoRepository {
  // cadastra um novo veículo na frota
  Future<void> insert(Veiculo veiculo);
  // atualiza dados de um veículo existente
  Future<void> update(Veiculo veiculo);
  // exclui o veículo do cadastro
  Future<void> delete(Veiculo veiculo);
  // lista todos os veículos cadastrados
  Future<List<Veiculo>> findAll();
  // busca um veículo pelo identificador primário
  Future<Veiculo?> findById(int id);
  // retorna apenas veículos com status 'disponivel' para alocação
  Future<List<Veiculo>> findAvailable();
}