import 'package:flutter/material.dart';

// modelo reativo genérico baseado em changenotifier para gerenciamento de listas de busca
class SearchListModel<T> extends ChangeNotifier {
  // coleção em memória dos itens carregados
  List<T> data = [];

  SearchListModel();

  // atualiza o conjunto de dados e notifica os widgets ouvintes para reconstrução da interface
  void setData(List<T> dt) {
    data = dt;
    notifyListeners();
  }

  // retorna a lista atual de dados
  List<T> getData() {
    return data;
  }
  
  // dispara notificação aos ouvintes quando a lista interna for alterada
  void updateList() {
    notifyListeners();
  }
}