import 'package:gestao_logistica/domain/entities/usuario.dart';
import 'package:gestao_logistica/domain/repository/usuario_repository.dart';
import 'package:gestao_logistica/service_locator.dart';
import 'package:gestao_logistica/data/api/api_config.dart';

// serviço de domínio responsável por gerenciar a sessão do usuário autenticado no aplicativo
class UsuarioDomainServices {
  // obtém a instância ativa do repositório de usuários configurado no servicelocator
  final IUsuarioRepository _usuarioRepository =
      ServiceLocator.instance.findRepository(ServiceLocator.usuarioRepository);

  // variável estática que armazena os dados cadastrais e permissões do usuário logado na sessão
  static Usuario? _usuarioLogado;

  // getter público para que telas e controladores possam inspecionar o usuário ativo
  static Usuario? get usuarioLogado => _usuarioLogado;

  // executa a autenticação chamando o repositório e inicializa o usuário logado em caso de sucesso
  Future<bool> autenticar(String username, String password) async {
    final usuario = await _usuarioRepository.login(username, password);
    if (usuario != null) {
      _usuarioLogado = usuario;
      return true;
    }
    return false;
  }

  // encerra a sessão ativa, limpando o usuário em memória e expurgando o token jwt salvo
  void deslogar() {
    _usuarioLogado = null;
    ApiConfig.clearToken();
  }
}