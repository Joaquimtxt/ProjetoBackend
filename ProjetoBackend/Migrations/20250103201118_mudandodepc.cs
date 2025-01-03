using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoBackend.Migrations
{
    /// <inheritdoc />
    public partial class mudandodepc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensCompra_Produtos_ProdutoId",
                table: "ItensCompra");

            migrationBuilder.AlterColumn<int>(
                name: "Quantidade",
                table: "ItensCompra",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProdutoId",
                table: "ItensCompra",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "ItensCompra",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorUnitario",
                table: "ItensCompra",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "ValorTotal",
                table: "Compras",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaFiscal",
                table: "Compras",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensCompra_Produtos_ProdutoId",
                table: "ItensCompra",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensCompra_Produtos_ProdutoId",
                table: "ItensCompra");

            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "ItensCompra");

            migrationBuilder.DropColumn(
                name: "ValorUnitario",
                table: "ItensCompra");

            migrationBuilder.DropColumn(
                name: "NotaFiscal",
                table: "Compras");

            migrationBuilder.AlterColumn<double>(
                name: "Quantidade",
                table: "ItensCompra",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProdutoId",
                table: "ItensCompra",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ValorTotal",
                table: "Compras",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensCompra_Produtos_ProdutoId",
                table: "ItensCompra",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "ProdutoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
