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
      // 1. Removemos a tentativa de criar o ApostadorId, já que o erro confirmou que ele existe
      // migrationBuilder.AddColumn<Guid>(name: "ApostadorId", ...); <--- REMOVIDO

      // 2. Adicionamos apenas a ApostaRodadaId (permitindo nulo para não quebrar o banco)
      migrationBuilder.AddColumn<Guid>(
          name: "ApostaRodadaId",
          table: "RankingRodadas",
          type: "uuid",
          nullable: true);

      // 3. Ajustamos a ApostadorCampeonatoId para opcional (essencial para Avulsas)
      migrationBuilder.AlterColumn<Guid>(
          name: "ApostadorCampeonatoId",
          table: "ApostasRodada",
          type: "uuid",
          nullable: true,
          oldClrType: typeof(Guid),
          oldType: "uuid");

      // 4. Criamos o índice para a nova coluna (sem Unique por enquanto)
      migrationBuilder.CreateIndex(
          name: "IX_RankingRodadas_ApostaRodadaId",
          table: "RankingRodadas",
          column: "ApostaRodadaId",
          unique: false);

      // 5. Criamos a Foreign Key para a ApostaRodada
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
