import 'package:gestao_logistica/domain/entities/item_inventario.dart';

// contrato do repositório de itens de inventário e controle de estoque
abstract class IItemInventarioRepository {
  // cadastra um novo item no estoque
  Future<void> insert(ItemInventario item);
  // atualiza dados cadastrais de um item existente
  Future<void> update(ItemInventario item);
  // exclui o item do inventário
  Future<void> delete(ItemInventario item);
  // lista todos os itens cadastrados no inventário
  Future<List<ItemInventario>> findAll();
  // busca um item pelo identificador primário
  Future<ItemInventario?> findById(int id);
  // atualiza a quantidade disponível do item em estoque
  Future<void> updateStock(int id, int novaQuantidade);
}