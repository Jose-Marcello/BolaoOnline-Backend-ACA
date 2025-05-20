using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRankingRodada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RankingRodadas",
                columns: new[] { "Id", "RodadaId", "ApostadorCampeonatoId", "Pontuacao", "Posicao", "DataAtualizacao" },
                values: new object[,]
                {
                    { new Guid("6ED8FD7F-A64F-4CE1-8ADA-1A62281024C2"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new Guid("0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6"), 0, 0, new DateTime(2025, 5, 15, 16, 38, 13, 981, DateTimeKind.Unspecified).AddTicks(1710) },
                    { new Guid("7CBAAA82-0461-4569-930F-2DBC0E344DA3"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new Guid("70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F"), 27, 4, new DateTime(2025, 5, 15, 16, 41, 39, 906, DateTimeKind.Unspecified).AddTicks(6659) },
                    { new Guid("C0CA4FA9-6362-414F-8AC2-6052CE752289"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new Guid("FF670299-D4A2-439B-984D-E71366E1946B"), 0, 0, new DateTime(2025, 5, 15, 16, 38, 13, 982, DateTimeKind.Unspecified).AddTicks(150) },
                    { new Guid("2253941A-469C-4080-AD1B-8C7267658E05"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new Guid("92743816-E057-449C-8D62-F973F584918A"), 26, 5, new DateTime(2025, 5, 15, 16, 41, 40, 48, DateTimeKind.Unspecified).AddTicks(9538) },
                    { new Guid("59809DCB-4B19-4125-9BB2-B576E0161873"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new Guid("70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F"), 0, 0, new DateTime(2025, 5, 15, 16, 38, 13, 981, DateTimeKind.Unspecified).AddTicks(9157) },
                    { new Guid("F633B2E6-D3E5-47C2-AC07-B801BAD64C45"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new Guid("6B62054D-0849-4337-AF9D-005FEAE6C7EE"), 36, 2, new DateTime(2025, 5, 15, 16, 41, 39, 847, DateTimeKind.Unspecified).AddTicks(1894) },
                    { new Guid("CB1FB98C-677C-4863-ADCC-B80D22379DE1"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new Guid("0E684317-1FDE-473E-AED5-6783CA527B30"), 0, 0, new DateTime(2025, 5, 15, 16, 38, 13, 981, DateTimeKind.Unspecified).AddTicks(8047) },
                    { new Guid("44298748-AEAC-4B18-8F9E-CE1CD0DD7ADE"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new Guid("92743816-E057-449C-8D62-F973F584918A"), 0, 0, new DateTime(2025, 5, 15, 16, 38, 13, 982, DateTimeKind.Unspecified).AddTicks(1219) },
                    { new Guid("21747805-A40C-4D96-9008-DBDE35BB17A7"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new Guid("6B62054D-0849-4337-AF9D-005FEAE6C7EE"), 0, 0, new DateTime(2025, 5, 15, 16, 38, 13, 707, DateTimeKind.Unspecified).AddTicks(9731) },
                    { new Guid("DACB1BA0-6421-4510-B372-DF31DD241768"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new Guid("0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6"), 39, 1, new DateTime(2025, 5, 15, 16, 41, 39, 924, DateTimeKind.Unspecified).AddTicks(4611) },
                    { new Guid("A9C29302-ACFF-4962-9FC4-F69FC10C3DCD"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new Guid("0E684317-1FDE-473E-AED5-6783CA527B30"), 0, 6, new DateTime(2025, 5, 15, 16, 41, 39, 879, DateTimeKind.Unspecified).AddTicks(1588) },
                    { new Guid("30DDA08A-1AC8-4E5B-B724-F9A7D223E800"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new Guid("FF670299-D4A2-439B-984D-E71366E1946B"), 29, 3, new DateTime(2025, 5, 15, 16, 41, 39, 953, DateTimeKind.Unspecified).AddTicks(2647) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RankingRodadas",
                keyColumns: new[] { "Id" },
                keyValues: new object[]
                {
                    new Guid("6ED8FD7F-A64F-4CE1-8ADA-1A62281024C2"),
                    new Guid("7CBAAA82-0461-4569-930F-2DBC0E344DA3"),
                    new Guid("C0CA4FA9-6362-414F-8AC2-6052CE752289"),
                    new Guid("2253941A-469C-4080-AD1B-8C7267658E05"),
                    new Guid("59809DCB-4B19-4125-9BB2-B576E0161873"),
                    new Guid("F633B2E6-D3E5-47C2-AC07-B801BAD64C45"),
                    new Guid("CB1FB98C-677C-4863-ADCC-B80D22379DE1"),
                    new Guid("44298748-AEAC-4B18-8F9E-CE1CD0DD7ADE"),
                    new Guid("21747805-A40C-4D96-9008-DBDE35BB17A7"),
                    new Guid("DACB1BA0-6421-4510-B372-DF31DD241768"),
                    new Guid("A9C29302-ACFF-4962-9FC4-F69FC10C3DCD"),
                    new Guid("30DDA08A-1AC8-4E5B-B724-F9A7D223E800")
                });
        }
    }
}