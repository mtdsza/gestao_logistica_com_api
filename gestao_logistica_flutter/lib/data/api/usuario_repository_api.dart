import 'package:gestao_logistica/data/api/api_client.dart';
import 'package:gestao_logistica/data/api/api_config.dart';
import 'package:gestao_logistica/domain/entities/usuario.dart';
import 'package:gestao_logistica/domain/repository/usuario_repository.dart';

// implementação do repositório de usuários consumindo a api restful asp.net core
class UsuarioRepositoryApi implements IUsuarioRepository {
  // cliente http compartilhado para envio de requisições autenticadas
  final ApiClient _client = ApiClient();

  // realiza autenticação de usuário na api, capturando e salvando o token jwt retornado
  @override
  Future<Usuario?> login(String username, String password) async {
    try {
      // envia credenciais para o endpoint /auth/login
      final data = await _client.post('/auth/login', {
        'username': username,
        'password': password,
      });

      // se a autenticação for bem-sucedida, persiste o token e retorna o usuário de domínio
      if (data != null && data is Map<String, dynamic>) {
        final token = data['token'] as String?;
        if (token != null) {
          // salva o token jwt para ser injetado nas requisições subsequentes
          await ApiConfig.saveToken(token);
        }

        // instancia o objeto de domínio com os dados validados pelo servidor
        return Usuario(
          id: data['id'] as int?,
          username: data['username'] as String? ?? username,
          password: '',
          isAdmin: data['isAdmin'] as bool? ?? false,
          idMotorista: data['idMotorista'] as int?,
          ativo: true,
        );
      }
      return null;
    } catch (e) {
      rethrow;
    }
  }

  // cadastra um novo usuário no sistema enviando payload para o endpoint de registro
  @override
  Future<void> insert(Usuario usuario) async {
    await _client.post('/auth/register', {
      'username': usuario.username,
      'password': usuario.password,
      'isAdmin': usuario.isAdmin,
      'idMotorista': usuario.idMotorista,
    });
  }

  // consulta a listagem de todos os usuários cadastrados na api
  @override
  Future<List<Usuario>> findAll() async {
    final data = await _client.get('/auth/usuarios');
    if (data is List) {
      // mapeia a lista dinâmica de objetos json para instâncias da entidade usuario
      return data.map((item) {
        final map = item as Map<String, dynamic>;
        return Usuario(
          id: map['id'] as int?,
          username: (map['username'] ?? '') as String,
          password: '',
          isAdmin: map['isAdmin'] as bool? ?? (map['is_admin'] == 1),
          idMotorista: map['idMotorista'] as int? ?? map['id_motorista'] as int?,
          ativo: map['ativo'] == null ? true : (map['ativo'] == true || map['ativo'] == 1),
        );
      }).toList();
    }
    return [];
  }

  // atualiza o status de ativação do usuário através de requisição patch
  @override
  Future<void> updateAtivo(int id, bool ativo) async {
    await _client.patch('/auth/usuarios/$id/ativo', {
      'ativo': ativo,
    });
  }
}

