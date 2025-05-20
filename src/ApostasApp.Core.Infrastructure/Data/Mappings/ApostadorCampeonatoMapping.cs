using ApostasApp.Core.Domain.Models.Campeonatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class ApostadorCampeonatoMapping : IEntityTypeConfiguration<ApostadorCampeonato>
    {
        public void Configure(EntityTypeBuilder<ApostadorCampeonato> builder)
        {                     

            //chave composta formada pelo Id das duas tabelas
            //builder.HasKey(x => new { x.EquipeId, x.CampeonatoId });
            //Usar INDICE para garantir a unicidade - além da validação

            builder.HasKey(ec => ec.Id);                         

            //relacionamento muitos para muitos
            //(Apostadores->Campeonatos - usando uma table de junção : ApostadoresCampeonatos
            builder.HasOne(bc => bc.Apostador)
              .WithMany(b => b.ApostadoresCampeonatos)
               .HasForeignKey(bc => bc.ApostadorId);           
            builder.HasOne(bc => bc.Campeonato)
                .WithMany(c => c.ApostadoresCampeonatos)
                 .HasForeignKey(bc => bc.CampeonatoId);       

            //relação Aposta x ApostadorCampeonato
            builder.HasMany(ac => ac.Apostas)
              .WithOne(a => a.ApostadorCampeonato)
              .HasForeignKey(a => a.ApostadorCampeonatoId)
              .IsRequired(true);

            builder.HasMany(r => r.RankingRodadas)
             .WithOne(a => a.ApostadorCampeonato)
             .HasForeignKey(a => a.ApostadorCampeonatoId)
             .IsRequired(true);


            builder.ToTable("ApostadoresCampeonatos");

            
        }
    }
}