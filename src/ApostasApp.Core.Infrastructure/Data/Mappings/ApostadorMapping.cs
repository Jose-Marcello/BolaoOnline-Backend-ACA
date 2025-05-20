using ApostasApp.Core.Domain.Models.Apostadores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApostasApp.Core.InfraStructure.Data.Mappings
{
    public class ApostadorMapping : IEntityTypeConfiguration<Apostador>
    {
        public void Configure(EntityTypeBuilder<Apostador> builder)
        {
            builder.HasKey(c => c.Id);

           
            builder.ToTable("Apostadores");
        }
    }
}