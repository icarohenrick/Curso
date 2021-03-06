using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Curso.Interceptadores
{
    public class InterceptadoresDeConexao : DbConnectionInterceptor
    {
        public override InterceptionResult ConnectionOpening(
            DbConnection connection, 
            ConnectionEventData eventData, 
            InterceptionResult result)
        {
            System.Console.WriteLine("Entrei no método ConnectionOpening");

            var connectionString = ((SqlConnection)connection).ConnectionString;

            System.Console.WriteLine(connectionString);

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                //DataSource = "IP Segundo Servidor",
                ApplicationName = "CursoEFCore"
            };

            connection.ConnectionString = connectionStringBuilder.ToString();

            System.Console.WriteLine(connectionStringBuilder.ToString());

            return result;
        }
    }
}