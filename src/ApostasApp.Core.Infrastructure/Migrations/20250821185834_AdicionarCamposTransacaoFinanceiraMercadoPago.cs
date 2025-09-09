using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposTransacaoFinanceiraMercadoPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalReference",
                table: "TransacoesFinanceiras",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TransacoesFinanceiras",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalReference",
                table: "TransacoesFinanceiras");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TransacoesFinanceiras");
        }
    }
}
