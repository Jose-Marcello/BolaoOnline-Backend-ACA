using ApostasApp.Core.Domain.Models.Apostas; // Para ApostaRodada e Palpite
using ApostasApp.Core.Domain.Models.Apostadores; // Para Apostador
using ApostasApp.Core.Domain.Models.Rodadas;     // Para Rodada
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class ApostaRodadaMapping : IEntityTypeConfiguration<ApostaRodada>
    {
        public void Configure(EntityTypeBuilder<ApostaRodada> builder)
        {
            builder.HasKey(ar => ar.Id); // Define a chave primária

            // Mapeamento das propriedades com tipos explícitos e nulabilidade

            builder.Property(ar => ar.IdentificadorAposta)
                .HasColumnType("varchar(100)") // Sugestão: Definir tamanho para evitar nvarchar(max)
                .IsRequired(false); // Conforme sua entidade (nullable)

            builder.Property(ar => ar.DataHoraSubmissao)
                .IsRequired(false); // Conforme sua entidade (nullable)

            builder.Property(ar => ar.EhApostaCampeonato)
                .IsRequired(); // Booleans são geralmente not null por padrão, mas explícito é mais claro

            builder.Property(ar => ar.EhApostaIsolada)
                .IsRequired(); // Booleans são geralmente not null por padrão

            builder.Property(ar => ar.CustoPagoApostaRodada)
                .HasColumnType("decimal(18,2)") // Correto, você já tinha isso!
                .IsRequired(false); // Conforme sua entidade (nullable)

            // Ajuste aqui para decimal(18,2) e IsRequired()
            builder.Property(ar => ar.PontuacaoTotalRodada)
                .HasColumnType("decimal(18,2)") // Definir precisão e escala para decimal
                .IsRequired(); // Sugestão: Se a pontuação sempre será calculada, não deve ser nula

            builder.Property(ar => ar.Enviada)
                .IsRequired(); // Booleans são geralmente not null por padrão


            // Configuração dos relacionamentos (FKs)

            // Relação 1:N entre ApostadorCampeonato e ApostaRodada
            builder.HasOne(ar => ar.ApostadorCampeonato)
                .WithMany() // Use WithMany() se ApostadorCampeonato não tem ICollection<ApostaRodada>
                .HasForeignKey(ar => ar.ApostadorCampeonatoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Sugestão: Evitar exclusão em cascata acidental

            // Relação 1:N entre Rodada e ApostaRodada
            builder.HasOne(ar => ar.Rodada)
                .WithMany(r => r.ApostasRodada) // Confere se você adicionou 'public IEnumerable<ApostaRodada> ApostasRodada { get; set; }' em Rodada
                .HasForeignKey(ar => ar.RodadaId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Sugestão: Evitar exclusão em cascata acidental

            // Relação 1:N entre ApostaRodada e Palpite (Uma ApostaRodada tem muitos Palpites)
            builder.HasMany(ar => ar.Palpites)
                .WithOne(p => p.ApostaRodada)
                .HasForeignKey(p => p.ApostaRodadaId)
                .IsRequired() // Palpite sempre pertence a uma ApostaRodada
                .OnDelete(DeleteBehavior.Cascade); // Sugestão: Se a ApostaRodada for deletada, seus Palpites também podem ser

            builder.ToTable("ApostasRodada"); // Nome da tabela no banco de dados
        }
    }
}

/*


using ApostasApp.Core.Domain.Models.Apostas; // Para ApostaRodada e Palpite
using ApostasApp.Core.Domain.Models.Apostadores; // Para Apostador
using ApostasApp.Core.Domain.Models.Rodadas;     // Para Rodada
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class ApostaRodadaMapping : IEntityTypeConfiguration<ApostaRodada>
    {
        public void Configure(EntityTypeBuilder<ApostaRodada> builder)
        {
            builder.HasKey(ar => ar.Id); // Define a chave primária

            // Relação 1:N entre Apostador e ApostaRodada (Um Apostador pode ter muitas ApostaRodada)
            builder.HasOne(ar => ar.ApostadorCampeonato)
                .WithMany() // Se Apostador não tiver uma coleção de ApostaRodada (que geralmente não tem), use WithMany() sem parâmetro
                .HasForeignKey(ar => ar.ApostadorCampeonatoId)
                .IsRequired();

            // Relação 1:N entre Rodada e ApostaRodada (Uma Rodada pode ter muitas ApostaRodada)
            builder.HasOne(ar => ar.Rodada)
                .WithMany(r => r.ApostasRodada) // Supondo que você adicionará 'public IEnumerable<ApostaRodada> ApostasRodada { get; set; }' em Rodada, se ainda não o fez. Caso contrário, use WithMany()
                .HasForeignKey(ar => ar.RodadaId)
                .IsRequired();

            // Relação 1:N entre ApostaRodada e Palpite (Uma ApostaRodada tem muitos Palpites)
            builder.HasMany(ar => ar.Palpites)
                .WithOne(p => p.ApostaRodada)
                .HasForeignKey(p => p.ApostaRodadaId)
                .IsRequired();

            // Outras configurações de colunas, se necessário (ex: .HasColumnType("decimal(18,2)") para valores monetários)
            builder.Property(ar => ar.CustoPagoApostaRodada)
                   .HasColumnType("decimal(18,2)"); // Defina a precisão e escala para decimal

            // Mapeamento da tabela
            builder.ToTable("ApostasRodada"); // Nome da tabela no banco de dados
        }
    }
}*/