using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarApostadoresCampeonatos : Migration
    {
           /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "ApostadoresCampeonatos",
                    columns: new[] { "Id", "CampeonatoId", "ApostadorId", "Pontuacao", "Posicao" },
                    values: new object[,]
                    {
                    { new Guid("6B62054D-0849-4337-AF9D-005FEAE6C7EE"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("2FFF3E52-D7E6-4040-BAA4-8B22F33F09EE"), 0, 0 },
                    { new Guid("0E684317-1FDE-473E-AED5-6783CA527B30"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("34DFAE09-DC0D-4A95-8A5C-B1FD018B6FF1"), 0, 0 },
                    { new Guid("70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("F895771E-8B21-4BAB-B49D-A017483ACFCA"), 0, 0 },
                    { new Guid("0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("581A35E3-66A4-41E5-99FF-691D91DFD3A5"), 0, 0 },
                    { new Guid("FF670299-D4A2-439B-984D-E71366E1946B"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("EF74875F-9FB8-4920-AF00-369C4F3F7FED"), 0, 0 },
                    { new Guid("92743816-E057-449C-8D62-F973F584918A"), new Guid("DF3F9BD6-41AE-4867-D0FB-08DD8C07D71C"), new Guid("D6CD921C-C824-4327-8885-A28ACAEF082D"), 0, 0 }
                    });
            }

            /// <inheritdoc />
            protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DeleteData(
                    table: "ApostadoresCampeonatos",
                    keyColumns: new[] { "Id" },
                    keyValues: new object[]
                    {
                    new Guid("6B62054D-0849-4337-AF9D-005FEAE6C7EE"),
                    new Guid("0E684317-1FDE-473E-AED5-6783CA527B30"),
                    new Guid("70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F"),
                    new Guid("0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6"),
                    new Guid("FF670299-D4A2-439B-984D-E71366E1946B"),
                    new Guid("92743816-E057-449C-8D62-F973F584918A")
                    });
            }
        }
    }