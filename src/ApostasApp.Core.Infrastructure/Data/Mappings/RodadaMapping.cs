using ApostasApp.Core.Domain.Models.Rodadas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.Infrastructure.Data.Mappings
{
    public class RodadaMapping : IEntityTypeConfiguration<Rodada>
    {
        public void Configure(EntityTypeBuilder<Rodada> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.NumeroRodada)
                .IsRequired();                

            builder.Property(r => r.DataInic)
                .IsRequired();

            builder.Property(r => r.DataFim)
                .IsRequired();

            builder.Property(r => r.NumJogos)
                .IsRequired();

            builder.Property(r => r.Status)
              .IsRequired();

            // 1 : N => Rodada : Jogos
            builder.HasMany(r => r.JogosRodada)
                .WithOne(j => j.Rodada)
                .HasForeignKey(j => j.RodadaId)
                .IsRequired(true);


            builder.HasMany(r => r.RankingRodadas)
             .WithOne(r => r.Rodada)
             .HasForeignKey(r => r.RodadaId)
             .IsRequired(true);

            
            builder.ToTable("Rodadas");
        }
    }
}