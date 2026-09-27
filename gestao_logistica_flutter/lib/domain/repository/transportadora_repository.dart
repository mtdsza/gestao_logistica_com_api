import 'package:gestao_logistica/domain/entities/transportadora.dart';

// contrato do repositório de transportadoras definindo operações de persistência e consulta
abstract class ITransportadoraRepository {
  // cadastra uma nova transportadora
  Future<void> insert(Transportadora transportadora);
  // atualiza dados de uma transportadora existente
  Future<void> update(Transportadora transportadora);
  // exclui o registro de uma transportadora
  Future<void> delete(Transportadora transportadora);
  // lista todas as transportadoras
  Future<List<Transportadora>> findAll();
  // busca uma transportadora pelo seu id
  Future<Transportadora?> findById(int id);
}