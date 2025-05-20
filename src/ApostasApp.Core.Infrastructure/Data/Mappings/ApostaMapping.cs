using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApostasApp.Core.Domain.Models.Apostas;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class ApostaMapping : IEntityTypeConfiguration<Aposta>
    {
        public void Configure(EntityTypeBuilder<Aposta> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(j => j.DataHoraAposta);
            //.IsRequired();           

            builder.ToTable("Apostas");
        }
    }
}

