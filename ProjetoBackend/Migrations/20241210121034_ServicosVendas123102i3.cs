using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoBackend.Migrations
{
    /// <inheritdoc />
    public partial class ServicosVendas123102i3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "ServicosVendas",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "ServicosVendas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "ServicosVendas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "ServicosVendas");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "ServicosVendas");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "ServicosVendas");
        }
    }
}
