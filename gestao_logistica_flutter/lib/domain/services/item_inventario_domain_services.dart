import 'package:gestao_logistica/domain/entities/item_inventario.dart';
import 'package:gestao_logistica/domain/repository/item_inventario_repository.dart';
import 'package:gestao_logistica/service_locator.dart';

// serviço de domínio responsável pelas regras e operações de negócio de estoque e inventário
class ItemInventarioDomainServices {
  // obtém a instância ativa do repositório de itens no servicelocator
  final IItemInventarioRepository _itemInventarioRepository =
      ServiceLocator.instance.findRepository(ServiceLocator.itemInventarioRepository);

  // cadastra um novo item de inventário
  Future<void> insert(ItemInventario item) async {
    await _itemInventarioRepository.insert(item);
  }

  // atualiza dados de um item existente
  Future<void> update(ItemInventario item) async {
    await _itemInventarioRepository.update(item);
  }

  // remove o item do estoque
  Future<void> delete(ItemInventario item) async {
    await _itemInventarioRepository.delete(item);
  }

  // consulta a lista de todos os itens cadastrados
  Future<List<ItemInventario>> findAll() async {
    return await _itemInventarioRepository.findAll();
  }

  // busca um item pelo identificador primário
  Future<ItemInventario?> findById(int id) async {
    return await _itemInventarioRepository.findById(id);
  }

  // realiza ajuste manual ou pontual de quantidade em estoque
  Future<void> updateStock(int id, int novaQuantidade) async {
    await _itemInventarioRepository.updateStock(id, novaQuantidade);
  }
}