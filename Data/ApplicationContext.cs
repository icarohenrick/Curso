using System;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter _writer = new StreamWriter("Meu_log_do_ef_core.txt", append:true);

        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Conversor> Conversores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=(localdb)\\mssqllocaldb; Initial Catalog=CursoDEVIO-02; Integrated Security=true; pooling=true; MultipleActiveResultSets=true;";

            optionsBuilder
                .UseSqlServer(strConnection)
                //.UseSqlServer(strConnection, p=> p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                // .UseSqlServer(strConnection, o => o
                //                             .MaxBatchSize(100)
                //                             .CommandTimeout(5)
                //                             .EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null))
                //.UseSqlServer(strConnection)
                //.EnableSensitiveDataLogging()
                //.UseLazyLoadingProxies()
                .LogTo(Console.WriteLine, LogLevel.Information)
                /*.LogTo(
                    Console.WriteLine, 
                    new [] { CoreEventId.ContextInitialized, RelationalEventId.CommandExecuted },
                    LogLevel.Information,
                    DbContextLoggerOptions.LocalTime | DbContextLoggerOptions.SingleLine 
                );*/
                // .LogTo(_writer.WriteLine, new [] { CoreEventId.ContextInitialized, RelationalEventId.CommandExecuted },
                //     LogLevel.Information,
                //     DbContextLoggerOptions.LocalTime | DbContextLoggerOptions.SingleLine )
                //.EnableDetailedErrors(true)
                .EnableSensitiveDataLogging()
                ;
        }

        #region Log
        // public override void Dispose()
        // {
        //     base.Dispose();
        //     _writer.Dispose();
        // }
        #endregion
        
        #region Filtro Global
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     //Filtro Global
        //     //modelBuilder.Entity<Departamento>().HasQueryFilter(p => !p.Excluido);
        // }
        #endregion

        #region Collate e Sequence
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");
        //     //CI = Não Use Case Sensitive => Icaro == icaro
        //     //AI = Não use Sensitive de Acentuação => Ícaro == Icaro

        //     modelBuilder.Entity<Departamento>().Property(p => p.Descricao).UseCollation("SQL_Latin1_General_CP1_CS_AS");
        //     //CS = Use Case Sensitive => Icaro != icaro
        //     //AS = Use Sensitive de Acentuação => Ícaro != Icaro

        //     modelBuilder.HasSequence<int>("MinhaSequencia", "sequencias") //Tipos aceitos => int, byte e long (padrão) 
        //     .StartsAt(1) //Inicia em
        //     .IncrementsBy(2) //Incrementa de 2 em 2
        //     .HasMin(1) // Minimo
        //     .HasMax(10) //Maximo (chegouno maximo da exception ou vai pro cyclic)
        //     .IsCyclic(); // Reseta a contagem

        //     modelBuilder.Entity<Departamento>().Property(p => p.Id).HasDefaultValueSql("NEXT VALUE FOR sequencias.MinhaSequencia");
        // }

        #endregion
    
        #region Indices
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Departamento>()
        //     //.HasIndex(p => p.Descricao);
        //     .HasIndex(p => new { p.Descricao, p.Ativo })
        //     .HasDatabaseName("idx_meu_indice_composto")
        //     .HasFilter("Descricao IS NOT NULL")
        //     .HasFillFactor(80) //20% para uso de preenchimento de dados pelo SQL Server
        //     .IsUnique();
        // }
        #endregion

        #region Propagação de Dados
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Estado>().HasData(new[] 
        //     {
        //         new Estado {Id = 1, Nome = "São Paulo" }, 
        //         new Estado {Id = 2, Nome = "Minas Gerais" }
        //     });
        // }
        #endregion

        #region Esquemas e Conversores
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.HasDefaultSchema("cadastros");

            // modelBuilder.Entity<Estado>().ToTable("Estados", "SegundoEsquema");

            var conversao = new ValueConverter<Versao, string>(
                p => p.ToString(), p => (Versao)Enum.Parse(typeof(Versao), p)
            );

             var conversao1 = new EnumToStringConverter<Versao>();

            modelBuilder.Entity<Conversor>()
                .Property(p => p.Versao)
                .HasConversion(conversao1);
                //.HasConversion(conversao);
                //.HasConversion(p => p.ToString(), p => (Versao)Enum.Parse(typeof(Versao), p));
                //.HasConversion<string>();

            modelBuilder.Entity<Conversor>()
                .Property(p => p.Status)
                .HasConversion(new Curso.Conversores.ConversorCustomizado());
        }
        #endregion
    }
}