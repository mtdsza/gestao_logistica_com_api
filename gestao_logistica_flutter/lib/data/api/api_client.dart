import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'package:http/http.dart' as http;
import 'package:gestao_logistica/data/api/api_config.dart';

// exceção customizada para erros retornados pela api ou falhas de comunicação http
class ApiException implements Exception {
  // mensagem de erro descritiva
  final String message;

  // lista opcional de inconsistências ou validações enviadas pelo backend
  final List<String> errors;

  // código de status http retornado pelo servidor
  final int statusCode;

  // construtor da exceção com valores padrão para statusCode e lista de erros
  ApiException(this.message, {this.errors = const [], this.statusCode = 400});

  @override
  String toString() {
    if (errors.isNotEmpty) {
      return '$message: ${errors.join(", ")}';
    }
    return message;
  }
}

// cliente http centralizado configurado como singleton
// atua como interceptador de requisições e respostas no aplicativo flutter:
// 1. injeção de cabeçalhos padrão ('application/json') e autorização ('bearer <token>') em todas as chamadas
// 2. decodificação uniforme do envelope resultwrapper retornado pelo asp.net core
// 3. conversão automática de falhas com success=false ou status >= 400 em instâncias de apiexception
class ApiClient {
  // instância única do padrão singleton
  static final ApiClient _instance = ApiClient._internal();

  // construtor de fábrica que retorna a mesma instância única
  factory ApiClient() => _instance;

  // construtor privado impedindo instanciação externa direta
  ApiClient._internal();

  // interceptor de saída: monta os cabeçalhos http e injeta o token bearer jwt ativo
  Map<String, String> _buildHeaders() {
    final headers = <String, String>{
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };

    // se houver token jwt salvo em apiconfig, anexa ao cabeçalho authorization
    final token = ApiConfig.token;
    if (token != null && token.isNotEmpty) {
      headers['Authorization'] = 'Bearer $token';
    }

    return headers;
  }

  // interceptor de entrada: decodifica a resposta json e desembala o envelope resultwrapper
  dynamic _processResponse(http.Response response) {
    // verifica se a resposta não possui corpo de retorno
    if (response.body.isEmpty) {
      if (response.statusCode >= 200 && response.statusCode < 300) {
        return null;
      }
      if (response.statusCode == 401) {
        ApiConfig.clearToken();
        throw ApiException('Sessão expirada ou usuário não autenticado (401). Faça login novamente.', statusCode: 401);
      }
      if (response.statusCode == 403) {
        throw ApiException('Acesso negado (403). Você não possui permissão para esta operação.', statusCode: 403);
      }
      if (response.statusCode == 404) {
        throw ApiException('Recurso não encontrado (404).', statusCode: 404);
      }
      throw ApiException('Resposta vazia da API (status ${response.statusCode})', statusCode: response.statusCode);
    }

    // converte o corpo json em mapa ou lista dinâmica de forma segura
    dynamic decoded;
    try {
      decoded = jsonDecode(response.body);
    } catch (_) {
      if (response.statusCode >= 400) {
        throw ApiException(
          'Erro no servidor (${response.statusCode}): ${response.body}',
          statusCode: response.statusCode,
        );
      }
      return response.body;
    }

    // se o json for um objeto, verifica se segue o padrão resultwrapper da api
    if (decoded is Map<String, dynamic>) {
      final success = decoded['success'] as bool? ?? (response.statusCode >= 200 && response.statusCode < 300);
      final message = decoded['message'] as String? ?? decoded['title'] as String? ?? '';
      final errors = (decoded['errors'] as List?)?.map((e) => e.toString()).toList() ?? [];
      final data = decoded['data'];

      // se o backend indicou falha na operação ou status http de erro, lança apiexception
      if (!success || response.statusCode >= 400) {
        if (response.statusCode == 401) {
          ApiConfig.clearToken();
        }
        throw ApiException(
          message.isNotEmpty
              ? message
              : (response.statusCode == 401
                  ? 'Sessão expirada ou usuário não autenticado. Faça login novamente.'
                  : 'Erro na requisição'),
          errors: errors,
          statusCode: response.statusCode,
        );
      }

      // retorna o payload desembalado diretamente para a camada consumidora
      return data;
    }

    // se statusCode >= 400 mas não é Map (por exemplo lista de erros ou resposta primitiva)
    if (response.statusCode >= 400) {
      if (response.statusCode == 401) {
        ApiConfig.clearToken();
      }
      throw ApiException(
        'Erro na requisição (${response.statusCode})',
        statusCode: response.statusCode,
      );
    }

    return decoded;
  }

  // executa chamada http capturando falhas de conectividade e timeout
  Future<dynamic> _executarRequisicao(Future<http.Response> Function() chamada) async {
    try {
      final response = await chamada().timeout(const Duration(seconds: 15));
      return _processResponse(response);
    } on ApiException {
      rethrow;
    } on SocketException catch (e) {
      throw ApiException('Não foi possível conectar ao servidor da API em ${ApiConfig.baseUrl}. Verifique se o backend está em execução. ($e)', statusCode: 0);
    } on http.ClientException catch (e) {
      throw ApiException('Falha de conexão com a API: ${e.message}', statusCode: 0);
    } on TimeoutException {
      throw ApiException('Tempo limite de conexão esgotado ao contatar a API (${ApiConfig.baseUrl}).', statusCode: 408);
    } catch (e) {
      throw ApiException('Erro inesperado de comunicação com a API: $e', statusCode: 0);
    }
  }

  // executa requisição http get para o endpoint informado
  Future<dynamic> get(String endpoint) async {
    final uri = Uri.parse('${ApiConfig.baseUrl}$endpoint');
    return _executarRequisicao(() => http.get(uri, headers: _buildHeaders()));
  }

  // executa requisição http post serializando o corpo em json
  Future<dynamic> post(String endpoint, dynamic body) async {
    final uri = Uri.parse('${ApiConfig.baseUrl}$endpoint');
    return _executarRequisicao(() => http.post(
      uri,
      headers: _buildHeaders(),
      body: jsonEncode(body),
    ));
  }

  // executa requisição http put para atualização completa do recurso
  Future<dynamic> put(String endpoint, dynamic body) async {
    final uri = Uri.parse('${ApiConfig.baseUrl}$endpoint');
    return _executarRequisicao(() => http.put(
      uri,
      headers: _buildHeaders(),
      body: jsonEncode(body),
    ));
  }

  // executa requisição http patch para atualização parcial do recurso
  Future<dynamic> patch(String endpoint, dynamic body) async {
    final uri = Uri.parse('${ApiConfig.baseUrl}$endpoint');
    return _executarRequisicao(() => http.patch(
      uri,
      headers: _buildHeaders(),
      body: jsonEncode(body),
    ));
  }

  // executa requisição http delete para remoção do recurso
  Future<dynamic> delete(String endpoint) async {
    final uri = Uri.parse('${ApiConfig.baseUrl}$endpoint');
    return _executarRequisicao(() => http.delete(uri, headers: _buildHeaders()));
  }
}

