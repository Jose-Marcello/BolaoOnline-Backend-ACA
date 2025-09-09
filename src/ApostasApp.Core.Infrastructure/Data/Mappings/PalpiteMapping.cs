using ApostasApp.Core.Domain.Models.Apostas; // Para Palpite e ApostaRodada
using ApostasApp.Core.Domain.Models.Jogos;      // Para Jogo
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class PalpiteMapping : IEntityTypeConfiguration<Palpite>
    {
        public void Configure(EntityTypeBuilder<Palpite> builder)
        {
            builder.HasKey(p => p.Id); // Define a chave primária

            // Mapeamento das propriedades com tipos explícitos e nulabilidade

            builder.Property(p => p.PlacarApostaCasa)
                .IsRequired(false); // Conforme sua entidade (nullable)

            builder.Property(p => p.PlacarApostaVisita)
                .IsRequired(false); // Conforme sua entidade (nullable)

            // Ajuste aqui para decimal(18,2) e IsRequired()
            builder.Property(p => p.Pontos)
                .HasColumnType("decimal(18,2)") // Definir precisão e escala para decimal
                .IsRequired(); // Sugestão: Pontuação deve ser sempre preenchida


            // Relação 1:N entre Jogo e Palpite (Um Jogo pode ter muitos Palpites)
            builder.HasOne(p => p.Jogo)
                .WithMany(j => j.Palpites) // Confere se você adicionou 'public IEnumerable<Palpite> Palpites { get; set; }' em Jogo
                .HasForeignKey(p => p.JogoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Sugestão: Evitar exclusão em cascata acidental

            // A relação N:1 de Palpite para ApostaRodada já foi configurada em ApostaRodadaMapping
            // Não precisa repetir o HasOne. A navegação inversa é suficiente.

            builder.ToTable("Palpites"); // Nome da tabela no banco de dados
        }
    }
}


/*

using ApostasApp.Core.Domain.Models.Apostas; // Para Palpite e ApostaRodada
using ApostasApp.Core.Domain.Models.Jogos;     // Para Jogo
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class PalpiteMapping : IEntityTypeConfiguration<Palpite>
    {
        public void Configure(EntityTypeBuilder<Palpite> builder)
        {
            builder.HasKey(p => p.Id); // Define a chave primária

            // Relação 1:N entre Jogo e Palpite (Um Jogo pode ter muitos Palpites)
            // Note: Se na sua classe Jogo você não tiver uma coleção de Palpites (ex: public IEnumerable<Palpite> Palpites { get; set; }),
            // pode usar WithMany() sem parâmetro. Recomendo adicionar para facilitar consultas.
            builder.HasOne(p => p.Jogo)
            .WithMany(j => j.Palpites) // Adicione j => j.Palpites AQUI!
            .HasForeignKey(p => p.JogoId)
            .IsRequired();

            // A relação N:1 de Palpite para ApostaRodada já foi configurada em ApostaRodadaMapping
            // builder.HasOne(p => p.ApostaRodada)
            //    .WithMany(ar => ar.Palpites) // Esta parte já está no ApostaRodadaMapping
            //    .HasForeignKey(p => p.ApostaRodadaId)
            //    .IsRequired();

            // Outras configurações de colunas, se necessário
            // Exemplo: builder.Property(p => p.PlacarApostaCasa).IsRequired();

            // Mapeamento da tabela
            builder.ToTable("Palpites"); // Nome da tabela no banco de dados
        }
    }
*/