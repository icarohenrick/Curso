using Microsoft.EntityFrameworkCore;
using Curso.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Curso.Configurations
{
    #region TPH
    // public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    // {
    //     public void Configure(EntityTypeBuilder<Pessoa> builder)
    //     {
    //         builder.ToTable("Pessoas")
    //         .HasDiscriminator<int>("TipoPessoa")
    //         .HasValue<Pessoa>(3)
    //         .HasValue<Instrutor>(6)
    //         .HasValue<Aluno>(99);
    //     }
    // }
    #endregion

    #region TPP
    public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.ToTable("Pessoas");
        }
    }

    public class InstrutorConfiguration : IEntityTypeConfiguration<Instrutor>
    {
        public void Configure(EntityTypeBuilder<Instrutor> builder)
        {
            builder.ToTable("Instrutores");
        }
    }

    public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            builder.ToTable("Alunos");
        }
    }

    #endregion
}