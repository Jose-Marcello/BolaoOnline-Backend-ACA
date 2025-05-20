using ApostasApp.Core.Domain.Models.Equipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class EquipeMapping : IEntityTypeConfiguration<Equipe>
    {
        public void Configure(EntityTypeBuilder<Equipe> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                .IsRequired()  
                .HasColumnType("varchar(40)");

            // 0 : N => Uf : Jogos - uma equipe pode não ter UF (EX: Seleções)

            /*builder.HasMany(e => e.JogosCasa)
              .WithOne(j => j.EquipeCasa)
              .HasForeignKey(j => j.EquipeCasaId)
              .IsRequired(true);*/
         
            /*
            builder.HasMany(e => e.JogosVisita)
              .WithOne(j => j.EquipeVisitante)
              .HasForeignKey(j => j.EquipeVisitanteId)
              .IsRequired(true);*/


            builder.ToTable("Equipes");
        }
    }
}