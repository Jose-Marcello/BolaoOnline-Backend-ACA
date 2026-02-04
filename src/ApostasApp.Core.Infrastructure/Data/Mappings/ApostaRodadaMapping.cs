// ApostasApp.Core.Infrastructure.Data.Mappings/ApostaRodadaMapping.cs
using ApostasApp.Core.Domain.Models.Apostas; // Para ApostaRodada e Palpite
using ApostasApp.Core.Domain.Models.Campeonatos; // <<-- ADICIONADO: Para ApostadorCampeonato
using ApostasApp.Core.Domain.Models.RankingRodadas;
using ApostasApp.Core.Domain.Models.Rodadas;      // Para Rodada
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.Infrastructure.Data.Mappings
{
    public class ApostaRodadaMapping : IEntityTypeConfiguration<ApostaRodada>
    {
        public void Configure(EntityTypeBuilder<ApostaRodada> builder)
        {
            builder.HasKey(ar => ar.Id); // Define a chave primária

            // Mapeamento das propriedades com tipos explícitos e nulabilidade
            builder.Property(ar => ar.IdentificadorAposta)
                .HasColumnType("varchar(100)")
                .IsRequired(false);

            builder.Property(ar => ar.DataCriacao) // <<-- ADICIONADO: Mapeamento para DataCriacao
                .IsRequired();

            builder.Property(ar => ar.DataHoraSubmissao)
                .IsRequired(false);

            builder.Property(ar => ar.EhApostaCampeonato)
                .IsRequired();

            builder.Property(ar => ar.EhApostaIsolada)
                .IsRequired();

            builder.Property(ar => ar.CustoPagoApostaRodada)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            builder.Property(ar => ar.PontuacaoTotalRodada)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(ar => ar.Enviada)
                .IsRequired();

            // Configuração dos relacionamentos (FKs)

            // Relação 1:N entre ApostadoresCampeonatos e ApostaRodada
            // Uma ApostadorCampeonato pode ter muitas ApostaRodada
            builder.HasOne(ar => ar.ApostadorCampeonato) // <<-- AGORA APONTA PARA ApostadorCampeonato
                .WithMany(ac => ac.ApostasRodada) // <<-- Confere se você adicionou ICollection<ApostaRodada> ApostasRodada { get; set; } em ApostadorCampeonato.cs
                .HasForeignKey(ar => ar.ApostadorCampeonatoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

           // Configuração do Relacionamento 1:N (Apostador -> ApostasRodada)
           builder.HasOne(ar => ar.Apostador)
                  .WithMany(a => a.ApostasRodadas) // Certifique-se que essa ICollection existe na classe Apostador
                  .HasForeignKey(ar => ar.ApostadorId)
                  .OnDelete(DeleteBehavior.Restrict); // Recomendado para evitar deleções em cascata acidentais

          // Relação 1:N entre Rodada e ApostaRodada
          builder.HasOne(ar => ar.Rodada)
                .WithMany(r => r.ApostasRodada)
                .HasForeignKey(ar => ar.RodadaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

         // Relação 1:N entre ApostaRodada e Palpite
         builder.HasMany(ar => ar.Palpites)
                .WithOne(p => p.ApostaRodada)
                .HasForeignKey(p => p.ApostaRodadaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


         // Relação 1:1 entre ApostaRodada e RankingRodada (deixando apenas do outro lado)
         /*builder.HasOne(ar => ar.RankingRodada)
          .WithOne(rr => rr.ApostaRodada)
          .HasForeignKey<RankingRodada>(rr => rr.ApostaRodadaId) // Chave estrangeira fica no Ranking
          .OnDelete(DeleteBehavior.Cascade); // Se deletar a aposta, deleta o ranking correspondente
         */

      builder.ToTable("ApostasRodada"); // Nome da tabela no banco de dados
        }
    }
}
