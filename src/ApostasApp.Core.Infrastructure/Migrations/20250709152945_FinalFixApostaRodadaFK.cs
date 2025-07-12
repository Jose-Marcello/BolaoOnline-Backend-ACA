using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalFixApostaRodadaFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApostasRodada_Apostadores_ApostadorCampeonatoId",
                table: "ApostasRodada");

            migrationBuilder.AddForeignKey(
                name: "FK_ApostasRodada_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostasRodada",
                column: "ApostadorCampeonatoId",
                principalTable: "ApostadoresCampeonatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApostasRodada_ApostadoresCampeonatos_ApostadorCampeonatoId",
                table: "ApostasRodada");

            migrationBuilder.AddForeignKey(
                name: "FK_ApostasRodada_Apostadores_ApostadorCampeonatoId",
                table: "ApostasRodada",
                column: "ApostadorCampeonatoId",
                principalTable: "Apostadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
