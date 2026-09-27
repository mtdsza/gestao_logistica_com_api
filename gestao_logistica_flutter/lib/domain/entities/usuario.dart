// entidade de domínio que representa o usuário do sistema
class Usuario {
  // identificador único do usuário
  final int? id;
  // login utilizado na autenticação
  final String username;
  // hash ou representação da senha
  final String password;
  // indica se o usuário possui privilégios de administrador
  final bool isAdmin;
  // chave estrangeira referenciando o motorista associado (se aplicável)
  final int? idMotorista;
  // indica se a conta está ativa para autenticação e operação
  final bool ativo;

  Usuario({
    this.id,
    required this.username,
    required this.password,
    required this.isAdmin,
    this.idMotorista,
    this.ativo = true,
  });

  // converte a entidade para mapa compatível com persistência local em sqlite
  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'username': username,
      'password': password,
      'is_admin': isAdmin ? 1 : 0,
      'id_motorista': idMotorista,
      'ativo': ativo ? 1 : 0,
    };
  }

  // instancia a entidade a partir de um mapa de dados
  factory Usuario.fromMap(Map<String, dynamic> map) {
    return Usuario(
      id: map['id'] as int?,
      username: map['username'] as String,
      password: map['password'] as String,
      isAdmin: (map['is_admin'] as int) == 1,
      idMotorista: map['id_motorista'] as int?,
      ativo: map['ativo'] == null ? true : (map['ativo'] as int) == 1,
    );
  }
}