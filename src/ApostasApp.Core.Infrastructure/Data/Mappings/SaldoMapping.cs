// Em ApostasApp.Core.InfraStructure.Data.Mappings/SaldoMapping.cs

using ApostasApp.Core.Domain.Models.Financeiro; // Para Saldo
using ApostasApp.Core.Domain.Models.Apostadores; // Para Apostador
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class SaldoMapping : IEntityTypeConfiguration<Saldo>
    {
        public void Configure(EntityTypeBuilder<Saldo> builder)
        {
            builder.HasKey(s => s.Id); // Chave primária

            builder.Property(s => s.Valor)
                   .HasColumnType("decimal(18,2)") // Essencial para decimais!
                   .IsRequired(); // Saldo deve ter um valor

            builder.Property(s => s.DataUltimaAtualizacao)
                   .IsRequired(); // Data de atualização é obrigatória

            // Relacionamento 1:1 entre Apostador e Saldo
            // Um Apostador TEM UM Saldo. Um Saldo PERTENCE A UM Apostador.
            builder.HasOne(s => s.Apostador) // O Saldo tem um Apostador
                   .WithOne(a => a.Saldo)   // O Apostador tem um Saldo
                   .HasForeignKey<Saldo>(s => s.ApostadorId) // A FK está na tabela Saldo
                   .IsRequired(); // Um Saldo sempre deve estar ligado a um Apostador

            // Relacionamento 1:N entre Saldo e TransacaoFinanceira
            // Um Saldo pode ter muitas TransacoesFinanceiras
            //builder.HasMany(s => s.Transacoes) // Um Saldo tem muitas Transacoes
            //       .WithOne(tf => tf.Saldo)    // Uma Transacao tem um Saldo
            //       .HasForeignKey(tf => tf.SaldoId) // A FK SaldoId está na tabela TransacoesFinanceiras
            //       .IsRequired(); // Uma Transação sempre deve estar ligada a um Saldo

            builder.ToTable("Saldos"); // Nome da tabela no banco
        }
    }
}