import 'package:gestao_logistica/data/local/usuario_repository_sqlite.dart';
import 'package:gestao_logistica/data/local/transportadora_repository_sqlite.dart';
import 'package:gestao_logistica/data/local/motorista_repository_sqlite.dart';
import 'package:gestao_logistica/data/local/veiculo_repository_sqlite.dart';
import 'package:gestao_logistica/data/local/item_inventario_repository_sqlite.dart';
import 'package:gestao_logistica/data/local/viagem_repository_sqlite.dart';

import 'package:gestao_logistica/data/api/usuario_repository_api.dart';
import 'package:gestao_logistica/data/api/transportadora_repository_api.dart';
import 'package:gestao_logistica/data/api/motorista_repository_api.dart';
import 'package:gestao_logistica/data/api/veiculo_repository_api.dart';
import 'package:gestao_logistica/data/api/item_inventario_repository_api.dart';
import 'package:gestao_logistica/data/api/viagem_repository_api.dart';

// define as possíveis fontes de dados
enum RepositoryType { local, memory, api }

// cria o servicelocator como singleton
class ServiceLocator {
  ServiceLocator._internal();
  static final ServiceLocator _instance = ServiceLocator._internal();
  static ServiceLocator get instance => _instance;

  // identificadores de texto para registrar e recuperar cada repositório
  static const String usuarioRepository = "usuario";
  static const String transportadoraRepository = "transportadora";
  static const String motoristaRepository = "motorista";
  static const String veiculoRepository = "veiculo";
  static const String itemInventarioRepository = "item_inventario";
  static const String viagemRepository = "viagem";

  // mapa em memória que armazena as dependências registradas
  final Map<String, dynamic> _repositories = {};

  // tipo de repositório ativo no momento (.api para consumir a api asp.net core)
  RepositoryType repositoryType = RepositoryType.api;

  // método chamado no início do app (main.dart) para configurar o repositório correto
  void setupRepository() {
    switch (repositoryType) {
      case RepositoryType.local:
        setupConfigRepositoryLocal();
        break;
      case RepositoryType.memory:
        setupConfigRepositoryMemory();
        break;
      case RepositoryType.api:
        setupConfigRepositoryApi();
        break;
    }
  }

  // registra as implementações físicas em sqlite no mapa de dependências
  void setupConfigRepositoryLocal() {
    _repositories[usuarioRepository] = UsuarioRepositorySqlite();
    _repositories[transportadoraRepository] = TransportadoraRepositorySqlite();
    _repositories[motoristaRepository] = MotoristaRepositorySqlite();
    _repositories[veiculoRepository] = VeiculoRepositorySqlite();
    _repositories[itemInventarioRepository] = ItemInventarioRepositorySqlite();
    _repositories[viagemRepository] = ViagemRepositorySqlite();
  }

  // registra as implementações conectadas à api asp.net core
  void setupConfigRepositoryApi() {
    _repositories[usuarioRepository] = UsuarioRepositoryApi();
    _repositories[transportadoraRepository] = TransportadoraRepositoryApi();
    _repositories[motoristaRepository] = MotoristaRepositoryApi();
    _repositories[veiculoRepository] = VeiculoRepositoryApi();
    _repositories[itemInventarioRepository] = ItemInventarioRepositoryApi();
    _repositories[viagemRepository] = ViagemRepositoryApi();
  }

  // reservado para futuras implementações voláteis em memória para testes unitários
  void setupConfigRepositoryMemory() {}

  // método para buscar e obter a instância do repositório desejado
  dynamic findRepository(String id) {
    if (_repositories.containsKey(id)) {
      return _repositories[id];
    }
    throw Exception("Repositório de ID '$id' não registrado no ServiceLocator.");
  }
}