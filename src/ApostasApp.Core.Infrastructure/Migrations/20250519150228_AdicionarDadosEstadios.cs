using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDadosEstadios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Estadios",
                columns: new[] { "Id", "Nome", "UfId" },
                values: new object[,]
                {
                    { new Guid("6436408c-a14c-460b-23c6-08dd2f25d992"), "Nilton Santos (Engenhão)", new Guid("79c34fee-0e99-430f-a9d8-f8b9425f7538") },
                    { new Guid("b34b22a0-e61e-4093-23c7-08dd2f25d992"), "Mário Filho (Maracanã)", new Guid("79c34fee-0e99-430f-a9d8-f8b9425f7538") },
                    { new Guid("8f34a4ec-de6b-4c43-23c8-08dd2f25d992"), "Cicero Pompeu de Toledo (Morumbis)", new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("50f93910-1396-407b-89b8-08dd30dedae9"), "Magalhães Pinto (Mineirão)", new Guid("96a27f8e-b44a-41e2-9599-548bfd74d261") },
                    { new Guid("630651b9-b97d-4f28-7955-08dd34f1b353"), "Beira Rio", new Guid("0a42b9b4-e610-499c-b2bb-cf015018c5f8") },
                    { new Guid("1645135b-fd1d-4626-7957-08dd34f1b353"), "Allianz Arena", new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("d3050873-6c71-41eb-7959-08dd34f1b353"), "Fonte Nova", new Guid("601254e5-d35d-4ab7-a7ff-3a62956325db") },
                    { new Guid("d4fb31a3-397a-4067-795a-08dd34f1b353"), "Arena Castelão", new Guid("6b4d639b-cc7c-49c3-a923-0584a8eda24c") },
                    { new Guid("2daba58d-cd14-432b-795b-08dd34f1b353"), "Arena Grêmio", new Guid("0a42b9b4-e610-499c-b2bb-cf015018c5f8") },
                    { new Guid("b97fe8ee-ec40-4e0f-795c-08dd34f1b353"), "Heriberto Hulse", new Guid("0774d1e3-d609-4fc5-8b4d-7918d45cb4f9") },
                    { new Guid("7dfddbf2-a8bf-4af6-795e-08dd34f1b353"), "São Januário", new Guid("79c34fee-0e99-430f-a9d8-f8b9425f7538") },
                    { new Guid("ab3db143-2d19-485e-795f-08dd34f1b353"), "Ligga Arena", new Guid("f7ff578d-a73c-4219-be11-ea908db40678") },
                    { new Guid("0121a9d9-bcf3-4734-7960-08dd34f1b353"), "Neo Química Arena", new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("662adeb0-b248-4c28-7961-08dd34f1b353"), "Arena MRV", new Guid("96a27f8e-b44a-41e2-9599-548bfd74d261") },
                    { new Guid("23220d98-1152-4254-7962-08dd34f1b353"), "Nabi Abi Chedid", new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("06647b11-6fc3-400b-7963-08dd34f1b353"), "Arena Pantanal", new Guid("d3ce51d3-7f76-4432-a886-63dc4715a2d2") },
                    { new Guid("c9c7d681-1d5c-46a2-d5ce-08dd35a03205"), "Serra Courada", new Guid("e66a1e06-2b69-4bdc-806f-cb6286f7cc9c") },
                    { new Guid("344c9196-14b1-4ee7-cf54-08dd4acacaa2"), "Arena Barueri", new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("266e6ec0-1dae-4d41-dd13-08dd7b7e8a7e"), "Maião", new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("902a93c4-ab1e-4148-dd14-08dd7b7e8a7e"), "Barradão", new Guid("601254e5-d35d-4ab7-a7ff-3a62956325db") },
                    { new Guid("9882a91f-64ac-4404-dd15-08dd7b7e8a7e"), "Vila Belmiro", new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("e01ca1bd-1533-4ce8-dd16-08dd7b7e8a7e"), "Ilha do Retiro", new Guid("92df3b85-57a7-4f78-84bf-ad35696a5fad") },
                    { new Guid("85526cd8-ca84-41a8-dd17-08dd7b7e8a7e"), "Arena Pernambuco", new Guid("92df3b85-57a7-4f78-84bf-ad35696a5fad") },
                    { new Guid("05e55687-2b4e-4a0c-cbb8-08dd8cdee95d"), "Alfredo\u00A0 Jaconi", new Guid("0a42b9b4-e610-499c-b2bb-cf015018c5f8") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Estadios",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("0121a9d9-bcf3-4734-7960-08dd34f1b353"),
                    new Guid("05e55687-2b4e-4a0c-cbb8-08dd8cdee95d"),
                    new Guid("06647b11-6fc3-400b-7963-08dd34f1b353"),
                    new Guid("1645135b-fd1d-4626-7957-08dd34f1b353"),
                    new Guid("23220d98-1152-4254-7962-08dd34f1b353"),
                    new Guid("266e6ec0-1dae-4d41-dd13-08dd7b7e8a7e"),
                    new Guid("2daba58d-cd14-432b-795b-08dd34f1b353"),
                    new Guid("344c9196-14b1-4ee7-cf54-08dd4acacaa2"),
                    new Guid("50f93910-1396-407b-89b8-08dd30dedae9"),
                    new Guid("630651b9-b97d-4f28-7955-08dd34f1b353"),
                    new Guid("6436408c-a14c-460b-23c6-08dd2f25d992"),
                    new Guid("662adeb0-b248-4c28-7961-08dd34f1b353"),
                    new Guid("7dfddbf2-a8bf-4af6-795e-08dd34f1b353"),
                    new Guid("85526cd8-ca84-41a8-dd17-08dd7b7e8a7e"),
                    new Guid("8f34a4ec-de6b-4c43-23c8-08dd2f25d992"),
                    new Guid("902a93c4-ab1e-4148-dd14-08dd7b7e8a7e"),
                    new Guid("9882a91f-64ac-4404-dd15-08dd7b7e8a7e"),
                    new Guid("ab3db143-2d19-485e-795f-08dd34f1b353"),
                    new Guid("b34b22a0-e61e-4093-23c7-08dd2f25d992"),
                    new Guid("b97fe8ee-ec40-4e0f-795c-08dd34f1b353"),
                    new Guid("c9c7d681-1d5c-46a2-d5ce-08dd35a03205"),
                    new Guid("d3050873-6c71-41eb-7959-08dd34f1b353"),
                    new Guid("d4fb31a3-397a-4067-795a-08dd34f1b353"),
                    new Guid("e01ca1bd-1533-4ce8-dd16-08dd7b7e8a7e")
                });
        }
    }
}