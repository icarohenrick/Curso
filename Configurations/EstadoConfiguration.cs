using Microsoft.EntityFrameworkCore;
using Curso.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Curso.Configurations
{
    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder.HasOne(x => x.Governador)
            .WithOne(p => p.Estado)
            .HasForeignKey<Governador>(p => p.EstadoReference);

            builder.HasMany(x => x.Cidades)
            .WithOne()
            .IsRequired(false);
            //.OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(p => p.Governador).AutoInclude();
        }
    }
}