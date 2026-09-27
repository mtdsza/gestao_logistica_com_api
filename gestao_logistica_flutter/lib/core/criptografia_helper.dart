import 'dart:convert'; // biblioteca nativa do dart para codificação utf-8
import 'package:crypto/crypto.dart'; // importação do pacote de criptografia

// helper legado utilizado no modo sqlite local para derivação de hash com salt
class CriptografiaHelper {
  // salt estático adicionado ao texto da senha antes da geração do hash
  static const String _salt = "salt_7gd@19f#g917f";

  // implementação do algoritmo sha-256 para criptografar as senhas no sqlite local
  static String encriptar(String senha) {
    final textoComSalt = _salt + senha;
    final bytes = utf8.encode(textoComSalt);
    final digest = sha256.convert(bytes);
    return digest.toString();
  }
}