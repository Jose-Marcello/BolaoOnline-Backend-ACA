// Em ApostasApp.Core.InfraStructure.Data.Mappings/TransacaoFinanceiraMapping.cs
using ApostasApp.Core.Domain.Models.Financeiro;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
// Lembre-se de adicionar este using se já não estiver lá:
using ApostasApp.Core.Domain.Models.Apostadores;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class TransacaoFinanceiraMapping : IEntityTypeConfiguration<TransacaoFinanceira>
    {
        public void Configure(EntityTypeBuilder<TransacaoFinanceira> builder)
        {
            builder.HasKey(tf => tf.Id);

            builder.Property(tf => tf.Valor)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(tf => tf.Tipo)
                   .IsRequired();

            builder.Property(tf => tf.DataTransacao)
                   .IsRequired();

            builder.Property(tf => tf.Descricao)
                   .HasColumnType("varchar(250)")
                   .IsRequired(false);
                       
            // NOVOS RELACIONAMENTOS OPCIONAIS (esses já estão corretos)
            builder.HasOne(tf => tf.Campeonato)
                   .WithMany()
                   .HasForeignKey(tf => tf.CampeonatoId)
                   .IsRequired(false);

            builder.HasOne(tf => tf.Rodada)
                   .WithMany()
                   .HasForeignKey(tf => tf.RodadaId)
                   .IsRequired(false);

            builder.ToTable("TransacoesFinanceiras");
        }
    }
}