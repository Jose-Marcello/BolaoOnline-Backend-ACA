using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Ufs;
using ApostasApp.Core.Domain.Models.Usuarios; // Para a entidade Usuario
using Microsoft.AspNetCore.Identity; // Para IdentityRole
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Para IdentityDbContext
using Microsoft.EntityFrameworkCore;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.RankingRodadas;
using ApostasApp.Core.Domain.Models.Financeiro;
using System.Linq; // Para SelectMany
using System.Threading; // Para CancellationToken
using System.Threading.Tasks; // Para Task
using System; // Para DateTime, GetProperty

namespace ApostasApp.Core.InfraStructure.Data.Context
{
    // O DbContext herda de IdentityDbContext, especificando o tipo de usuário, papel e a chave (string)
    public class MeuDbContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        public MeuDbContext(DbContextOptions<MeuDbContext> options)
            : base(options)
        {
        }

        // DbSets para suas entidades personalizadas
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
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // COMENTE TEMPORARIAMENTE ESTE BLOCO INTEIRO PARA TESTE
                // Isso vai desativar a lógica de tratamento de RegistrationDate
                /*
                var registrationDateProperty = entry.Entity.GetType().GetProperty("RegistrationDate");

                if (registrationDateProperty != null)
                {
                    if (entry.State == EntityState.Added)
                    {
                        registrationDateProperty.SetValue(entry.Entity, DateTime.Now);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        // Essa linha pode estar causando o problema ao desmarcar a entidade
                        entry.Property("RegistrationDate").IsModified = false; 
                    }
                }
                */
            }
            // Chama a implementação base de SaveChangesAsync para persistir as alterações
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
