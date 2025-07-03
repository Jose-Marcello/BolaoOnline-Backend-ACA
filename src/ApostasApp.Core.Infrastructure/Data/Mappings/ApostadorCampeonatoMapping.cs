// ApostasApp.Core.InfraStructure.Data.Mappings/ApostadorCampeonatoMapping.cs
using ApostasApp.Core.Domain.Models.Campeonatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class ApostadorCampeonatoMapping : IEntityTypeConfiguration<ApostadorCampeonato>
    {
        public void Configure(EntityTypeBuilder<ApostadorCampeonato> builder)
        {
            // Define a chave primária no Id (já deve ser um GUID gerado pela aplicação)
            builder.HasKey(ac => ac.Id);
            builder.Property(ac => ac.Id)
                   .ValueGeneratedNever(); // O Id é gerado pela aplicação, não pelo banco.

            // Define um ÍNDICE ÚNICO na combinação ApostadorId e CampeonatoId.
            // Isso garante que um apostador só possa aderir a um campeonato UMA VEZ.
            // IMPORTANTE: Isso não é uma Chave Primária, mas uma restrição de unicidade.
            builder.HasIndex(ac => new { ac.ApostadorId, ac.CampeonatoId })
                   .IsUnique();

            // Relacionamento Apostador <-> ApostadorCampeonato
            builder.HasOne(ac => ac.Apostador)
                .WithMany(a => a.ApostadoresCampeonatos)
                .HasForeignKey(ac => ac.ApostadorId)
                .IsRequired(); // Garante que a FK seja obrigatória

            // Relacionamento Campeonato <-> ApostadorCampeonato
            builder.HasOne(ac => ac.Campeonato)
                .WithMany(c => c.ApostadoresCampeonatos)
                .HasForeignKey(ac => ac.CampeonatoId)
                .IsRequired(); // Garante que a FK seja obrigatória

            // Relacionamento RankingRodada <-> ApostadorCampeonato
            builder.HasMany(ac => ac.RankingRodadas)
                .WithOne(rr => rr.ApostadorCampeonato)
                .HasForeignKey(rr => rr.ApostadorCampeonatoId)
                .IsRequired(true);

            // Mapeamento das propriedades com NOT NULL
            builder.Property(ac => ac.Pontuacao)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(ac => ac.Posicao)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(ac => ac.DataInscricao)
                   .IsRequired(); // Assumimos que DateTime.Now é setado no construtor da entidade

            builder.Property(ac => ac.CustoAdesaoPago)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.ToTable("ApostadoresCampeonatos");
        }
    }
}
