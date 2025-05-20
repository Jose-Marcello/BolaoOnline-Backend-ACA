using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDadosEquipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Equipes",
                columns: new[] { "Id", "Nome", "Sigla", "Escudo", "Tipo", "UfId" },
                values: new object[,]
                {
                    { new Guid("b54f6e57-a123-49b0-a349-08dd29d2bf39"), "Flamengo", "FLA", "1f3964e9-c076-49fd-b5ab-808c813c679a_flamengo_original.png", 1, new Guid("79c34fee-0e99-430f-a9d8-f8b9425f7538") },
                    { new Guid("3a080e03-535f-4900-2d28-08dd29da27cc"), "Fluminense", "FLU", "341b22ef-c38e-4c56-9813-04c2c39142e5_fluminense-original.png", 1, new Guid("79c34fee-0e99-430f-a9d8-f8b9425f7538") },
                    { new Guid("1cd5a292-7aef-4ef5-2d29-08dd29da27cc"), "Botafogo", "BOT", "ba4a7ac5-92fe-4dcc-af67-417ef31f7600_botafogo_original.png", 1, new Guid("79c34fee-0e99-430f-a9d8-f8b9425f7538") },
                    { new Guid("4cd53646-3e32-4787-6a34-08dd2c5aed0e"), "Vasco da Gama", "VAS", "814bdc23-01a1-428d-8282-cc7bdb470b3e_vasco-original.png", 1, new Guid("79c34fee-0e99-430f-a9d8-f8b9425f7538") },
                    { new Guid("76f62637-4714-4db3-44e1-08dd34e915e0"), "Palmeiras", "PAL", "6bef9dd3-ea05-4806-9dc2-a6cec3ca3fdb_palmeiras-original.png", 1, new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("9c7efa8c-9c4e-41ba-44e2-08dd34e915e0"), "Internacional", "INT", "5d49f55c-4ac4-4274-9c9a-4e1c703c070f_internacional-original.png", 1, new Guid("0a42b9b4-e610-499c-b2bb-cf015018c5f8") },
                    { new Guid("e7ea5347-0268-474a-44e3-08dd34e915e0"), "São Paulo", "SPA", "798fd8ba-05db-4bd8-a0f9-3fca7068a38b_sao-paulo-original.png", 1, new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("1817f362-a5af-4666-44e4-08dd34e915e0"), "Cruzeiro", "CRU", "8d5555a5-91d7-473a-a904-b4228ece2d87_cruzeiro-original.png", 1, new Guid("96a27f8e-b44a-41e2-9599-548bfd74d261") },
                    { new Guid("e1b02622-9e86-42a3-44e5-08dd34e915e0"), "Bahia", "BAH", "4ea21fb4-b609-425b-bb0e-5bc325b94e4a_bahia-original.png", 1, new Guid("601254e5-d35d-4ab7-a7ff-3a62956325db") },
                    { new Guid("fe7bef36-d2e0-4a34-44e6-08dd34e915e0"), "Atlético Mineiro", "CAM", "648052d1-d75b-48a8-b98e-2cc6ab00bf42_atletico-mineiro-original.png", 1, new Guid("96a27f8e-b44a-41e2-9599-548bfd74d261") },
                    { new Guid("ca110312-a213-407d-44e7-08dd34e915e0"), "Athlético Paranaense", "CAP", "41a49ca1-7b18-4cae-b76c-48a769276b5e_athletico_paranaense_original.png", 1, new Guid("f7ff578d-a73c-4219-be11-ea908db40678") },
                    { new Guid("235fe750-c7a3-4dfb-44e8-08dd34e915e0"), "Atlético Goianiense", "ATG", "0228bc7b-d18e-49c7-86f5-093fedf33373_atletico-goianiense-original.png", 1, new Guid("e66a1e06-2b69-4bdc-806f-cb6286f7cc9c") },
                    { new Guid("f9701da6-b33f-4ac0-44e9-08dd34e915e0"), "Vitória", "VIT", "e8bae1cf-b4ea-498f-9668-2635c918270e_vitoria_original.png", 1, new Guid("601254e5-d35d-4ab7-a7ff-3a62956325db") },
                    { new Guid("33a6dcaa-e460-428b-44ea-08dd34e915e0"), "Grêmio", "GRE", "efe71655-ef5e-4ce5-b0b1-ab063631dec7_gremio_original.png", 1, new Guid("0a42b9b4-e610-499c-b2bb-cf015018c5f8") },
                    { new Guid("491124af-8235-4bbc-44eb-08dd34e915e0"), "Criciúma", "CRI", "afb47477-175e-4cb8-8cb6-87b178127a8f_Criciuma-original.png", 1, new Guid("0774d1e3-d609-4fc5-8b4d-7918d45cb4f9") },
                    { new Guid("cdbeba90-26ec-4dc1-44ec-08dd34e915e0"), "Juventude", "JUV", "807a5c99-d98a-49e7-ba7a-4fff1377b2a2_juventude-ORIGINAL.png", 1, new Guid("0a42b9b4-e610-499c-b2bb-cf015018c5f8") },
                    { new Guid("876a8563-820c-44fc-44ed-08dd34e915e0"), "Bragantino", "BRA", "153b324e-c5be-4b06-9114-6a0cc3f0bf50_bragantino_original.png", 1, new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("82a19a84-217f-4de0-44ee-08dd34e915e0"), "Fortaleza", "FOR", "21be4a42-8637-44c5-9651-41c8ee2f9621_fortaleza-original.png", 1, new Guid("6b4d639b-cc7c-49c3-a923-0584a8eda24c") },
                    { new Guid("ff495477-8921-4642-44ef-08dd34e915e0"), "Ceará", "CEA", "adf3215f-c2e6-4db5-896b-cb658d704085_ceara-original.png", 1, new Guid("6b4d639b-cc7c-49c3-a923-0584a8eda24c") },
                    { new Guid("88a5877d-9f46-4fbc-44f0-08dd34e915e0"), "Sport", "SPO", "80446194-457e-438b-a65b-ed3165ff969e_sport-original.png", 1, new Guid("92df3b85-57a7-4f78-84bf-ad35696a5fad") },
                    { new Guid("fed3348d-cc22-46cb-44f1-08dd34e915e0"), "Santos", "SAN", "eae774e5-248b-4351-8493-e3f428340bfd_santos-original.png", 1, new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("254d0b8c-5c41-4342-44f2-08dd34e915e0"), "Mirassol", "MIR", "aeec7f61-7366-4538-9033-946f1d99de30_mirassol-original.png", 1, new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("ea30bc89-b510-4ee6-a354-08dd34ef142a"), "Corinthians", "COR", "9c0da88e-75ab-4a06-aae1-100e3f940c1a_Corinthians-original.png", 1, new Guid("bbff2f35-1dee-4f34-8b63-d5a315f22607") },
                    { new Guid("84d1da94-e3ba-431c-a355-08dd34ef142a"), "Cuiabá", "CUI", "50f24080-f4b0-49d7-b11b-5b60da35a2ec_cuiaba-original.png", 1, new Guid("d3ce51d3-7f76-4432-a886-63dc4715a2d2") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Equipes",
                keyColumns: new[] { "Id" },
                keyValues: new object[]
                {
            new Guid("1817f362-a5af-4666-44e4-08dd34e915e0"),
            new Guid("1cd5a292-7aef-4ef5-2d29-08dd29da27cc"),
            new Guid("235fe750-c7a3-4dfb-44e8-08dd34e915e0"),
            new Guid("254d0b8c-5c41-4342-44f2-08dd34e915e0"),
            new Guid("33a6dcaa-e460-428b-44ea-08dd34e915e0"),
            new Guid("3a080e03-535f-4900-2d28-08dd29da27cc"),
            new Guid("491124af-8235-4bbc-44eb-08dd34e915e0"),
            new Guid("4cd53646-3e32-4787-6a34-08dd2c5aed0e"),
            new Guid("76f62637-4714-4db3-44e1-08dd34e915e0"),
            new Guid("82a19a84-217f-4de0-44ee-08dd34e915e0"),
            new Guid("84d1da94-e3ba-431c-a355-08dd34ef142a"),
            new Guid("876a8563-820c-44fc-44ed-08dd34e915e0"),
            new Guid("88a5877d-9f46-4fbc-44f0-08dd34e915e0"),
            new Guid("9c7efa8c-9c4e-41ba-44e2-08dd34e915e0"),
            new Guid("b54f6e57-a123-49b0-a349-08dd29d2bf39"),
            new Guid("ca110312-a213-407d-44e7-08dd34e915e0"),
            new Guid("cdbeba90-26ec-4dc1-44ec-08dd34e915e0"),
            new Guid("e1b02622-9e86-42a3-44e5-08dd34e915e0"),
            new Guid("e7ea5347-0268-474a-44e3-08dd34e915e0"),
            new Guid("ea30bc89-b510-4ee6-a354-08dd34ef142a"),
            new Guid("f9701da6-b33f-4ac0-44e9-08dd34e915e0"),
            new Guid("fed3348d-cc22-46cb-44f1-08dd34e915e0"),
            new Guid("fe7bef36-d2e0-4a34-44e6-08dd34e915e0"),
            new Guid("ff495477-8921-4642-44ef-08dd34e915e0")
                });
        }
    }
}