using ApostasApp.Core.Domain.Models.Campeonatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.Infrastructure.Data.Mappings
{
    public class CampeonatoMapping : IEntityTypeConfiguration<Campeonato>
    {
        public void Configure(EntityTypeBuilder<Campeonato> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(c => c.DataInic)
                .IsRequired();

            builder.Property(c => c.DataFim)
                .IsRequired();

            builder.Property(c => c.NumRodadas)
                .IsRequired();

            builder.Property(c => c.Tipo)
                .IsRequired();

            // 1 : N => Campeonato : Rodadas
            builder.HasMany(r => r.Rodadas)
                .WithOne(c => c.Campeonato)
                .HasForeignKey(c => c.CampeonatoId);

            builder.ToTable("Campeonatos");
        }
    }
}