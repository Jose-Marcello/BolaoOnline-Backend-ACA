using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarApostadorRodadaEPalpitesIdentificadorApostaEmApostaRodada_e_AjustesFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apostas");

            migrationBuilder.AddColumn<decimal>(
                name: "CustoApostaRodada",
                table: "Rodadas",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustoAdesao",
                table: "Campeonatos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CustoAdesaoPago",
                table: "ApostadoresCampeonatos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInscricao",
                table: "ApostadoresCampeonatos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ApostasRodada",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorCampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RodadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentificadorAposta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataHoraSubmissao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EhApostaCampeonato = table.Column<bool>(type: "bit", nullable: false),
                    EhApostaIsolada = table.Column<bool>(type: "bit", nullable: false),
                    CustoPagoApostaRodada = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PontuacaoTotalRodada = table.Column<int>(type: "int", nullable: false),
                    Enviada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApostasRodada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApostasRodada_Apostadores_ApostadorCampeonatoId",
                        column: x => x.ApostadorCampeonatoId,
                        principalTable: "Apostadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApostasRodada_Rodadas_RodadaId",
                        column: x => x.RodadaId,
                        principalTable: "Rodadas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Palpites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostaRodadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlacarApostaCasa = table.Column<int>(type: "int", nullable: true),
                    PlacarApostaVisita = table.Column<int>(type: "int", nullable: true),
                    Pontos = table.Column<int>(type: "int", nullable: false),
                    JogoId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Palpites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Palpites_ApostasRodada_ApostaRodadaId",
                        column: x => x.ApostaRodadaId,
                        principalTable: "ApostasRodada",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Palpites_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Palpites_Jogos_JogoId1",
                        column: x => x.JogoId1,
                        principalTable: "Jogos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApostasRodada_ApostadorCampeonatoId",
                table: "ApostasRodada",
                column: "ApostadorCampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_ApostasRodada_RodadaId",
                table: "ApostasRodada",
                column: "RodadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Palpites_ApostaRodadaId",
                table: "Palpites",
                column: "ApostaRodadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Palpites_JogoId",
                table: "Palpites",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_Palpites_JogoId1",
                table: "Palpites",
                column: "JogoId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Palpites");

            migrationBuilder.DropTable(
                name: "ApostasRodada");

            migrationBuilder.DropColumn(
                name: "CustoApostaRodada",
                table: "Rodadas");

            migrationBuilder.DropColumn(
                name: "CustoAdesao",
                table: "Campeonatos");

            migrationBuilder.DropColumn(
                name: "CustoAdesaoPago",
                table: "ApostadoresCampeonatos");

            migrationBuilder.DropColumn(
                name: "DataInscricao",
                table: "ApostadoresCampeonatos");

            migrationBuilder.CreateTable(
                name: "Apostas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorCampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataHoraAposta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Enviada = table.Column<bool>(type: "bit", nullable: false),
                    PlacarApostaCasa = table.Column<int>(type: "int", nullable: false),
                    PlacarApostaVisita = table.Column<int>(type: "int", nullable: false),
                    Pontos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apostas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apostas_ApostadoresCampeonatos_ApostadorCampeonatoId",
                        column: x => x.ApostadorCampeonatoId,
                        principalTable: "ApostadoresCampeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apostas_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_ApostadorCampeonatoId",
                table: "Apostas",
                column: "ApostadorCampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_JogoId",
                table: "Apostas",
                column: "JogoId");
        }
    }
}
