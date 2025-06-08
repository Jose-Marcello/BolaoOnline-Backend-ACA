using ApostasApp.Core.Domain.Models.Campeonatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class ApostadorCampeonatoMapping : IEntityTypeConfiguration<ApostadorCampeonato>
    {
        public void Configure(EntityTypeBuilder<ApostadorCampeonato> builder)
        {
            builder.HasKey(ec => ec.Id);

            // Relacionamento Apostador <-> ApostadorCampeonato
            builder.HasOne(bc => bc.Apostador)
                .WithMany(b => b.ApostadoresCampeonatos)
                .HasForeignKey(bc => bc.ApostadorId);

            // Relacionamento Campeonato <-> ApostadorCampeonato
            builder.HasOne(bc => bc.Campeonato)
                .WithMany(c => c.ApostadoresCampeonatos)
                .HasForeignKey(bc => bc.CampeonatoId);

            // REMOVER ESTE TRECHO INTEIRO, POIS A RELAÇÃO APOSTA x APOSTADORCAMPEONATO MUDOU
            // builder.HasMany(ac => ac.Apostas)
            //   .WithOne(a => a.ApostadorCampeonato)
            //   .HasForeignKey(a => a.ApostadorCampeonatoId)
            //   .IsRequired(true);

            // Relacionamento RankingRodada <-> ApostadorCampeonato (Este continua válido)
            builder.HasMany(r => r.RankingRodadas)
                .WithOne(a => a.ApostadorCampeonato)
                .HasForeignKey(a => a.ApostadorCampeonatoId)
                .IsRequired(true);

            builder.ToTable("ApostadoresCampeonatos");
        }
    }
}