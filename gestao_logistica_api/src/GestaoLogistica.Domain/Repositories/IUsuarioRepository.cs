using GestaoLogistica.Domain.Entities;

namespace GestaoLogistica.Domain.Repositories;

// contrato de persistência para operações relacionadas a usuários
public interface IUsuarioRepository
{
    // busca usuário pelo nome de login único
    Task<Usuario?> GetByUsernameAsync(string username);

    // busca usuário pelo identificador numérico
    Task<Usuario?> GetByIdAsync(int id);

    // lista todos os usuários cadastrados
    Task<List<Usuario>> GetAllAsync();

    // persiste um novo usuário no banco
    Task AddAsync(Usuario usuario);

    // atualiza os dados ou status de ativação do usuário
    Task UpdateAsync(Usuario usuario);
}
