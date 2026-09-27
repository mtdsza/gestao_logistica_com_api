using GestaoLogistica.Application.Common.Interfaces;

namespace GestaoLogistica.Infrastructure.Security;

// implementação do serviço de hashing utilizando a biblioteca bcrypt
// atende à exigência estrita do trabalho de nunca salvar senhas em texto puro
public class BcryptPasswordHasher : IPasswordHasher
{
    // gera o hash seguro da senha embutindo automaticamente um salt aleatório criptográfico
    // o fator de trabalho (work factor = 11) realiza 2048 iterações de chave para resistir a ataques de força bruta
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
    }

    // verifica se a senha digitada pelo usuário gera o mesmo hash em tempo constante
    // evita ataques de temporização (timing attacks) e validação incorreta
    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }
}
