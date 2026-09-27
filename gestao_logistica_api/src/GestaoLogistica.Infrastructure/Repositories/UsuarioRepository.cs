using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Domain.Repositories;
using GestaoLogistica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositories;

// implementação do repositório de usuários utilizando o entity framework core
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    // busca usuário pelo login incluindo os dados do motorista associado
    public async Task<Usuario?> GetByUsernameAsync(string username)
    {
        return await _context.Usuarios
            .Include(u => u.Motorista)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    // busca usuário pelo identificador numérico
    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Motorista)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    // lista todos os usuários cadastrados ordenados por id
    public async Task<List<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .Include(u => u.Motorista)
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    // insere novo usuário no banco
    public async Task AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    // atualiza dados do usuário no banco
    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }
}
