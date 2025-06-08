using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoFKPalpiteJogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Palpites_Jogos_JogoId1",
                table: "Palpites");

            migrationBuilder.DropIndex(
                name: "IX_Palpites_JogoId1",
                table: "Palpites");

            migrationBuilder.DropColumn(
                name: "JogoId1",
                table: "Palpites");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JogoId1",
                table: "Palpites",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Palpites_JogoId1",
                table: "Palpites",
                column: "JogoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Palpites_Jogos_JogoId1",
                table: "Palpites",
                column: "JogoId1",
                principalTable: "Jogos",
                principalColumn: "Id");
        }
    }
}
