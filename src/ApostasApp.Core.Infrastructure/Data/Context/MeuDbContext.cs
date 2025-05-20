using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Ufs;
using ApostasApp.Core.Domain.Models.Usuarios;
using DApostasApp.Core.Domain.Models.RankingRodadas;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Models.Jogos;

namespace ApostasApp.Core.InfraStructure.Data.Context
{
    public class MeuDbContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        public MeuDbContext(DbContextOptions<MeuDbContext> options)
            : base(options)
        {
        }
        // Podemos adicionar aqui outros DbSet para nossas entidades personalizadas

        public DbSet<Campeonato> Campeonatos { get; set; }
        public DbSet<Rodada> Rodadas { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Uf> Ufs { get; set; }
        public DbSet<EquipeCampeonato> EquipesCampeonatos { get; set; }
        public DbSet<Estadio> Estadios { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Apostador> Apostadores { get; set; }
        public DbSet<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }
        public DbSet<Aposta> Apostas { get; set; }
        public DbSet<RankingRodada> RankingRodadas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeuDbContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
