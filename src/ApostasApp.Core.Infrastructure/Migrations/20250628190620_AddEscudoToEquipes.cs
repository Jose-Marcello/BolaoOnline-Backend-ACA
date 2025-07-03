using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEscudoToEquipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Estadios_EstadioId",
                table: "Jogos");

            migrationBuilder.AlterColumn<string>(
                name: "Sigla",
                table: "Equipes",
                type: "varchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Equipes",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)");

            migrationBuilder.AddColumn<string>(
                name: "Escudo",
                table: "Equipes",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Estadios_EstadioId",
                table: "Jogos",
                column: "EstadioId",
                principalTable: "Estadios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Estadios_EstadioId",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "Escudo",
                table: "Equipes");

            migrationBuilder.AlterColumn<string>(
                name: "Sigla",
                table: "Equipes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Equipes",
                type: "varchar(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Estadios_EstadioId",
                table: "Jogos",
                column: "EstadioId",
                principalTable: "Estadios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
