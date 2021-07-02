using System;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.IO;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Curso.Configurations;
using System.Reflection;
using System.Collections.Generic;
using Curso.Funcoes;
using System.Linq;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {

        //public DbSet<Livro> Livros { get; set; } 
        //public DbSet<Funcao> Funcoes { get; set; }

        #region Parte Anetior ao EF Functions
        //private readonly StreamWriter _writer = new StreamWriter("Meu_log_do_ef_core.txt", append:true);

        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        // public DbSet<Estado> Estados { get; set; }
        // public DbSet<Conversor> Conversores { get; set; }
        // public DbSet<Cliente> Clientes { get; set; }
        // public DbSet<Ator> Atores { get; set; }
        // public DbSet<Filme> Filmes { get; set; }
        // public DbSet<Documento> Documentos { get; set; }

        // public DbSet<Pessoa> Pessoas { get; set; }
        // public DbSet<Instrutor> Instrutores { get; set; }
        // public DbSet<Aluno> Alunos { get; set; }

        // public DbSet<Dictionary<string, object>> Configuracoes => Set<Dictionary<string, object>>("Configuracoes");

        // public DbSet<Attributo> Attributos { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=(localdb)\\mssqllocaldb; Initial Catalog=CursoDEVIO-02; Integrated Security=true; pooling=true; MultipleActiveResultSets=true;";

            optionsBuilder
                .UseSqlServer(strConnection)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution)
                // .AddInterceptors(new Interceptadores.InterceptadoresDeComandos())
                // .AddInterceptors(new Interceptadores.InterceptadoresDeConexao())
                // .AddInterceptors(new Interceptadores.InterceptadoresPersistencia())
                //.UseSqlServer(strConnection, p=> p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                // .UseSqlServer(strConnection, o => o
                //                             .MaxBatchSize(100)
                //                             .CommandTimeout(5)
                //                             .EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null))
                //.UseSqlServer(strConnection)
                //.EnableSensitiveDataLogging()
                //.UseLazyLoadingProxies()
                
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
                ;
        }

        #region UDF
        // [DbFunction(Name = "LEFT", IsBuiltIn = true)]
        // public static string Left(string dados, int quantidade)
        // {
        //     throw new NotImplementedException();
        // }
        #endregion

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
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // modelBuilder.HasDefaultSchema("cadastros");

        //     // modelBuilder.Entity<Estado>().ToTable("Estados", "SegundoEsquema");

        //     var conversao = new ValueConverter<Versao, string>(
        //         p => p.ToString(), p => (Versao)Enum.Parse(typeof(Versao), p)
        //     );

        //      var conversao1 = new EnumToStringConverter<Versao>();

        //     modelBuilder.Entity<Conversor>()
        //         .Property(p => p.Versao)
        //         .HasConversion(conversao1);
        //         //.HasConversion(conversao);
        //         //.HasConversion(p => p.ToString(), p => (Versao)Enum.Parse(typeof(Versao), p));
        //         //.HasConversion<string>();

        //     modelBuilder.Entity<Conversor>()
        //         .Property(p => p.Status)
        //         .HasConversion(new Curso.Conversores.ConversorCustomizado());
        // }
        #endregion

        #region Properties Shadows
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Departamento>()
        //         .Property<DateTime>("UltimaAtualizacao");
        // }
        #endregion

        #region Owned Types
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region UDF

            //Curso.Funcoes.MinhasFuncoes.RegistrarFuncoes(modelBuilder);

            // modelBuilder.HasDbFunction(
            //     typeof(MinhasFuncoes)
            //     .GetRuntimeMethod("Left", new[] {typeof(string), typeof(int) }))
            //     .HasName("LEFT")
            //     .IsBuiltIn();

            modelBuilder
                .HasDbFunction(_minhaFuncao)
                .HasName("LEFT")
                .IsBuiltIn();

            modelBuilder
                .HasDbFunction(_letrasMaiusculas)
                .HasName("ConverterParaLetrasMaiusculas")
                .HasSchema("dbo");

            modelBuilder
                .HasDbFunction(_dateDiff)
                .HasName("DATEDIFF")
                .HasTranslation(p =>
                {
                    var argumentos = p.ToList();

                    var constante = (SqlConstantExpression)argumentos[0];
                    argumentos[0] = new  SqlFragmentExpression(constante.Value.ToString());

                    return new SqlFunctionExpression(
                        "DATEDIFF", //Função
                        argumentos, //Parametros mapeados
                        false, //Função Retorna valor nulo
                        new[] {false, false, false}, //Parametros podem ser nulos
                        typeof(int), // Tipo de Retorno
                        null); //Algum mapeamento adicional?
                })
                .IsBuiltIn();

            #endregion
            // modelBuilder.Entity<Cliente>(p => 
            // {
            //     p.OwnsOne(x => x.Endereco, end=> 
            //     {
            //         end.Property(p => p.Bairro).HasColumnName("Bairro");

            //         end.ToTable("Enderecos");
            //     });
            // });

            //Aplicação um a um
            //modelBuilder.ApplyConfiguration(new ClienteConfiguration());
            
            //From Assembly Exmeplo 01
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //From Assembly Exmeplo 02
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            // modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Configuracoes", b => 
            // {
            //     b.Property<int>("Id");

            //     b.Property<string>("Chave")
            //         .HasColumnType("VARCHAR(40)")
            //         .IsRequired();

            //     b.Property<string>("Valor")
            //         .HasColumnType("VARCHAR(255)")
            //         .IsRequired();
            // });
        
            // modelBuilder.Entity<Funcao>(conf =>
            // {
            //     conf.Property<string>("PropriedadeDeSombra")
            //         .HasColumnType("VARCHAR(100)")
            //         .HasDefaultValueSql("'Teste'");
            // });
        }
        #endregion

        private static MethodInfo _minhaFuncao = typeof(MinhasFuncoes)
            .GetRuntimeMethod("Left", new[] {typeof(string), typeof(int) });

        private static MethodInfo _letrasMaiusculas = typeof(MinhasFuncoes)
            .GetRuntimeMethod(nameof(MinhasFuncoes.LetrasMaiusculas), new[] {typeof(string) });

        private static MethodInfo _dateDiff = typeof(MinhasFuncoes)
            .GetRuntimeMethod(nameof(MinhasFuncoes.DateDiff), new[] {typeof(string), typeof(DateTime), typeof(DateTime) });
    }
}