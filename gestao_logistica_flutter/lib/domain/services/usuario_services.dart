import 'package:gestao_logistica/domain/entities/usuario.dart';
import 'package:gestao_logistica/domain/repository/usuario_repository.dart';
import 'package:gestao_logistica/service_locator.dart';

// serviço de domínio para operações cadastrais e administrativas de usuários
class UsuarioServices {
  // obtém a instância ativa do repositório de usuários no servicelocator
  final IUsuarioRepository _usuarioRepository =
      ServiceLocator.instance.findRepository(ServiceLocator.usuarioRepository);

  // insere um novo usuário na base de dados
  Future<void> insert(Usuario usuario) async {
    await _usuarioRepository.insert(usuario);
  }

  // consulta a lista com todos os usuários cadastrados
  Future<List<Usuario>> findAll() async {
    return await _usuarioRepository.findAll();
  }

  // altera a flag de ativação do usuário
  Future<void> updateAtivo(int id, bool ativo) async {
    await _usuarioRepository.updateAtivo(id, ativo);
  }
}