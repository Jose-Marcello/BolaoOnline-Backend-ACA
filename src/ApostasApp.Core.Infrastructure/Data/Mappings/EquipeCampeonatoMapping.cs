using ApostasApp.Core.Domain.Models.Campeonatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ApostasApp.Core.Infrastructure.Data.Mappings
{
    public class EquipeCampeonatoMapping : IEntityTypeConfiguration<EquipeCampeonato>
    {
        public void Configure(EntityTypeBuilder<EquipeCampeonato> builder)
        {

            //builder.HasAlternateKey(c => c.Id); //testando - para possibilitar gerar um Id apesar de não ser a chave da tablea

            //chave composta formada pelo Id das duas tabelas
            //builder.HasKey(x => new { x.EquipeId, x.CampeonatoId });


            builder.HasKey(ec => ec.Id);                         

            //relacionamento muitos para muitos
            //(Equipes->Campeonatos - usando uma table de junção : EquipesCampeonatos
            builder.HasOne(bc => bc.Equipe)
              .WithMany(b => b.EquipesCampeonatos)
               .HasForeignKey(bc => bc.EquipeId);
            builder.HasOne(bc => bc.Campeonato)
                .WithMany(c => c.EquipesCampeonatos)
                 .HasForeignKey(bc => bc.CampeonatoId);

            //relação Jogo x EquipeCampeonato (equipeCasa)
            builder.HasMany(ec => ec.JogosCasa)
              .WithOne(j => j.EquipeCasa)              
              .HasForeignKey(j => j.EquipeCasaId)
              .IsRequired(true);

            //relação Jogo x EquipeCampeonato (equipeVisitante)
            builder.HasMany(ec => ec.JogosVisitante)
              .WithOne(j => j.EquipeVisitante)
              .HasForeignKey(j => j.EquipeVisitanteId)
              .IsRequired(true);

            builder.ToTable("EquipesCampeonatos");
            
        }
    }
}