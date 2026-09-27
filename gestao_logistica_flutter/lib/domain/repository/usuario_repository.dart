import 'package:gestao_logistica/domain/entities/usuario.dart';

// contrato do repositório de usuários definindo operações de autenticação e consulta
abstract class IUsuarioRepository {
  // autentica o usuário via credenciais informadas
  Future<Usuario?> login(String username, String password);
  // insere um novo usuário na base de dados
  Future<void> insert(Usuario usuario);
  // lista todos os usuários cadastrados
  Future<List<Usuario>> findAll();
  // atualiza o status de ativação da conta de um usuário
  Future<void> updateAtivo(int id, bool ativo);
}