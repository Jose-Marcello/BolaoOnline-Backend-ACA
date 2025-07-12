// EM ApostasApp.Core.InfraStructure.Data.Mappings/ApostadorMapping.cs COMPLETO

using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Financeiro; // Para Saldo
using ApostasApp.Core.Domain.Models.Usuarios; // Para Usuario (Identity)
using ApostasApp.Core.Domain.Models.Campeonatos; // Para ApostadorCampeonato
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class ApostadorMapping : IEntityTypeConfiguration<Apostador>
    {
        public void Configure(EntityTypeBuilder<Apostador> builder)
        {
            builder.HasKey(a => a.Id); // Usar 'a' para consistência

            // Mapeamento das propriedades
            builder.Property(a => a.NomeCompleto)
                   .HasColumnType("varchar(250)") // Exemplo de definição de tipo e tamanho
                   .IsRequired();
                        

            builder.Property(a => a.Status)
                   .IsRequired(); // Enum será mapeado como int por padrão

            // Relação 1:1 com Saldo
            builder.HasOne(a => a.Saldo)
                   .WithOne(s => s.Apostador)
                   .HasForeignKey<Saldo>(s => s.ApostadorId) // A FK está na tabela Saldo
                   .IsRequired();

            // Relação 1:1 (ou 1:N se um usuário puder ser vários apostadores, mas geralmente é 1:1) com Usuario (Identity)
            builder.HasOne(a => a.Usuario)
                   .WithMany() // ou WithOne(), dependendo de como ApplicationUser mapeia Apostador
                   .HasForeignKey(a => a.UsuarioId) // A FK está na tabela Apostador
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); // Exemplo: se deletar o usuário, deleta o apostador. Escolha a sua regra.

            // Relação N:N (muitos-para-muitos) com Campeonato através da tabela ApostadorCampeonato
            builder.HasMany(a => a.ApostadoresCampeonatos)
                   .WithOne(ac => ac.Apostador)
                   .HasForeignKey(ac => ac.ApostadorId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); // Exemplo: se deletar o apostador, deleta as associações.

            builder.ToTable("Apostadores");
        }
    }
}