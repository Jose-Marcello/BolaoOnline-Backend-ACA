using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarModuloFinanceireMapeamentoTransacaoCorrigido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Apostadores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeCompleto",
                table: "Apostadores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Saldo",
                table: "Apostadores",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "TransacoesFinanceiras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DataTransacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(250)", nullable: true),
                    CampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RodadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransacoesFinanceiras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransacoesFinanceiras_Apostadores_ApostadorId",
                        column: x => x.ApostadorId,
                        principalTable: "Apostadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransacoesFinanceiras_Campeonatos_CampeonatoId",
                        column: x => x.CampeonatoId,
                        principalTable: "Campeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransacoesFinanceiras_Rodadas_RodadaId",
                        column: x => x.RodadaId,
                        principalTable: "Rodadas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransacoesFinanceiras_ApostadorId",
                table: "TransacoesFinanceiras",
                column: "ApostadorId");

            migrationBuilder.CreateIndex(
                name: "IX_TransacoesFinanceiras_CampeonatoId",
                table: "TransacoesFinanceiras",
                column: "CampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransacoesFinanceiras_RodadaId",
                table: "TransacoesFinanceiras",
                column: "RodadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransacoesFinanceiras");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Apostadores");

            migrationBuilder.DropColumn(
                name: "NomeCompleto",
                table: "Apostadores");

            migrationBuilder.DropColumn(
                name: "Saldo",
                table: "Apostadores");
        }
    }
}
