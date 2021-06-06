using Microsoft.EntityFrameworkCore.Migrations;

namespace DominandoEFCore.Migrations_BPK
{
    public partial class TestePropagacaoDados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Excluido",
                table: "Funcionarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Excluido",
                table: "Departamentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Estados",
                columns: new[] { "Id", "Nome" },
                values: new object[] { 1, "São Paulo" });

            migrationBuilder.InsertData(
                table: "Estados",
                columns: new[] { "Id", "Nome" },
                values: new object[] { 2, "Minas Gerais" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropColumn(
                name: "Excluido",
                table: "Funcionarios");

            migrationBuilder.DropColumn(
                name: "Excluido",
                table: "Departamentos");
        }
    }
}
