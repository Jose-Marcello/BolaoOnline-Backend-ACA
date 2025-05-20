using Microsoft.EntityFrameworkCore.Migrations;
using System;
#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDadosUfs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ufs",
                columns: new[] { "Id", "Nome", "Sigla" },
                values: new object[,]
                {
                    { Guid.Parse("11E1BD14-6B1B-487F-9D21-0085EB0E783E"), "Amapá", "AP" },
                    { Guid.Parse("6B4D639B-CC7C-49C3-A923-0584A8EDA24C"), "Ceará", "CE" },
                    { Guid.Parse("35F20917-0202-4279-A69B-196739E4BCC9"), "Amazonas", "AM" },
                    { Guid.Parse("D72A73B3-A9B0-469E-990A-19B7E4EAE0AC"), "Distrito Federal", "DF" },
                    { Guid.Parse("A0E51943-BC7E-495C-A25C-1B861AB68857"), "Piauí", "PI" },
                    { Guid.Parse("601254E5-D35D-4AB7-A7FF-3A62956325DB"), "Bahia", "BA" },
                    { Guid.Parse("40ED3A74-55DE-4411-99CC-40A58BB7F3C4"), "Alagoas", "AL" },
                    { Guid.Parse("2A6A2F33-0B22-4480-8721-447D80DB59E9"), "Espírito Santo", "ES" },
                    { Guid.Parse("96A27F8E-B44A-41E2-9599-548BFD74D261"), "Minas Gerais", "MG" },
                    { Guid.Parse("B5BE8930-678A-4748-BBFF-566033C6CB5F"), "Sergipe", "SE" },
                    { Guid.Parse("9F6411EA-2A82-441A-B6D5-5DFE09610D43"), "Acre", "AC" },
                    { Guid.Parse("D3CE51D3-7F76-4432-A886-63DC4715A2D2"), "Mato Grosso", "MT" },
                    { Guid.Parse("0774D1E3-D609-4FC5-8B4D-7918D45CB4F9"), "Santa Catarina", "SC" },
                    { Guid.Parse("5E1601DB-213E-4CF3-8613-A08A3C9ADCF2"), "Paraíba", "PB" },
                    { Guid.Parse("8294226C-07B0-4453-B03B-A7E1E8B6B942"), "Rondônia", "RO" },
                    { Guid.Parse("92DF3B85-57A7-4F78-84BF-AD35696A5FAD"), "Pernambuco", "PE" },
                    { Guid.Parse("3EF12E80-3F8C-4EAF-9C13-B4CC0FF4E45A"), "Rio Grande do Norte", "RN" },
                    { Guid.Parse("DA84CE71-FC89-4580-8A97-BD6A8552C056"), "Pará", "PA" },
                    { Guid.Parse("E66A1E06-2B69-4BDC-806F-CB6286F7CC9C"), "Goiás", "GO" },
                    { Guid.Parse("A796DE1B-2432-4C82-91B1-CDEF0BD4782E"), "Roraima", "RR" },
                    { Guid.Parse("0A42B9B4-E610-499C-B2BB-CF015018C5F8"), "Rio Grande do Sul", "RS" },
                    { Guid.Parse("5577ABC3-43E1-4087-9097-D107AD76C6D2"), "Tocantins", "TO" },
                    { Guid.Parse("BBFF2F35-1DEE-4F34-8B63-D5A315F22607"), "São Paulo", "SP" },
                    { Guid.Parse("F7FF578D-A73C-4219-BE11-EA908DB40678"), "Paraná", "PR" },
                    { Guid.Parse("2AD0722F-0CB4-46AE-AA8C-F63D8AA33922"), "Maranhão", "MA" },
                    { Guid.Parse("A4353E4C-5355-4C47-8CC6-F6BA990DE591"), "Mato Grosso do Sul", "MS" },
                    { Guid.Parse("79C34FEE-0E99-430F-A9D8-F8B9425F7538"), "Rio de Janeiro", "RJ" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ufs",
                keyColumns: new[] { "Id" }, // "Id" como string dentro de um array
                keyValues: new object[]
                {
                    Guid.Parse("11E1BD14-6B1B-487F-9D21-0085EB0E783E"),
                    Guid.Parse("6B4D639B-CC7C-49C3-A923-0584A8EDA24C"),
                    Guid.Parse("35F20917-0202-4279-A69B-196739E4BCC9"),
                    Guid.Parse("D72A73B3-A9B0-469E-990A-19B7E4EAE0AC"),
                    Guid.Parse("A0E51943-BC7E-495C-A25C-1B861AB68857"),
                    Guid.Parse("601254E5-D35D-4AB7-A7FF-3A62956325DB"),
                    Guid.Parse("40ED3A74-55DE-4411-99CC-40A58BB7F3C4"),
                    Guid.Parse("2A6A2F33-0B22-4480-8721-447D80DB59E9"),
                    Guid.Parse("96A27F8E-B44A-41E2-9599-548BFD74D261"),
                    Guid.Parse("B5BE8930-678A-4748-BBFF-566033C6CB5F"),
                    Guid.Parse("9F6411EA-2A82-441A-B6D5-5DFE09610D43"),
                    Guid.Parse("D3CE51D3-7F76-4432-A886-63DC4715A2D2"),
                    Guid.Parse("0774D1E3-D609-4FC5-8B4D-7918D45CB4F9"),
                    Guid.Parse("5E1601DB-213E-4CF3-8613-A08A3C9ADCF2"),
                    Guid.Parse("8294226C-07B0-4453-B03B-A7E1E8B6B942"),
                    Guid.Parse("92DF3B85-57A7-4F78-84BF-AD35696A5FAD"),
                    Guid.Parse("3EF12E80-3F8C-4EAF-9C13-B4CC0FF4E45A"),
                    Guid.Parse("DA84CE71-FC89-4580-8A97-BD6A8552C056"),
                    Guid.Parse("E66A1E06-2B69-4BDC-806F-CB6286F7CC9C"),
                    Guid.Parse("A796DE1B-2432-4C82-91B1-CDEF0BD4782E"),
                    Guid.Parse("0A42B9B4-E610-499C-B2BB-CF015018C5F8"),
                    Guid.Parse("5577ABC3-43E1-4087-9097-D107AD76C6D2"),
                    Guid.Parse("BBFF2F35-1DEE-4F34-8B63-D5A315F22607"),
                    Guid.Parse("F7FF578D-A73C-4219-BE11-EA908DB40678"),
                    Guid.Parse("2AD0722F-0CB4-46AE-AA8C-F63D8AA33922"),
                    Guid.Parse("A4353E4C-5355-4C47-8CC6-F6BA990DE591"),
                    Guid.Parse("79C34FEE-0E99-430F-A9D8-F8B9425F7538")
                });
        }
    }
}