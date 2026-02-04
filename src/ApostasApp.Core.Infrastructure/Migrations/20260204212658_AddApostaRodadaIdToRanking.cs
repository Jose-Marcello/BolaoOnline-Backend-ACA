using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.Infrastructure.Migrations
{
  /// <inheritdoc />
  public partial class AddApostaRodadaIdToRanking : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_ApostasRodada_Apostadores_ApostadorId",
          table: "ApostasRodada");

      migrationBuilder.AddColumn<Guid>(
          name: "ApostaRodadaId",
          table: "RankingRodadas",
          type: "uuid",
          nullable: false,
          defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

      migrationBuilder.AddColumn<Guid>(
          name: "ApostadorId",
          table: "RankingRodadas",
          type: "uuid",
          nullable: false,
          defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

      migrationBuilder.AlterColumn<Guid>(
          name: "ApostadorCampeonatoId",
          table: "ApostasRodada",
          type: "uuid",
          nullable: true,
          oldClrType: typeof(Guid),
          oldType: "uuid");

      migrationBuilder.CreateIndex(
          name: "IX_RankingRodadas_ApostadorId",
          table: "RankingRodadas",
          column: "ApostadorId");

      migrationBuilder.CreateIndex(
          name: "IX_RankingRodadas_ApostaRodadaId",
          table: "RankingRodadas",
          column: "ApostaRodadaId",
          unique: true);

      migrationBuilder.AddForeignKey(
          name: "FK_ApostasRodada_Apostadores_ApostadorId",
          table: "ApostasRodada",
          column: "ApostadorId",
          principalTable: "Apostadores",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.AddForeignKey(
          name: "FK_RankingRodadas_Apostadores_ApostadorId",
          table: "RankingRodadas",
          column: "ApostadorId",
          principalTable: "Apostadores",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_RankingRodadas_ApostasRodada_ApostaRodadaId",
          table: "RankingRodadas",
          column: "ApostaRodadaId",
          principalTable: "ApostasRodada",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropForeignKey(
          name: "FK_ApostasRodada_Apostadores_ApostadorId",
          table: "ApostasRodada");

      migrationBuilder.DropForeignKey(
          name: "FK_RankingRodadas_Apostadores_ApostadorId",
          table: "RankingRodadas");

      migrationBuilder.DropForeignKey(
          name: "FK_RankingRodadas_ApostasRodada_ApostaRodadaId",
          table: "RankingRodadas");

      migrationBuilder.DropIndex(
          name: "IX_RankingRodadas_ApostadorId",
          table: "RankingRodadas");

      migrationBuilder.DropIndex(
          name: "IX_RankingRodadas_ApostaRodadaId",
          table: "RankingRodadas");

      migrationBuilder.DropColumn(
          name: "ApostaRodadaId",
          table: "RankingRodadas");

      migrationBuilder.DropColumn(
          name: "ApostadorId",
          table: "RankingRodadas");

      migrationBuilder.AlterColumn<Guid>(
          name: "ApostadorCampeonatoId",
          table: "ApostasRodada",
          type: "uuid",
          nullable: false,
          defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
          oldClrType: typeof(Guid),
          oldType: "uuid",
          oldNullable: true);

      migrationBuilder.AddForeignKey(
          name: "FK_ApostasRodada_Apostadores_ApostadorId",
          table: "ApostasRodada",
          column: "ApostadorId",
          principalTable: "Apostadores",
          principalColumn: "Id",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
