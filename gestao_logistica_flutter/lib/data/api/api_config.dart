import 'dart:io';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

// gerenciador centralizado de configuração de rede e persistência do token jwt
class ApiConfig {
  // chave utilizada para persistir o token jwt no armazenamento local do dispositivo
  static const String _tokenKey = 'jwt_auth_token';

  // armazena url customizada definida em tempo de execução, se aplicável
  static String? _customBaseUrl;

  // resolve dinamicamente a url base da api de acordo com a plataforma de execução
  // no android o host da máquina de desenvolvimento é mapeado para 10.0.2.2,
  // enquanto em navegadores web e desktop (linux/macos/windows) utiliza-se localhost
  static String get baseUrl {
    if (_customBaseUrl != null) return _customBaseUrl!;

    // emulador android mapeia o localhost da máquina hospedeira em 10.0.2.2
    if (!kIsWeb && Platform.isAndroid) {
      return 'http://10.0.2.2:5167/api';
    }

    // ambiente desktop e navegadores web conectam diretamente via localhost
    return 'http://localhost:5167/api';
  }

  // permite sobrescrever dinamicamente o endereço da api para testes ou outros servidores
  static set baseUrl(String url) {
    _customBaseUrl = url;
  }

  // armazena em memória o token jwt ativo na sessão do usuário
  static String? _token;

  // obtém o token jwt em memória para inclusão nos cabeçalhos das requisições
  static String? get token => _token;

  // salva o token jwt na variável de memória e no sharedpreferences persistente
  static Future<void> saveToken(String token) async {
    _token = token;
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_tokenKey, token);
  }

  // carrega o token persistido no armazenamento local durante a inicialização do aplicativo
  static Future<String?> loadToken() async {
    final prefs = await SharedPreferences.getInstance();
    _token = prefs.getString(_tokenKey);
    return _token;
  }

  // remove o token da memória e do armazenamento persistente durante o logout
  static Future<void> clearToken() async {
    _token = null;
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_tokenKey);
  }
}

