using Microsoft.EntityFrameworkCore;
using Curso.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Curso.Configurations
{
    public class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
    {
        public void Configure(EntityTypeBuilder<Documento> builder)
        {
            //builder.Property(p => p.CPF).HasField("_cpf");
            builder.Property("_cpf").HasColumnName("CPF").HasMaxLength(15);
        }
    }
}