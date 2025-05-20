using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarEquipesCampeonatos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EquipesCampeonatos",
                columns: new[] { "Id", "CampeonatoId", "EquipeId" },
                values: new object[,]
                {
                    { new Guid("7B4FE02F-FDCD-46D5-DF33-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("B54F6E57-A123-49B0-A349-08DD29D2BF39") },
                    { new Guid("39C6A03B-DEC0-4E21-DF34-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("3A080E03-535F-4900-2D28-08DD29DA27CC") },
                    { new Guid("B9F5A70C-53AA-4F85-DF35-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("1CD5A292-7AEF-4EF5-2D29-08DD29DA27CC") },
                    { new Guid("34D2C704-9903-44F7-DF36-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("4CD53646-3E32-4787-6A34-08DD2C5AED0E") },
                    { new Guid("9B396A95-EBC8-496B-DF37-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("76F62637-4714-4DB3-44E1-08DD34E915E0") },
                    { new Guid("126E2C58-C339-4872-DF38-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("9C7EFA8C-9C4E-41BA-44E2-08DD34E915E0") },
                    { new Guid("D5AE20CE-9A38-4F33-DF39-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("F9701DA6-B33F-4AC0-44E9-08DD34E915E0") },
                    { new Guid("821CE6CE-3203-4620-DF3A-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("33A6DCAA-E460-428B-44EA-08DD34E915E0") },
                    { new Guid("5CA14292-CA9B-4823-DF3B-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("CDBEBA90-26EC-4DC1-44EC-08DD34E915E0") },
                    { new Guid("9005F0CC-26CA-449F-DF3C-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("876A8563-820C-44FC-44ED-08DD34E915E0") },
                    { new Guid("BCA1295C-A1EA-42AB-DF3D-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("82A19A84-217F-4DE0-44EE-08DD34E915E0") },
                    { new Guid("2F8461E5-FB06-4BD1-DF3E-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("FF495477-8921-4642-44EF-08DD34E915E0") },
                    { new Guid("E77A8FB9-4B79-4070-DF3F-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("88A5877D-9F46-4FBC-44F0-08DD34E915E0") },
                    { new Guid("1E9475A7-AF66-4498-DF40-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("FED3348D-CC22-46CB-44F1-08DD34E915E0") },
                    { new Guid("289EF14C-9A58-40DD-DF41-08DD8C8EEB97"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("254D0B8C-5C41-4342-44F2-08DD34E915E0") },
                    { new Guid("AC871AA9-08AC-4C3F-91C6-08DD8C8FB6DF"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("1817F362-A5AF-4666-44E4-08DD34E915E0") },
                    { new Guid("EA66793E-7AFD-41CD-91C7-08DD8C8FB6DF"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("E1B02622-9E86-42A3-44E5-08DD34E915E0") },
                    { new Guid("C7D5269A-80D8-4412-91C8-08DD8C8FB6DF"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("E7EA5347-0268-474A-44E3-08DD34E915E0") },
                    { new Guid("45531351-E348-4057-91C9-08DD8C8FB6DF"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("FE7BEF36-D2E0-4A34-44E6-08DD34E915E0") },
                    { new Guid("368F1EB7-BE1A-442F-3162-08DD8CDFE199"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("EA30BC89-B510-4EE6-A354-08DD34EF142A") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EquipesCampeonatos",
                keyColumns: new[] { "Id" },
                keyValues: new object[]
                {
                    new Guid("7B4FE02F-FDCD-46D5-DF33-08DD8C8EEB97"),
                    new Guid("39C6A03B-DEC0-4E21-DF34-08DD8C8EEB97"),
                    new Guid("B9F5A70C-53AA-4F85-DF35-08DD8C8EEB97"),
                    new Guid("34D2C704-9903-44F7-DF36-08DD8C8EEB97"),
                    new Guid("9B396A95-EBC8-496B-DF37-08DD8C8EEB97"),
                    new Guid("126E2C58-C339-4872-DF38-08DD8C8EEB97"),
                    new Guid("D5AE20CE-9A38-4F33-DF39-08DD8C8EEB97"),
                    new Guid("821CE6CE-3203-4620-DF3A-08DD8C8EEB97"),
                    new Guid("5CA14292-CA9B-4823-DF3B-08DD8C8EE97"),
                    new Guid("9005F0CC-26CA-449F-DF3C-08DD8C8EE97"),
                    new Guid("BCA1295C-A1EA-42AB-DF3D-08DD8C8EE97"),
                    new Guid("2F8461E5-FB06-4BD1-DF3E-08DD8C8EE97"),
                    new Guid("E77A8FB9-4B79-4070-DF3F-08DD8C8EE97"),
                    new Guid("1E9475A7-AF66-4498-DF40-08DD8C8EE97"),
                    new Guid("289EF14C-9A58-40DD-DF41-08DD8C8EE97"),
                    new Guid("AC871AA9-08AC-4C3F-91C6-08DD8C8FB6DF"),
                    new Guid("EA66793E-7AFD-41CD-91C7-08DD8C8FB6DF"),
                    new Guid("C7D5269A-80D8-4412-91C8-08DD8C8FB6DF"),
                    new Guid("45531351-E348-4057-91C9-08DD8C8FB6DF"),
                    new Guid("368F1EB7-BE1A-442F-3162-08DD8CDFE199")
                });
        }
    }
}
