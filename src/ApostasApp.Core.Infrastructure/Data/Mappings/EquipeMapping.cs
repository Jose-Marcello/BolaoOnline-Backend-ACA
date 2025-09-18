using ApostasApp.Core.Domain.Models.Equipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.Infrastructure.Data.Mapping // Verifique seu namespace de mapeamento
{
    public class EquipeMapping : IEntityTypeConfiguration<Equipe>
    {
        public void Configure(EntityTypeBuilder<Equipe> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                .IsRequired()
                .HasColumnType("varchar(100)"); // Ajuste o tamanho se necessário

            builder.Property(e => e.Sigla)
                .IsRequired()
                .HasColumnType("varchar(10)"); // Ajuste o tamanho se necessário

            builder.Property(e => e.Tipo)
                .IsRequired();

            builder.Property(e => e.Escudo) // Mapeamento da nova coluna
                .HasColumnType("varchar(255)") // Defina um tamanho adequado para o caminho
                .IsRequired(false); // Define como opcional (null na base)

            // Configuração da chave estrangeira para UF
            builder.HasOne(e => e.Uf)
                   .WithMany() // Se Uf não tem uma coleção de Equipes
                   .HasForeignKey(e => e.UfId)
                   .OnDelete(DeleteBehavior.NoAction); // Ou .Restrict, dependendo da sua regra de negócio
        }
    }
}
