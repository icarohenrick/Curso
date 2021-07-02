using System;
using System.Linq;
using System.Transactions;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConsultaProjetada();
        }
        #region Migrações

        #endregion

        #region Performance
        /*        
        static void ConsultaRastreada()
        {
            using var db = new Curso.Data.ApplicationContext();
            var funcionarios = db.Funcionarios.Include(p => p.Departamento).ToList();
        }
        static void ConsultaNaoRastreada()
        {
            using var db = new Curso.Data.ApplicationContext();
            var funcionarios = db.Funcionarios.AsNoTracking().Include(p => p.Departamento).ToList();
        }
        
        static void ConsultaComResolucaoDeIdentidade()
        {
            using var db = new Curso.Data.ApplicationContext();

            var funcionarios = db.Funcionarios.AsNoTrackingWithIdentityResolution()
                .Include(p => p.Departamento).ToList();
        }

        static void ConsultaCustomizada()
        {
            using var db = new Curso.Data.ApplicationContext();

            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

            var funcionarios = db.Funcionarios.Include(p => p.Departamento).ToList();
        }

        static void ConsultaProjetadaERastreada()
        {
            using var db = new Curso.Data.ApplicationContext();

            var departamento = db.Departamentos
                .Include(p => p.Funcionarios)
                .Select(p => new {
                    Departamento = p,
                    TotalFuncionarios = p.Funcionarios.Count()
                }).ToList();

                departamento[0].Departamento.Descricao = "Departamento Teste Atualizado";

                db.SaveChanges();
        }

        static void ConsultaProjetada()
        {
            using var db = new Curso.Data.ApplicationContext();

            //var departamentos = db.Departamentos.ToArray(); //360 MB

            var departamentos = db.Departamentos.Select(x => x.Descricao).ToArray(); // 55 MB
            
            var memoria = (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024 + " MB");

            Console.WriteLine(memoria);
        }

        static void Inserir_200_Departamentos_Com_1MB()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var random = new Random();

            db.Departamentos.AddRange(Enumerable.Range(1,200).Select(p => 
                new Departamento
                {
                    Descricao = "Departamento Teste",
                    Image = getBytes()
                }));
            
             db.SaveChanges();
            
            byte[] getBytes()
            {
                var buffer = new byte[1024 * 1024];
                random.NextBytes(buffer);

                return buffer;
            }
        }

        static void Setup()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            _ = db.Departamentos.Add(new Departamento
            {
                Descricao = "Departamento Teste",
                Ativo = true,
                Funcionarios = Enumerable.Range(1, 100).Select(p => new Funcionario
                {
                    CPF = p.ToString().PadLeft(11, '0'),
                    Nome = $"Funcionario {p}",
                    RG = p.ToString()
                }).ToList()
            });

            db.SaveChanges();
        }
        */
        #endregion

        #region UDF
        /*static void DateDIFF()
        {
            CadastrarLivro();

            using var db = new Curso.Data.ApplicationContext();

            //var resultado = db.Livros.Select(p => EF.Functions.DateDiffDay(p.CadastradoEm, DateTime.Now));

            var resultado = db.Livros.Select(p => Curso.Funcoes.MinhasFuncoes.DateDiff("DAY", p.CadastradoEm, DateTime.Now));

            foreach (var diff in resultado)
            {
                Console.WriteLine(diff);
            }
        }

        static void FuncaoDefinidaPeloUsuario()
        {
            CadastrarLivro();

            using var db = new Curso.Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(@"
                CREATE FUNCTION ConverterParaLetrasMaiusculas(@dados VARCHAR(100))
                RETURNS VARCHAR(100)
                BEGIN
                    RETURN UPPER(@dados)
                END
            ");

            var resultado = db.Livros.Select(p => Curso.Funcoes.MinhasFuncoes.LetrasMaiusculas(p.Titulo));

            foreach (var parteTitulo in resultado)
            {
                Console.WriteLine(parteTitulo);
            }
        }

        static void FuncaoLEFT()
        {
            CadastrarLivro();
            using var db = new Curso.Data.ApplicationContext();

            //var resultado = db.Livros.Select(p => Curso.Data.ApplicationContext.Left(p.Titulo, 10));
            var resultado = db.Livros.Select(p => Curso.Funcoes.MinhasFuncoes.Left(p.Titulo, 10));

            foreach (var parteTitulo in resultado)
            {
                Console.WriteLine(parteTitulo);
            }
        }*/
        #endregion

        #region transações
        /*static void TransactionScope()
        {
            CadastrarLivro();

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                ConsultarAtualizar();
                CadastrarLivroEnterprice();
                CadastrarLivroDominandoEFCore();

                scope.Complete();
            }
        }

        static void ConsultarAtualizar()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                var livro = db.Livros.FirstOrDefault(p => p.Id ==1);
                    livro.Autor = "Icaro Santos";
                    db.SaveChanges();
            }
        }

        static void CadastrarLivroEnterprice()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "ASP.NET CORE Enterprise Applications",
                        Autor = "Patrizia"
                    }
                );
                db.SaveChanges();
            }
        }

        static void CadastrarLivroDominandoEFCore()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Icaro Santos"
                    }
                );
                db.SaveChanges();
            }
        }

        static void SalvarPontoTransacao()
        {
            CadastrarLivro();

            using (var db = new Curso.Data.ApplicationContext())
            {
                using var transacao = db.Database.BeginTransaction();

                try
                {
                    var livro = db.Livros.FirstOrDefault(p => p.Id ==1);
                    livro.Autor = "Icaro Santos";
                    db.SaveChanges();

                    transacao.CreateSavepoint("desfazer_apenas_insercao");

                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "ASP.NET CORE Enterprise Applications",
                            Autor = "Icaro Henrique"
                        }
                    );
                    db.SaveChanges();

                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "Dominando o Entity Framework Core",
                            Autor = "Icaro Henrique".PadLeft(16, '*')
                        }
                    );
                    db.SaveChanges();

                    transacao.Commit();
                }
                catch(DbUpdateException e)
                {
                    transacao.RollbackToSavepoint("desfazer_apenas_insercao");

                    if(e.Entries.Count(p => p.State == EntityState.Added) == e.Entries.Count)
                    {
                        transacao.Commit();
                    }
                }
            }
        }

        static void ReverterTransacao()
        {
            CadastrarLivro();

            using(var db = new Curso.Data.ApplicationContext())
            {
                var transacao = db.Database.BeginTransaction();

                try
                {
                    var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                    livro.Autor = "Icaro Santos";

                    db.SaveChanges();

                    Console.ReadKey();

                    db.Livros.Add(
                        new Livro 
                        {
                            Titulo = "Dominando o Entity Framework Core",
                            Autor = "Icaro Santos".PadLeft(16, '*')
                        }
                    );

                    db.SaveChanges();
                }
                catch
                {
                    transacao.Rollback();
                }

                transacao.Commit();
            }
        }

        static void GerenciandoTransacaoManualmente()
        {
            CadastrarLivro();

            using(var db = new Curso.Data.ApplicationContext())
            {
                var transacao = db.Database.BeginTransaction();

                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                livro.Autor = "Icaro Santos";

                db.SaveChanges();

                Console.ReadKey();

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Icaro Santos"
                    }
                );

                db.SaveChanges();

                transacao.Commit();
            }
        }

        static void ComportamentoPadrao()
        {
            CadastrarLivro();

            using(var db = new Curso.Data.ApplicationContext())
            {
                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                livro.Autor = "Icaro Henrique";

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Patrizia Mastrodonato"
                    }
                );

                db.SaveChanges();
            }
        }

        static void CadastrarLivro()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Introdução ao Entity Framework Core",
                        Autor = "Icaro",
                        CadastradoEm = DateTime.Now.AddDays(-1)
                    }
                );

                db.SaveChanges();
            }
        }*/
        #endregion
        
        #region Interceptação
        // static void TesteInterceptacaoSaveChanges()
        // {
        //     using(var db = new Curso.Data.ApplicationContext())
        //     {
        //         db.Database.EnsureDeleted();
        //         db.Database.EnsureCreated();

        //         db.Funcoes.Add(new Funcao
        //         {
        //             Descricao1 = "Teste"
        //         });

        //         db.SaveChanges();
        //     }
        // }

        // static void TesteInterceptacao()
        // {
        //     using(var db = new Curso.Data.ApplicationContext())
        //     {
        //         var consulta = db.Funcoes
        //             .TagWith("Use NOLOCK")
        //             .FirstOrDefault();

        //         Console.WriteLine($"Consulta: {consulta?.Descricao1}");
        //     }
        // }
        #endregion

        #region EF Funcions
        // static void FuncaoCollate()
        // {
        //     using (var db = new Curso.Data.ApplicationContext())
        //     {
        //         var consulta1 = db.Funcoes
        //             .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CS_AS") == "tela");

        //         var consulta2 = db.Funcoes
        //             .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CI_AS") == "tela");

        //         Console.WriteLine($"Consulta1: {consulta1?.Descricao1}");
        //         Console.WriteLine($"Consulta2: {consulta2?.Descricao1}");
        //     }
        // }

        // static void FuncaoProperty()
        // {
        //     ApagarCriarBancoDeDados();

        //     using (var db = new Curso.Data.ApplicationContext())
        //     {
        //          var resultado = db.Funcoes
        //             //.AsNoTracking() //O Resultado não aparece se for descomentado devido a perda de referencia após a consulta
        //             .FirstOrDefault(p => EF.Property<string>(p, "PropriedadeDeSombra") == "Teste");

        //          var propriedadeDeSombra = db.Entry(resultado)
        //             .Property<string>("PropriedadeDeSombra")
        //             .CurrentValue;

        //         Console.WriteLine("Resultdo:");
        //         Console.WriteLine(propriedadeDeSombra);
        //     }
        // }

        // static void FuncoesDataLength()
        // {
        //     using (var db = new Curso.Data.ApplicationContext())
        //     {
        //         var resultado = db.Funcoes.AsNoTracking()
        //             .Select(p => new 
        //             {
        //                 TotalBytesCampoData = EF.Functions.DataLength(p.Data1),
        //                 TotalBytes1 = EF.Functions.DataLength(p.Descricao1),
        //                 TotalBytes2 = EF.Functions.DataLength(p.Descricao2),
        //                 Total1 = p.Descricao1.Length,
        //                 Total2 = p.Descricao2.Length

        //             }).FirstOrDefault();

        //         Console.WriteLine("Resultado:");
        //         Console.WriteLine(resultado);
        //     }
        // }

        // static void FuncoesLike()
        // {
        //     using (var db = new Curso.Data.ApplicationContext())
        //     {
        //         var script = db.Database.GenerateCreateScript();

        //         Console.WriteLine(script);

        //         var dados = db.Funcoes.AsNoTracking()
        //             //.Where(p => EF.Functions.Like(p.Descricao1, "Bo%"))
        //             .Where(p => EF.Functions.Like(p.Descricao1, "B[ao]%"))
        //             .Select(p => p.Descricao1).ToArray();

        //         foreach (var descricao in dados)
        //         {
        //             Console.WriteLine(descricao);
        //         }
        //     }
        // }
        
        // static void FuncoesDeDatas()
        // {
        //     ApagarCriarBancoDeDados();

        //     using (var db = new Curso.Data.ApplicationContext())
        //     {
        //         var script = db.Database.GenerateCreateScript();

        //         Console.WriteLine(script);

        //         var dados = db.Funcoes.AsNoTracking().Select(p => 
        //         new 
        //         {
        //             Dias = EF.Functions.DateDiffDay(DateTime.Now, p.Data1),
        //             Data = EF.Functions.DateFromParts(2021, 1, 2),
        //             DataValida = EF.Functions.IsDate(p.Data2)
        //         });

        //         foreach (var f in dados)
        //         {
        //             Console.WriteLine(f);
        //         }
        //     }
        // }

        // static void ApagarCriarBancoDeDados()
        // {
        //     using var db = new Curso.Data.ApplicationContext();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();

        //     db.Funcoes.AddRange(
        //         new Funcao
        //         {
        //             Data1 = DateTime.Now.AddDays(2),
        //             Data2 = "2021-01-01",
        //             Descricao1 = "Bala 1 ",
        //             Descricao2 = "Bala 1 "
        //         },
        //         new Funcao
        //         {
        //             Data1 = DateTime.Now.AddDays(1),
        //             Data2 = "XX21-01-01",
        //             Descricao1 = "Bola 2",
        //             Descricao2 = "Bola 2"
        //         },
        //         new Funcao
        //         {
        //             Data1 = DateTime.Now.AddDays(1),
        //             Data2 = "XX21-01-01",
        //             Descricao1 = "Tela",
        //             Descricao2 = "Tela"
        //         }
        //     );

        //     db.SaveChanges();
        // }
        #endregion

        #region Materias Anteriores
        /*
        #region Sexta Parte do Curso - Data Annotations 
        static void Atributos()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var script = db.Database.GenerateCreateScript();
                Console.WriteLine(script);

                db.Attributos.Add(new Attributo
                {
                    Descricao = "Exemplo",
                    Observacao = "Observação"
                });

                db.SaveChanges();
            }
        }
        #endregion
        
        #region Quinta Parte do Curso - Modelo de Dados
        static void PacotesDePropriedades()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var configuration = new Dictionary<string, object>
                {
                    ["Chave"] = "SenhaBancoDeDados",
                    ["Valor"] = Guid.NewGuid().ToString()
                };

                db.Configuracoes.Add(configuration);
                db.SaveChanges();

                var configuracoes = db.Configuracoes.AsNoTracking().Where(p => p["Chave"].ToString() == "SenhaBancoDeDados").ToArray();

                foreach (var dic in configuracoes)
                {
                    Console.WriteLine($"Chave {dic["Chave"]} - Valor: {dic["Valor"]}");
                }
            }
        }
        static void ExemploTPH()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var pessoa = new Pessoa { Nome = "Patrizia" };
                var instrutor = new Instrutor { Nome = "Icaro Henrique", Tecnologia=".NET", Desde=DateTime.Now };
                var aluno = new Aluno { Nome = "Bruce Dog", Idade=7, DataContrato = DateTime.Now.AddDays(-1) };

                db.AddRange(pessoa, instrutor, aluno);
                db.SaveChanges();

                var pessoas = db.Pessoas.AsNoTracking().ToArray();
                var instrutores = db.Instrutores.AsNoTracking().ToArray();
                //var alunos = db.Alunos.AsNoTracking().ToArray();
                var alunos = db.Pessoas.OfType<Aluno>().AsNoTracking().ToArray();

                Console.WriteLine($"Pessoas *********");
                foreach (var p in pessoas)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}");
                }
                Console.WriteLine($"Instrutores *********");
                foreach (var p in instrutores)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Tecnologia: {p.Tecnologia}, Desde: {p.Desde}");
                }
                Console.WriteLine($"Alunos *********");
                foreach (var p in alunos)
                {
                    Console.WriteLine($"Id: {p.Id} -> {p.Nome}, Idade: {p.Idade}, Data do Contrato: {p.DataContrato}");
                }
            }
        }

        static void CampoDeApoio()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var documento = new Documento();
                documento.SetCPF("432.761.988-11");

                db.Documentos.Add(documento);
                db.SaveChanges();

                foreach (var doc in db.Documentos.AsNoTracking())
                {
                    //Console.WriteLine($"CPF -> {doc.CPF}");
                    Console.WriteLine($"CPF -> {doc.GetCPF()}");
                }
            }
        }
        static void RelacionamentoMuitosParaMuitos()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                
                var ator1 = new Ator { Nome = "Icaro" };
                var ator2 = new Ator { Nome = "Patrizia" };
                var ator3 = new Ator { Nome = "Bruce" };

                var filme1 = new Filme { Descricao = "Piratas do Caribe" };
                var filme2 = new Filme { Descricao = "Batman - O Retorno" };
                var filme3 = new Filme { Descricao = "Legalmente Loira" };

                ator1.Filmes.Add(filme1);
                ator1.Filmes.Add(filme2);
                ator2.Filmes.Add(filme1);

                filme3.Atores.Add(ator1);
                filme3.Atores.Add(ator2);
                filme3.Atores.Add(ator3);

                db.AddRange(ator1, ator2, filme3);

                db.SaveChanges();

                foreach (var ator in db.Atores.Include(p => p.Filmes))
                {
                    Console.WriteLine($"Ator: {ator.Nome}");

                    foreach (var filme in ator.Filmes)
                    {
                        Console.WriteLine($"\tFilme: {filme.Descricao}");
                    }
                }
            }
        }
        static void Relacionamento1ParaMuitos()
        {
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                
                var estado = new Estado
                {
                    Nome = "São Paulo",
                    Governador = new Governador { Nome = "João Doria" }
                };

                estado.Cidades.Add(new Cidade { Nome = "Barueri"});

                db.Estados.Add(estado);

                db.SaveChanges();
            }

            using (var db = new Curso.Data.ApplicationContext())
            {
                var estados = db.Estados.ToList();

                estados[0].Cidades.Add(new Cidade { Nome = "Osasco"} );

                db.SaveChanges();

                foreach (var est in db.Estados.Include(p => p.Cidades).AsNoTracking())
                {
                    Console.WriteLine($"Estado: {est.Nome}, Governador: {est.Governador.Nome}");

                    foreach (var cidade in est.Cidades)
                    {
                        Console.WriteLine($"\t Cidade: {cidade.Nome}");
                    }
                }
            }
        }
        static void Relacionamento1Para1()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
            var estado = new Estado
            {
                Nome = "São Paulo",
                Governador = new Governador { Nome = "João Doria" }
            };

            db.Estados.Add(estado);

            db.SaveChanges();

            var estados = db.Estados.AsNoTracking().ToList();

            estados.ForEach(est => 
            {
                Console.WriteLine($"Estado: {est.Nome}, Governador: {est.Governador.Nome}");
            });
        }
        static void TiposDePropriedades()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
            var cliente = new Cliente
            {
                Nome = "Bruce Wayne",
                Telefone = "(11) 99271-2159",
                Endereco = new Endereco { Bairro = "Centro", Cidade = "Gotham City" }
            };

            db.Clientes.Add(cliente);

            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList();

            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };

            clientes.ForEach(cli => 
            {
                var json = System.Text.Json.JsonSerializer.Serialize(cli, options);

                Console.WriteLine(json);
            });
        }

        static void TrabalhandoPropriedadesDeSombra()
        {
            using var db = new Curso.Data.ApplicationContext();
            /*db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
            var departamento = new Departamento
            {
                Descricao = "Departamento Propriedade de Sombra"
            };

            db.Departamentos.Add(departamento);

            db.Entry(departamento).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;

            db.SaveChanges();

            var departamentos = db.Departamentos.Where(p => EF.Property<DateTime>(p, "UltimaAtualizacao") < DateTime.Now).ToArray();
        }

        static void PropriedadesDeSombra()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        static void ConversorCustomizado()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Conversores.Add(
                new Conversor
                {
                    Status = Status.Devolvido,
                }
            );

            db.SaveChanges();

            var conversorEmAnalise = db.Conversores.AsNoTracking().FirstOrDefault(p => p.Status == Status.Analise);
            var conversorDevolvido = db.Conversores.AsNoTracking().FirstOrDefault(p => p.Status == Status.Devolvido);
        }

        static void ConversorDeValor() => Esquema();

        static void Esquema()
        {
            using var db = new Curso.Data.ApplicationContext();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }
        static void PropagarDados()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }
        static void Collations()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
        #endregion

        #region Quarta Parte do Curso - Infraestrutura
        static void ExecutarEstrategiaResiliencia()
        {
            using var db = new Curso.Data.ApplicationContext();

            var strategy = db.Database.CreateExecutionStrategy();

            strategy.Execute(() => {
                using var transaction = db.Database.BeginTransaction();

                db.Departamentos.Add(new Departamento { Descricao = "Transacao" });
                db.SaveChanges();

                transaction.Commit();
            });
        }
        static void TempoComandoGeral()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.SetCommandTimeout(10);
            db.Database.ExecuteSqlRaw("waitfor delay '00:00:07'; SELECT 1");
        }
        static void HabilitandoBatchSize()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            for (int i = 0; i < 50; i++)
            {
                db.Departamentos.Add(new Departamento
                {
                    Descricao = $"Departamento {i}" 
                });
            }
            
            db.SaveChanges();
        }

        static void DadosSensiveis()
        {
            using var db = new Curso.Data.ApplicationContext();

            var descricao = "Departamento";
            var departamentos = db.Departamentos.Where(p => p.Descricao == descricao).ToArray();
        }

        static void ConsultarDepartamentos()
        {
            using var db = new Curso.Data.ApplicationContext();

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToArray();
        }
        #endregion

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
    */
    #endregion
    }
}
