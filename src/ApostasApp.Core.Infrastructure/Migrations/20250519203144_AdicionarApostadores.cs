using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarApostadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Apostadores",
                columns: new[] { "Id", "Status", "UsuarioId" },
                values: new object[,]
                {
                    { new Guid("EF74875F-9FB8-4920-AF00-369C4F3F7FED"), 1, "155af3ee-c56b-495e-b5a1-f34e5d8e380f" },
                    { new Guid("581A35E3-66A4-41E5-99FF-691D91DFD3A5"), 1, "725831c6-db84-463c-bc2e-ec2a86f69f4b" },
                    { new Guid("2FFF3E52-D7E6-4040-BAA4-8B22F33F09EE"), 1, "846a9285-bf07-455c-9d1a-fe4ea4a826fb" },
                    { new Guid("F895771E-8B21-4BAB-B49D-A017483ACFCA"), 1, "07d59dc8-f02c-458e-be85-90c442f6c945" },
                    { new Guid("D6CD921C-C824-4327-8885-A28ACAEF082D"), 1, "641d31f8-5b3c-40ec-a968-eb13c154d336" },
                    { new Guid("34DFAE09-DC0D-4A95-8A5C-B1FD018B6FF1"), 1, "ce028b4a-156c-4714-ab13-11e0303d1c73" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Apostadores",
                keyColumns: new[] { "Id" },
                keyValues: new object[]
                {
                    new Guid("EF74875F-9FB8-4920-AF00-369C4F3F7FED"),
                    new Guid("581A35E3-66A4-41E5-99FF-691D91DFD3A5"),
                    new Guid("2FFF3E52-D7E6-4040-BAA4-8B22F33F09EE"),
                    new Guid("F895771E-8B21-4BAB-B49D-A017483ACFCA"),
                    new Guid("D6CD921C-C824-4327-8885-A28ACAEF082D"),
                    new Guid("34DFAE09-DC0D-4A95-8A5C-B1FD018B6FF1")
                });
        }
    }
}