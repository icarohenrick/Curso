using System;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=(localdb)\\mssqllocaldb; Initial Catalog=CursoDEVIO-02; Integrated Security=true; pooling=true; MultipleActiveResultSets=true;";

            optionsBuilder
                .UseSqlServer(strConnection)
                .EnableSensitiveDataLogging()
                //.UseLazyLoadingProxies()
                .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}