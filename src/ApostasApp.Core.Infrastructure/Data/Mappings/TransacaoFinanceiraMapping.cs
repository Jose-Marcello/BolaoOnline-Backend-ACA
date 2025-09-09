// Localização: ApostasApp.Core.InfraStructure.Data.Mappings/TransacaoFinanceiraMapping.cs
using ApostasApp.Core.Domain.Models.Financeiro;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class TransacaoFinanceiraMapping : IEntityTypeConfiguration<TransacaoFinanceira>
    {
        public void Configure(EntityTypeBuilder<TransacaoFinanceira> builder)
        {
            builder.HasKey(tf => tf.Id);
            builder.Property(tf => tf.Id)
                   .ValueGeneratedNever(); // Id é gerado pela aplicação, não pelo banco

            builder.Property(tf => tf.Valor)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(tf => tf.Tipo)
                   .IsRequired();

            builder.Property(tf => tf.DataTransacao)
                   .IsRequired();

            builder.Property(tf => tf.Descricao)
                   .HasColumnType("varchar(250)")
                   .IsRequired();

            // Configuração da Chave Estrangeira para Saldo (já correta)
            builder.HasOne(tf => tf.Saldo)
                   .WithMany()
                   .HasForeignKey(tf => tf.SaldoId)
                   .IsRequired();

            // >>> NOVA CONFIGURAÇÃO DA CHAVE ESTRANGEIRA PARA APOSTARODADA <<<
            builder.HasOne(tf => tf.ApostaRodada) // TransacaoFinanceira pode ter uma ApostaRodada
                   .WithMany() // ApostaRodada pode ter muitas TransacoesFinanceiras
                   .HasForeignKey(tf => tf.ApostaRodadaId) // A propriedade ApostaRodadaId é a FK
                   .IsRequired(false); // Esta FK é OPCIONAL (nullable)

            builder.ToTable("TransacoesFinanceiras");
        }
    }
}
