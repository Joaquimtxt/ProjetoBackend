using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoBackend.Migrations
{
    /// <inheritdoc />
    public partial class AgoraVai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensVenda_Produtos_ProdutoId",
                table: "ItensVenda");

            migrationBuilder.AlterColumn<int>(
                name: "Quantidade",
                table: "ItensVenda",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProdutoId",
                table: "ItensVenda",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ServicoId",
                table: "ItensVenda",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_ServicoId",
                table: "ItensVenda",
                column: "ServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensVenda_Produtos_ProdutoId",
                table: "ItensVenda",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensVenda_Servicos_ServicoId",
                table: "ItensVenda",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "ServicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensVenda_Produtos_ProdutoId",
                table: "ItensVenda");

            migrationBuilder.DropForeignKey(
                name: "FK_ItensVenda_Servicos_ServicoId",
                table: "ItensVenda");

            migrationBuilder.DropIndex(
                name: "IX_ItensVenda_ServicoId",
                table: "ItensVenda");

            migrationBuilder.DropColumn(
                name: "ServicoId",
                table: "ItensVenda");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantidade",
                table: "ItensVenda",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProdutoId",
                table: "ItensVenda",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensVenda_Produtos_ProdutoId",
                table: "ItensVenda",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "ProdutoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
