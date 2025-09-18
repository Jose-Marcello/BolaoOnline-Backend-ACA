using ApostasApp.Core.Domain.Models.RankingRodadas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.Infrastructure.Data.Mappings
{
    public class RankingRodadaMapping : IEntityTypeConfiguration<RankingRodada>
    {
        public void Configure(EntityTypeBuilder<RankingRodada> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Pontuacao)
                .IsRequired();

            builder.Property(r => r.Posicao)
                .IsRequired();

            builder.Property(r => r.DataAtualizacao)
                .IsRequired();
                      

           /* // 1 : 1 => RankingRodada : ApostadorCampeonato
            builder.HasOne(r => r.ApostadorCampeonato)
                .WithOne()
                .HasForeignKey<RankingRodada>(r => r.ApostadorCampeonatoId)
                .IsRequired(true);

            builder.ToTable("RankingRodadas");*/
        }
    }
}