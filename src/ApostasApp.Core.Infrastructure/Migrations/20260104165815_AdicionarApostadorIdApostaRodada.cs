using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarApostadorIdApostaRodada : Migration
    {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      // 1. Cria a coluna primeiro permitindo NULO (evita o erro de FK)
      migrationBuilder.AddColumn<Guid>(
          name: "ApostadorId",
          table: "ApostasRodada",
          type: "uuid",
          nullable: true); // <-- Começa como true

      // 2. Executa o SQL para preencher os dados usando a ponte do campeonato
      migrationBuilder.Sql(@"
        UPDATE ""ApostasRodada"" ar
        SET ""ApostadorId"" = ac.""ApostadorId""
        FROM ""ApostadoresCampeonatos"" ac
        WHERE ar.""ApostadorCampeonatoId"" = ac.""Id"";
    ");

      // 3. Agora que está tudo preenchido, torna a coluna OBRIGATÓRIA e cria a FK
      migrationBuilder.AlterColumn<Guid>(
          name: "ApostadorId",
          table: "ApostasRodada",
          type: "uuid",
          nullable: false); // <-- Agora vira false

      migrationBuilder.AddForeignKey(
          name: "FK_ApostasRodada_Apostadores_ApostadorId",
          table: "ApostasRodada",
          column: "ApostadorId",
          principalTable: "Apostadores",
          principalColumn: "Id",
          onDelete: ReferentialAction.Restrict);

      migrationBuilder.CreateIndex(
          name: "IX_ApostasRodada_ApostadorId",
          table: "ApostasRodada",
          column: "ApostadorId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApostasRodada_Apostadores_ApostadorId",
                table: "ApostasRodada");

            migrationBuilder.DropIndex(
                name: "IX_ApostasRodada_ApostadorId",
                table: "ApostasRodada");

            migrationBuilder.DropColumn(
                name: "ApostadorId",
                table: "ApostasRodada");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataTransacao",
                table: "TransacoesFinanceiras",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataUltimaAtualizacao",
                table: "Saldos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInic",
                table: "Rodadas",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFim",
                table: "Rodadas",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataAtualizacao",
                table: "RankingRodadas",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataJogo",
                table: "Jogos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInic",
                table: "Campeonatos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFim",
                table: "Campeonatos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDate",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHoraSubmissao",
                table: "ApostasRodada",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataCriacao",
                table: "ApostasRodada",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInscricao",
                table: "ApostadoresCampeonatos",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
