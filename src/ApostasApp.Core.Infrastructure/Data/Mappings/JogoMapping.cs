using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApostasApp.Core.Domain.Models.Jogos;


namespace ApostasApp.Core.InfraStructure.Data.Mappings
{

    public class JogoMapping : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.DataJogo)
                    .IsRequired();

            builder.Property(j => j.HoraJogo)
                    .IsRequired();

            // 1 : N => Jogo : Apostas
            builder.HasMany(j => j.Apostas)
              .WithOne(a => a.Jogo)
              .HasForeignKey(a => a.JogoId)
              .IsRequired(true);

            // 1 : N
            builder.ToTable("Jogos");
        }
    }
}
