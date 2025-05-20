using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCampeonatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Campeonatos",
                columns: new[] { "Id", "Nome", "DataInic", "DataFim", "NumRodadas", "Tipo", "Ativo" },
                values: new object[]
                {
                    new Guid("df3f9bd6-41ae-4867-d0fb-08dd8c07d71c"),
                    "Campeonato Brasileiro 2025 - 1o Turno",
                    new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    new DateTime(2025, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    19,
                    1,
                    true
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Campeonatos",
                keyColumn: "Id",
                keyValue: new Guid("df3f9bd6-41ae-4867-d0fb-08dd8c07d71c"));
        }
    }
}
