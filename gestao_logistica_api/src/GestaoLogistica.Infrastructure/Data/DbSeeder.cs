using GestaoLogistica.Application.Common.Interfaces;
using GestaoLogistica.Domain.Entities;
using GestaoLogistica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Data;

// classe responsável pelo povoamento inicial automático do banco de dados na primeira execução
public static class DbSeeder
{
    // cria o schema físico do banco e insere os registros básicos caso as tabelas estejam vazias
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        // assegura que o arquivo de banco sqlite e todas as tabelas foram criados
        await context.Database.EnsureCreatedAsync();

        // verifica se o banco já foi populado previamente (idempotência)
        if (await context.Transportadoras.AnyAsync())
        {
            return; // já populado, evita dados duplicados
        }

        // 1. cadastra as transportadoras parceiras iniciais
        var t1 = new Transportadora { Cnpj = "11111111000100", NomeFantasia = "Rapido Curitiba", CustoKm = 3.20m, CustoPeso = 0.12m };
        var t2 = new Transportadora { Cnpj = "22222222000100", NomeFantasia = "Paulista Cargas", CustoKm = 3.80m, CustoPeso = 0.18m };
        var t3 = new Transportadora { Cnpj = "33333333000100", NomeFantasia = "Minas Cargas", CustoKm = 2.90m, CustoPeso = 0.10m };

        await context.Transportadoras.AddRangeAsync(t1, t2, t3);
        await context.SaveChangesAsync();

        // 2. cadastra motoristas iniciais com status 'disponivel'
        var m1 = new Motorista { Nome = "Joao da Silva (Frota Propria)", Cnh = "12345678900", Status = "Disponivel", IdTransportadora = null };
        var m2 = new Motorista { Nome = "Carlos Curitibano", Cnh = "11111111111", Status = "Disponivel", IdTransportadora = t1.Id };
        var m3 = new Motorista { Nome = "Pedro Paulista", Cnh = "22222222222", Status = "Disponivel", IdTransportadora = t2.Id };
        var m4 = new Motorista { Nome = "Lucas Mineiro", Cnh = "33333333333", Status = "Disponivel", IdTransportadora = t3.Id };

        await context.Motoristas.AddRangeAsync(m1, m2, m3, m4);
        await context.SaveChangesAsync();

        // 3. cadastra usuários iniciais com senhas criptografadas via bcrypt (fator de trabalho 11 e salt aleatório)
        var senhaAdminHash = passwordHasher.HashPassword("admin123");
        var senhaMotoristaHash = passwordHasher.HashPassword("motorista123");

        var uAdmin = new Usuario { Username = "admin", PasswordHash = senhaAdminHash, IsAdmin = true, IdMotorista = null, Ativo = true };
        var uJoao = new Usuario { Username = "joao", PasswordHash = senhaMotoristaHash, IsAdmin = false, IdMotorista = m1.Id, Ativo = true };
        var uCarlos = new Usuario { Username = "carlos", PasswordHash = senhaMotoristaHash, IsAdmin = false, IdMotorista = m2.Id, Ativo = true };
        var uPedro = new Usuario { Username = "pedro", PasswordHash = senhaMotoristaHash, IsAdmin = false, IdMotorista = m3.Id, Ativo = true };
        var uLucas = new Usuario { Username = "lucas", PasswordHash = senhaMotoristaHash, IsAdmin = false, IdMotorista = m4.Id, Ativo = true };

        await context.Usuarios.AddRangeAsync(uAdmin, uJoao, uCarlos, uPedro, uLucas);

        // 4. cadastra veículos iniciais com suas respectivas capacidades de carga
        var v1 = new Veiculo { Placa = "ABC-1234", Tipo = "Fiorino", CapacidadeCarga = 500.0m, Status = "Disponivel", IdTransportadora = null };
        var v2 = new Veiculo { Placa = "XYZ-5678", Tipo = "Caminhao Bau", CapacidadeCarga = 5000.0m, Status = "Disponivel", IdTransportadora = null };
        var v3 = new Veiculo { Placa = "PRC-1000", Tipo = "Caminhao Bau", CapacidadeCarga = 4000.0m, Status = "Disponivel", IdTransportadora = t1.Id };
        var v4 = new Veiculo { Placa = "SPC-2000", Tipo = "Carreta", CapacidadeCarga = 15000.0m, Status = "Disponivel", IdTransportadora = t2.Id };
        var v5 = new Veiculo { Placa = "MGC-3000", Tipo = "Fiorino", CapacidadeCarga = 600.0m, Status = "Disponivel", IdTransportadora = t3.Id };

        await context.Veiculos.AddRangeAsync(v1, v2, v3, v4, v5);

        // 5. cadastra mercadorias e estoques distribuídos entre são paulo, curitiba e belo horizonte
        var itens = new List<ItemInventario>
        {
            new() { Nome = "Televisor LED 55", Tipo = "Eletronico", PesoUnitario = 15.0m, Quantidade = 100, Cidade = "Sao Paulo", DataValidade = null, Localizacao = "Setor A - Prateleira 12" },
            new() { Nome = "Leite Condensado CX", Tipo = "Perecivel", PesoUnitario = 0.4m, Quantidade = 1000, Cidade = "Sao Paulo", DataValidade = "2026-05-15", Localizacao = "Câmara Fria 2 - Setor Sul" },
            new() { Nome = "Iogurte Natural CX", Tipo = "Perecivel", PesoUnitario = 0.2m, Quantidade = 500, Cidade = "Sao Paulo", DataValidade = "2026-08-30", Localizacao = "Câmara Fria 1 - Setor Norte" },
            new() { Nome = "Cimento Saco 50kg", Tipo = "Geral", PesoUnitario = 50.0m, Quantidade = 200, Cidade = "Curitiba", DataValidade = null, Localizacao = "Galpão Externo - Setor C" },
            new() { Nome = "Pneu de Caminhao", Tipo = "Geral", PesoUnitario = 80.0m, Quantidade = 50, Cidade = "Curitiba", DataValidade = null, Localizacao = "Estoque Central de Pneus" },
            new() { Nome = "Cafe Gourmet Moido", Tipo = "Geral", PesoUnitario = 0.5m, Quantidade = 300, Cidade = "Belo Horizonte", DataValidade = null, Localizacao = "Prateleira Grãos Minas 2" }
        };

        await context.ItensInventario.AddRangeAsync(itens);
        await context.SaveChangesAsync();
    }
}
