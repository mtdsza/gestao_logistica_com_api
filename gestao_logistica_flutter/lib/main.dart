import 'dart:io'; // biblioteca padrão para identificar o sistema operacional
import 'package:flutter/material.dart';
import 'package:gestao_logistica/routes.dart';
import 'package:gestao_logistica/service_locator.dart';
import 'package:sqflite_common_ffi/sqflite_ffi.dart'; // dá suporte aos bancos sqlite em plataformas desktop

import 'package:gestao_logistica/data/api/api_config.dart';

// ponto de entrada principal do aplicativo flutter
void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // carrega token jwt salvo anteriormente no armazenamento local, se existir
  await ApiConfig.loadToken();

  // verifica se o aplicativo está rodando em ambiente desktop (caso utilize sqlite local)
  if (Platform.isWindows || Platform.isLinux || Platform.isMacOS) {
    sqfliteFfiInit();
    databaseFactory = databaseFactoryFfi;
  }

  // inicializa o servicelocator, registrando os repositórios (api restful por padrão)
  ServiceLocator.instance.setupRepository();

  runApp(const MainApp());
}
// configuração global da interface e rotas
class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Gestão Logística',
      theme: ThemeData(
        primarySwatch: Colors.blue,
        useMaterial3: false,
      ),
      initialRoute: AppRoutes.login,
      routes: AppRoutes.routes,
    );
  }
}