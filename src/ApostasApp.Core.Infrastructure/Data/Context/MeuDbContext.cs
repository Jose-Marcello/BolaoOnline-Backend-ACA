using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.RankingRodadas;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Ufs;
using ApostasApp.Core.Domain.Models.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;

namespace ApostasApp.Core.Infrastructure.Data.Context
{
    public class MeuDbContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        public MeuDbContext(DbContextOptions<MeuDbContext> options)
            : base(options)
        {
        }

        public DbSet<Campeonato> Campeonatos { get; set; }
        public DbSet<Rodada> Rodadas { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Uf> Ufs { get; set; }
        public DbSet<EquipeCampeonato> EquipesCampeonatos { get; set; }
        public DbSet<Estadio> Estadios { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Apostador> Apostadores { get; set; }
        public DbSet<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }
        public DbSet<Palpite> Palpites { get; set; }
        public DbSet<RankingRodada> RankingRodadas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ApostaRodada> ApostasRodada { get; set; }
        public DbSet<TransacaoFinanceira> TransacoesFinanceiras { get; set; }
        public DbSet<Saldo> Saldos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeuDbContext).Assembly);

            // --- CONFIGURAÇÕES DE RELAÇÕES (MANTIDAS COMO ANTERIORMENTE) ---
            // 1. Refino da relação TransacaoFinanceira para Saldo:
            modelBuilder.Entity<TransacaoFinanceira>()
                .HasOne(tf => tf.Saldo)
                .WithMany()
                .HasForeignKey(tf => tf.SaldoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // 2. Correção para Ciclo de Exclusão de RankingRodada:
            modelBuilder.Entity<RankingRodada>()
                .HasOne(rr => rr.Rodada)
                .WithMany(r => r.RankingRodadas)
                .HasForeignKey(rr => rr.RodadaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RankingRodada>()
                .HasOne(rr => rr.ApostadorCampeonato)
                .WithMany(ac => ac.RankingRodadas)
                .HasForeignKey(rr => rr.ApostadorCampeonatoId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Correção para Ciclo de Exclusão de JOGOS:
            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.EquipeCasa)
                .WithMany(ec => ec.JogosCasa)
                .HasForeignKey(j => j.EquipeCasaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.EquipeVisitante)
                .WithMany(ec => ec.JogosVisitante)
                .HasForeignKey(j => j.EquipeVisitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Mapeamento e DeleteBehavior para Jogo -> Estadio (Unidirecional):
            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.Estadio)
                .WithMany()
                .HasForeignKey(j => j.EstadioId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Mapeamento e DeleteBehavior para Jogo -> Rodada:
            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.Rodada)
                .WithMany(r => r.JogosRodada)
                .HasForeignKey(j => j.RodadaId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configurações para as propriedades decimal
            modelBuilder.Entity<Campeonato>()
                .Property(c => c.CustoAdesao)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Rodada>()
                .Property(r => r.CustoApostaRodada)
                .HasColumnType("decimal(18, 2)");
        }
            // --- FIM: CONFIGURAÇÕES DE RELAÇÕES ---


            // --- INÍCIO: DATA SEEDING (COLOQUE SEUS DADOS EXTRAÍDOS AQUI) ---

            // Exemplo de Seed de UFs (preenchido com base na extração)
            // Substitua os GUIDs e Nomes/Siglas pelos seus dados REAIS
            
            /*
            modelBuilder.Entity<Uf>().HasData(
                new Uf { Id = Guid.Parse("C1052671-5582-4E87-8A87-A7B0FE10558C"), Nome = "Rio de Janeiro", Sigla = "RJ" },
                new Uf { Id = Guid.Parse("96C5F6F3-9D96-4158-9A70-AF273B0FE81D"), Nome = "São Paulo", Sigla = "SP" },
                new Uf { Id = Guid.Parse("D8A4E1A9-B1D6-4F23-9F61-8C1A5C13D7E8"), Nome = "Minas Gerais", Sigla = "MG" }

               
                // Adicione as outras 24 UFs aqui
            );

            // Seed de AspNetRoles (Roles para o Identity)
            // Use os IDs que você já tem no seu BolaoLocal, ou gere novos se estiver começando do zero com Identity
            var adminRoleId = "SEU_ID_ROLE_ADMIN_AQUI"; // Ex: "a1b2c3d4-e5f6-7890-1234-567890abcdef"
            var userRoleId = "SEU_ID_ROLE_USER_AQUI";   // Ex: "f6e5d4c3-b2a1-0987-6543-2109fedcba98"

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = adminRoleId },
                new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER", ConcurrencyStamp = userRoleId }
            );

            // Seed de AspNetUsers (Usuário Admin)
            // Use os IDs e dados reais do seu usuário admin se for migrar.
            // Se for um novo admin, use Guid.NewGuid().ToString() para o ID e ajuste os detalhes.
            var adminUserId = "SEU_ID_USUARIO_ADMIN_AQUI"; // Ex: "1a2b3c4d-5e6f-7890-1234-567890abcdef"
            var hasher = new PasswordHasher<Usuario>();

            var adminUser = new Usuario
            {
                Id = adminUserId,
                UserName = "admin@aposta.com", // Mude se tiver outro username
                NormalizedUserName = "ADMIN@APOSTA.COM",
                Email = "admin@aposta.com",
                NormalizedEmail = "ADMIN@APOSTA.COM",
                EmailConfirmed = true,
                CPF = "12345678901",
                Celular = "21999999999",
                Apelido = "AdminMaster",
                RegistrationDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                RefreshToken = Guid.NewGuid().ToString(),
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                PhoneNumberConfirmed = false
            };

            // Use a senha que você quer para o admin. Se for reutilizar um usuário antigo,
            // considere redefinir a senha para algo temporário ou instruir o usuário a fazer "Esqueci a senha".
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "SuaNovaSenhaSegura@123");

            modelBuilder.Entity<Usuario>().HasData(adminUser);

            // Seed de AspNetUserRoles (Associar Admin ao Papel Admin)
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );

            // Exemplo de Seed para Campeonatos (substitua pelos seus dados reais)
            modelBuilder.Entity<Campeonato>().HasData(
                new Campeonato
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), // Use seu GUID real
                    Nome = "Campeonato Brasileiro 2024",
                    DataInic = new DateTime(2024, 4, 13),
                    DataFim = new DateTime(2024, 12, 8),
                    NumRodadas = 38,
                    Tipo = 1, // Ajuste conforme seu enum
                    Ativo = true,
                    CustoAdesao = 100.00m
                },
                new Campeonato
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), // Use seu GUID real
                    Nome = "Copa do Brasil 2024",
                    DataInic = new DateTime(2024, 2, 20),
                    DataFim = new DateTime(2024, 11, 10),
                    NumRodadas = 1, // Exemplo: rodadas eliminatórias
                    Tipo = 2,
                    Ativo = true,
                    CustoAdesao = 0.00m // Pode ser grátis
                }
            );

            // Exemplo de Seed para Equipes (substitua pelos seus dados reais, e use os UfIds corretos)
            modelBuilder.Entity<Equipe>().HasData(
                new Equipe { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Nome = "Flamengo", Sigla = "FLA", Tipo = 1, UfId = Guid.Parse("C1052671-5582-4E87-8A87-A7B0FE10558C") },
                new Equipe { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Nome = "Vasco da Gama", Sigla = "VAS", Tipo = 1, UfId = Guid.Parse("C1052671-5582-4E87-8A87-A7B0FE10558C") },
                new Equipe { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Nome = "Palmeiras", Sigla = "PAL", Tipo = 1, UfId = Guid.Parse("96C5F6F3-9D96-4158-9A70-AF273B0FE81D") }
                // Adicione as outras equipes aqui
            );

            // Exemplo de Seed para Estadios (substitua pelos seus dados reais, e use os UfIds corretos)
            modelBuilder.Entity<Estadio>().HasData(
                new Estadio { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Nome = "Maracanã", UfId = Guid.Parse("C1052671-5582-4E87-8A87-A7B0FE10558C") },
                new Estadio { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Nome = "Morumbi", UfId = Guid.Parse("96C5F6F3-9D96-4158-9A70-AF273B0FE81D") }
                // Adicione os outros estadios aqui
            );

            // Exemplo de Seed para EquipeCampeonato (substitua pelos seus dados reais)
            modelBuilder.Entity<EquipeCampeonato>().HasData(
                new EquipeCampeonato { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), CampeonatoId = Guid.Parse("00000000-0000-0000-0000-000000000004"), EquipeId = Guid.Parse("10000000-0000-0000-0000-000000000001") }, // Fla no Brasileirão
                new EquipeCampeonato { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), CampeonatoId = Guid.Parse("00000000-0000-0000-0000-000000000004"), EquipeId = Guid.Parse("10000000-0000-0000-0000-000000000002") }  // Vasco no Brasileirão
                // Adicione as outras EquipesCampeonatos aqui
            );

            // Exemplo de Seed para Apostadores (substitua pelos seus dados reais, use o UsuarioId correto)
            // Crie um Apostador para o Admin
            modelBuilder.Entity<Apostador>().HasData(
                new Apostador
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), // Use seu GUID real
                    UsuarioId = adminUserId, // O ID do usuário Admin que você definiu acima
                    NomeCompleto = "Administrador do Sistema",
                    Email = "admin@aposta.com",
                    Status = 1 // Ajuste conforme seu enum
                }
                // Adicione outros apostadores aqui
            );

            // Exemplo de Seed para Saldos (substitua pelos seus dados reais, use o ApostadorId correto)
            modelBuilder.Entity<Saldo>().HasData(
                new Saldo
                {
                    Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), // Use seu GUID real
                    ApostadorId = Guid.Parse("40000000-0000-0000-0000-000000000001"), // O ID do apostador admin
                    Valor = 500.00m,
                    DataUltimaAtualizacao = DateTime.Now
                }
                // Adicione outros saldos aqui
            );

            // Exemplo de Seed para Rodadas (substitua pelos seus dados reais, use o CampeonatoId correto)
            modelBuilder.Entity<Rodada>().HasData(
                new Rodada
                {
                    Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    CampeonatoId = Guid.Parse("00000000-0000-0000-0000-000000000004"), // ID do Brasileirão
                    NumeroRodada = 1,
                    DataInic = new DateTime(2024, 4, 13),
                    DataFim = new DateTime(2024, 4, 15),
                    NumJogos = 10,
                    Status = 1,
                    CustoApostaRodada = 5.00m
                }
                // Adicione outras rodadas aqui
            );

            // Exemplo de Seed para Jogos (substitua pelos seus dados reais, use os IDs corretos)
            modelBuilder.Entity<Jogo>().HasData(
                new Jogo
                {
                    Id = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    RodadaId = Guid.Parse("60000000-0000-0000-0000-000000000001"), // ID da Rodada 1
                    DataJogo = new DateTime(2024, 4, 13),
                    HoraJogo = new TimeSpan(16, 0, 0),
                    EstadioId = Guid.Parse("20000000-0000-0000-0000-000000000001"), // ID do Maracanã
                    EquipeCasaId = Guid.Parse("30000000-0000-0000-0000-000000000001"), // ID do Fla no Brasileirão
                    EquipeVisitanteId = Guid.Parse("30000000-0000-0000-0000-000000000002"), // ID do Vasco no Brasileirão
                    PlacarCasa = null, // Ou um valor inicial se já ocorreu
                    PlacarVisita = null,
                    Status = 0 // Ajuste conforme seu enum de status do jogo
                }
                // Adicione outros jogos aqui
            );

            // Exemplo de Seed para ApostasRodada (substitua pelos seus dados reais)
            modelBuilder.Entity<ApostaRodada>().HasData(
                new ApostaRodada
                {
                    Id = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                    ApostadorCampeonatoId = Guid.Parse("30000000-0000-0000-0000-000000000001"), // Apostador Admin no Brasileirão
                    RodadaId = Guid.Parse("60000000-0000-0000-0000-000000000001"), // Rodada 1
                    IdentificadorAposta = "APOSTA-ADMIN-R1",
                    DataHoraSubmissao = DateTime.Now,
                    EhApostaCampeonato = false,
                    EhApostaIsolada = false,
                    CustoPagoApostaRodada = 5.00m,
                    PontuacaoTotalRodada = 0.00m,
                    Enviada = false
                }
                // Adicione outras apostas de rodada aqui
            );

            // Exemplo de Seed para Palpites (substitua pelos seus dados reais)
            modelBuilder.Entity<Palpite>().HasData(
                new Palpite
                {
                    Id = Guid.Parse("90000000-0000-0000-0000-000000000001"),
                    JogoId = Guid.Parse("70000000-0000-0000-0000-000000000001"), // Jogo Fla x Vasco
                    ApostaRodadaId = Guid.Parse("80000000-0000-0000-0000-000000000001"), // Aposta Admin R1
                    PlacarApostaCasa = 2,
                    PlacarApostaVisita = 1,
                    Pontos = 0.00m
                }
                // Adicione outros palpites aqui
            );

            // Exemplo de Seed para RankingRodadas (substitua pelos seus dados reais)
            modelBuilder.Entity<RankingRodada>().HasData(
                new RankingRodada
                {
                    Id = Guid.Parse("A0000000-0000-0000-0000-000000000001"),
                    RodadaId = Guid.Parse("60000000-0000-0000-0000-000000000001"), // Rodada 1
                    ApostadorCampeonatoId = Guid.Parse("30000000-0000-0000-0000-000000000001"), // Apostador Admin no Brasileirão
                    Pontuacao = 0,
                    Posicao = 0,
                    DataAtualizacao = DateTime.Now
                }
                // Adicione outros rankings aqui
            );

            // Exemplo de Seed para TransacoesFinanceiras (substitua pelos seus dados reais)
            // Use o SaldoId do Saldo do Apostador Admin
            modelBuilder.Entity<TransacaoFinanceira>().HasData(
                new TransacaoFinanceira
                {
                    Id = Guid.Parse("B0000000-0000-0000-0000-000000000001"), // Use seu GUID real
                    SaldoId = Guid.Parse("50000000-0000-0000-0000-000000000001"), // Saldo do Apostador Admin
                    ApostaRodadaId = null, // Pode ser nulo se não for transação de aposta
                    Valor = 50.00m,
                    Tipo = 0, // Ajuste conforme seu enum de tipo de transação (ex: 0 para depósito)
                    DataTransacao = DateTime.Now,
                    Descricao = "Depósito inicial via HasData"
                }
                // Adicione outras transacoes aqui
            );

            // --- FIM: DATA SEEDING ---
        }
        */

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // Lógica de auditoria ou outras operações antes de salvar
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
