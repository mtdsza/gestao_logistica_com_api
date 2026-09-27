using GestaoLogistica.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Data;

// contexto principal do entity framework core para acesso ao banco de dados sqlite
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // tabelas mapeadas no banco de dados
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Transportadora> Transportadoras => Set<Transportadora>();
    public DbSet<Motorista> Motoristas => Set<Motorista>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<ItemInventario> ItensInventario => Set<ItemInventario>();
    public DbSet<Viagem> Viagens => Set<Viagem>();
    public DbSet<ViagemItem> ViagemItens => Set<ViagemItem>();

    // configuração fluente das entidades, restrições e relacionamentos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // configuração da tabela de transportadoras
        modelBuilder.Entity<Transportadora>(entity =>
        {
            entity.ToTable("transportadoras");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Cnpj).IsUnique(); // garante unicidade do cnpj
            entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(20);
            entity.Property(e => e.NomeFantasia).IsRequired().HasMaxLength(150);
            entity.Property(e => e.CustoKm).HasColumnType("decimal(18,2)").HasDefaultValue(3.00m);
            entity.Property(e => e.CustoPeso).HasColumnType("decimal(18,2)").HasDefaultValue(0.15m);
        });

        // configuração da tabela de motoristas
        modelBuilder.Entity<Motorista>(entity =>
        {
            entity.ToTable("motoristas");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Cnh).IsUnique(); // cnh única no sistema
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cnh).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30).HasDefaultValue("Disponivel");

            // relação opcional com transportadora; se a transportadora for excluída, seta nulo
            entity.HasOne(e => e.Transportadora)
                  .WithMany(t => t.Motoristas)
                  .HasForeignKey(e => e.IdTransportadora)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // configuração da tabela de veículos
        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.ToTable("veiculos");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Placa).IsUnique(); // placa veicular única
            entity.Property(e => e.Placa).IsRequired().HasMaxLength(15);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CapacidadeCarga).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30).HasDefaultValue("Disponivel");

            entity.HasOne(e => e.Transportadora)
                  .WithMany(t => t.Veiculos)
                  .HasForeignKey(e => e.IdTransportadora)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // configuração da tabela de itens do inventário
        modelBuilder.Entity<ItemInventario>(entity =>
        {
            entity.ToTable("itens_inventario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(120);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PesoUnitario).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Quantidade).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.Cidade).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DataValidade).HasMaxLength(30);
            entity.Property(e => e.Localizacao).HasMaxLength(150);
        });

        // configuração da tabela de usuários
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique(); // login único
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.IsAdmin).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);

            // relacionamento um-para-um opcional com motorista
            entity.HasOne(e => e.Motorista)
                  .WithOne(m => m.Usuario)
                  .HasForeignKey<Usuario>(e => e.IdMotorista)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // configuração da tabela de viagens
        modelBuilder.Entity<Viagem>(entity =>
        {
            entity.ToTable("viagens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Origem).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Destino).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustoTransporte).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30).HasDefaultValue("Pendente");
            entity.Property(e => e.PesoTotal).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            entity.Property(e => e.DataPlanejada).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Faturado).IsRequired().HasDefaultValue(false);

            // motorista e veículo são obrigatórios e não podem ser excluídos enquanto houver viagens (restrict)
            entity.HasOne(e => e.Motorista)
                  .WithMany(m => m.Viagens)
                  .HasForeignKey(e => e.IdMotorista)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Veiculo)
                  .WithMany(v => v.Viagens)
                  .HasForeignKey(e => e.IdVeiculo)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Transportadora)
                  .WithMany(t => t.Viagens)
                  .HasForeignKey(e => e.IdTransportadora)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // configuração da tabela intermediária com chave composta
        modelBuilder.Entity<ViagemItem>(entity =>
        {
            entity.ToTable("viagem_itens");
            entity.HasKey(e => new { e.IdViagem, e.IdItem }); // chave primária composta
            entity.Property(e => e.QuantidadeTransportada).IsRequired();

            // se a viagem for apagada, seus itens associados são removidos em cascata
            entity.HasOne(e => e.Viagem)
                  .WithMany(v => v.Itens)
                  .HasForeignKey(e => e.IdViagem)
                  .OnDelete(DeleteBehavior.Cascade);

            // impede a exclusão de um produto do catálogo se ele estiver vinculado a viagens
            entity.HasOne(e => e.Item)
                  .WithMany(i => i.ViagemItens)
                  .HasForeignKey(e => e.IdItem)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
