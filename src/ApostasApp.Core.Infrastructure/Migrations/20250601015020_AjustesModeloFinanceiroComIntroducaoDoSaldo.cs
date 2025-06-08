using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustesModeloFinanceiroComIntroducaoDoSaldo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransacoesFinanceiras_Apostadores_ApostadorId",
                table: "TransacoesFinanceiras");

            migrationBuilder.DropColumn(
                name: "Saldo",
                table: "Apostadores");

            migrationBuilder.RenameColumn(
                name: "ApostadorId",
                table: "TransacoesFinanceiras",
                newName: "SaldoId");

            migrationBuilder.RenameIndex(
                name: "IX_TransacoesFinanceiras_ApostadorId",
                table: "TransacoesFinanceiras",
                newName: "IX_TransacoesFinanceiras_SaldoId");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Apostadores",
                type: "varchar(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Apostadores",
                type: "varchar(250)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Saldos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataUltimaAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saldos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Saldos_Apostadores_ApostadorId",
                        column: x => x.ApostadorId,
                        principalTable: "Apostadores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Saldos_ApostadorId",
                table: "Saldos",
                column: "ApostadorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransacoesFinanceiras_Saldos_SaldoId",
                table: "TransacoesFinanceiras",
                column: "SaldoId",
                principalTable: "Saldos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransacoesFinanceiras_Saldos_SaldoId",
                table: "TransacoesFinanceiras");

            migrationBuilder.DropTable(
                name: "Saldos");

            migrationBuilder.RenameColumn(
                name: "SaldoId",
                table: "TransacoesFinanceiras",
                newName: "ApostadorId");

            migrationBuilder.RenameIndex(
                name: "IX_TransacoesFinanceiras_SaldoId",
                table: "TransacoesFinanceiras",
                newName: "IX_TransacoesFinanceiras_ApostadorId");

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "Apostadores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Apostadores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)");

            migrationBuilder.AddColumn<decimal>(
                name: "Saldo",
                table: "Apostadores",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_TransacoesFinanceiras_Apostadores_ApostadorId",
                table: "TransacoesFinanceiras",
                column: "ApostadorId",
                principalTable: "Apostadores",
                principalColumn: "Id");
        }
    }
}
