using System;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsultaViaProcedure();
        }
        #region Terceira Parte do Curso - StoredProcedures
        static void ConsultaViaProcedure()
        {
            using var db = new Curso.Data.ApplicationContext();

            var dep = "Departamento";

            var departamentos = 
            db.Departamentos
            //.FromSqlRaw("execute GetDepartamentos @p0", "Departamento")
            .FromSqlInterpolated($"execute GetDepartamentos {dep}")
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;  
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void CriarStoredProcedureDeConsulta()
        {
            var criarDepartamento = @"
            CREATE OR ALTER PROCEDURE GetDepartamentos
            @Descricao VARCHAR(50)
            AS
            BEGIN 
                SELECT * FROM Departamentos WHERE Descricao LIKE @Descricao + '%'
            END
            ";

            using var db = new Curso.Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(criarDepartamento);
        }

        static void InserirDadosViaProcedure()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.ExecuteSqlRaw("execute CriarDepartamento @p0, @p1", "Departamento via Proc", false);
        }

        static void CriarStoredProcedure()
        {
            var criarDepartamento = @"
            CREATE OR ALTER PROCEDURE CriarDepartamento
            @Descricao VARCHAR(50),
            @Ativo BIT
            AS
            BEGIN 
                INSERT INTO
                    Departamentos(Descricao, Ativo, Excluido)
                VALUES (@Descricao, @Ativo, 0)
            END
            ";

            using var db = new Curso.Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(criarDepartamento);
        }
        #endregion

        #region Segunda Parte do Curso - Consultas
        static void DivisaoDeConsulta()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
            .Include(p => p.Funcionarios)
            .Where(p => p.Id < 3)
            //.AsSplitQuery()
            .AsSingleQuery()
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;  
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.ForegroundColor = ConsoleColor.Green;  
                    Console.WriteLine($"\t Nome: {funcionario}");
                }
            }
        }

        static void EntendendoConsulta_1ToN_Nto1()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var funcionarios = db.Funcionarios
            .Include(p => p.Departamento)
            .ToList();

            foreach (var funcionario in funcionarios)
            {
                Console.ForegroundColor = ConsoleColor.Green;  
                Console.WriteLine($"\t Nome: {funcionario.Nome} / Departamento: {funcionario.Departamento.Descricao}");
            }
        }

        static void ConsultaCoTag()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
            .TagWith("Estou enviando um comentário para o servidor")
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;  
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void ConsultaInterpolada()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var id = 1;
            var departamentos = db.Departamentos
            .FromSqlInterpolated($"SELECT * FROM Departamentos WITH(NOLOCK) WHERE Id>{id}")
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;  
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void ConsultaParametrizada()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var id = 0;
            var departamentos = db.Departamentos
            .FromSqlRaw("SELECT * FROM Departamentos WITH(NOLOCK) WHERE Id > {0}", id)
            .Where(p => !p.Excluido)
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;  
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void ConsultaProjetada()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0)
            .Select(p => new { p.Descricao, Funcionarios = p.Funcionarios.Select(f => f.Nome) })
            .ToList();

            foreach (var departamento in departamentos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;  
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.ForegroundColor = ConsoleColor.Green;  
                    Console.WriteLine($"\t Nome: {funcionario}");
                }
            }
        }

        static void IgnoreFiltroGlobal()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;  
                //Console.BackgroundColor = ConsoleColor.DarkBlue;  
                Console.WriteLine($"Descrição: {departamento.Descricao} \t Excluido: {departamento.Excluido}");
            }
        }

        static void FiltroGlobal()
        {
            using var db = new Curso.Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;  
                //Console.BackgroundColor = ConsoleColor.DarkBlue;  
                Console.WriteLine($"Descrição: {departamento.Descricao} \t Excluido: {departamento.Excluido}");
            }
        }

        static void Setup(Curso.Data.ApplicationContext db)
        {
            if (db.Database.EnsureCreated())
            {
                db.Departamentos.AddRange(
                    new Curso.Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Icaro Henrique",
                                CPF = "99999999911",
                                RG= "2100062"
                            }
                        },
                        Excluido = true
                    },
                    new Curso.Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Patrizia Mastrodonato",
                                CPF = "88888888811",
                                RG= "3100062"
                            },
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Bruce Wayne",
                                CPF = "77777777711",
                                RG= "1100062"
                            }
                        }
                    });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        #endregion

        #region Primeira Parte do Curso - Funções DB
        static void CarregamentoLento()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;

            var departamentos = db
                .Departamentos
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("---------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void CarregamentoExplicito()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db.Departamentos.ToList();

            foreach (var departamento in departamentos)
            {
                if(departamento.Id == 2)
                {
                    //db.Entry(departamento).Collection("Funcionarios").Load();
                    //db.Entry(departamento).Collection(p=>p.Funcionarios).Load();
                    db.Entry(departamento).Collection(p=>p.Funcionarios).Query().Where(p=>p.Id > 2).ToList();
                }

                Console.WriteLine("---------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void CarregamentoAdiantado()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .Include(p => p.Funcionarios);

            foreach (var departamento in departamentos)
            {

                Console.WriteLine("---------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void SetupTiposCarregamentos(Curso.Data.ApplicationContext db)
        {
            if (!db.Departamentos.Any())
            {
                db.Departamentos.AddRange(
                    new Curso.Domain.Departamento
                    {
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Icaro Henrique",
                                CPF = "99999999911",
                                RG= "2100062"
                            }
                        }
                    },
                    new Curso.Domain.Departamento
                    {
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                        {
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Patrizia Mastrodonato",
                                CPF = "88888888811",
                                RG= "3100062"
                            },
                            new Curso.Domain.Funcionario
                            {
                                Nome = "Bruce Wayne",
                                CPF = "77777777711",
                                RG= "1100062"
                            }
                        }
                    });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        static void ScriptGeralDoBancoDeDados()
        {
            using var db = new Curso.Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        static void MigracoesJaAplicadas()
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoes = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void TodasMigracoes()
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoes = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void AplicarMigracaoEmTempodeExecucao()
        {
            using var db = new Curso.Data.ApplicationContext();

            db.Database.Migrate();
        }

        static void MigracoesPendentes()
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

         static void SqlInjection()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new Curso.Domain.Departamento
                {
                    Descricao = "Departamento 01"
                },
                new Curso.Domain.Departamento
                {
                    Descricao = "Departamento 02"
                });
            db.SaveChanges();

            //var descricao = "Teste ' or 1='1";
            //db.Database.ExecuteSqlRaw("update departamentos set descricao='AtaqueSqlInjection' where descricao={0}",descricao);
            //db.Database.ExecuteSqlRaw($"update departamentos set descricao='AtaqueSqlInjection' where descricao='{descricao}'");
            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descricao: {departamento.Descricao}");
            }
        }

        static void ExecuteSQL()
        {
            using var db = new Curso.Data.ApplicationContext();

            // Primeira Opcao
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            // Segunda Opcao
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("update departamentos set descricao={0} where id=1", descricao);

            //Terceira Opcao
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricao={descricao} where id=1");
        }

        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            using var db = new Curso.Data.ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_, __) => ++_count;

            if (gerenciarEstadoConexao)
            {
                conexao.Open();
            }

            for (var i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }

            time.Stop();
            var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEstadoConexao}, Contador: {_count}";

            Console.WriteLine(mensagem);
        }

        static void HealthCheckBancoDeDados()
        {
            using var db = new Curso.Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            if (canConnect)
            {

                Console.WriteLine("Posso me conectar");
            }
            else
            {
                Console.WriteLine("Não posso me conectar");
            }
        }

        static void EnsureCreatedAndDeleted()
        {
            using var db = new Curso.Data.ApplicationContext();
            //db.Database.EnsureCreated();
            db.Database.EnsureDeleted();
        }

        static void GapDoEnsureCreated()
        {
            using var db1 = new Curso.Data.ApplicationContext();
            //using var db2 = new Curso.Data.ApplicationContextCidade();

            db1.Database.EnsureCreated();
            //db2.Database.EnsureCreated();

            //var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            //databaseCreator.CreateTables();
        }
        #endregion
    }
}
