using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRodadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rodadas",
                columns: new[] { "Id", "CampeonatoId", "NumeroRodada", "DataInic", "DataFim", "NumJogos", "Status" },
                values: new object[,]
                {
                    { new Guid("84a52203-e860-475f-62c6-08dd8c90086d"), new Guid("df3f9bd6-41ae-4867-d0fb-08dd8c07d71c"), 1, new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 3 },
                    { new Guid("ad7e6396-5e5c-4e5a-7482-08dd923c346e"), new Guid("df3f9bd6-41ae-4867-d0fb-08dd8c07d71c"), 2, new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 2 },
                    { new Guid("8544682f-cfa0-4929-7483-08dd923c346e"), new Guid("df3f9bd6-41ae-4867-d0fb-08dd8c07d71c"), 3, new DateTime(2025, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rodadas",
                keyColumns: new[] { "Id" },
                keyValues: new object[]
                {
                    new Guid("84a52203-e860-475f-62c6-08dd8c90086d"),
                    new Guid("ad7e6396-5e5c-4e5a-7482-08dd923c346e"),
                    new Guid("8544682f-cfa0-4929-7483-08dd923c346e")
                });
        }
    }
}
