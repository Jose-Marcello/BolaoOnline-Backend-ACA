using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarJogos : Migration
    {      
        /// <inheritdoc />       
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.InsertData(
                    table: "Jogos",
                    columns: new[] { "Id", "RodadaId", "DataJogo", "HoraJogo", "EstadioId", "EquipeCasaId", "EquipeVisitanteId", "PlacarCasa", "PlacarVisita", "Status" },
                    values: new object[,]
                    {
                    { new Guid("83A1D7BF-F2D6-4B7A-7145-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("8F34A4EC-DE6B-4C43-23C8-08DD2F25D992"), new Guid("C7D5269A-80D8-4412-91C8-08DD8C8FB6DF"), new Guid("E77A8FB9-4B79-4070-DF3F-08DD8C8EEB97"), 0, 0, 2 },
                    { new Guid("5A5C1401-0F5F-4A4A-7146-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("50F93910-1396-407B-89B8-08DD30DEDAE9"), new Guid("AC871AA9-08AC-4C3F-91C6-08DD8C8FB6DF"), new Guid("289EF14C-9A58-40DD-DF41-08DD8C8EEB97"), 2, 1, 2 },
                    { new Guid("E433085A-FA83-4450-7147-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("2DABA58D-CD14-432B-795B-08DD34F1B353"), new Guid("821CE6CE-3203-4620-DF3A-08DD8C8EEB97"), new Guid("45531351-E348-4057-91C9-08DD8C8FB6DF"), 2, 0, 2 },
                    { new Guid("B26CA879-0B3A-4F3A-7148-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("D4FB31A3-397A-4067-795A-08DD34F1B353"), new Guid("BCA1295C-A1EA-42AB-DF3D-08DD8C8EEB97"), new Guid("39C6A03B-DEC0-4e21-DF34-08DD8C8EEB97"), 2, 0, 2 },
                    { new Guid("ACF97B53-631F-4B44-7149-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("05E55687-2B4E-4A0C-CBB8-08DD8CDEE95D"), new Guid("5CA14292-CA9B-4823-DF3B-08DD8C8EEB97"), new Guid("D5AE20CE-9A38-4F33-DF39-08DD8C8EEB97"), 2, 0, 2 },
                    { new Guid("4AA7EBC4-3F83-4EE5-714A-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(21, 0, 0), new Guid("B34B22A0-E61E-4093-23C7-08DD2F25D992"), new Guid("7B4FE02F-FDCD-46D5-DF33-08DD8C8EEB97"), new Guid("126E2C58-C339-4872-DF38-08DD8C8EEB97"), 1, 1, 2 },
                    { new Guid("A4AD06FB-84EA-4256-714B-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(16, 0, 0), new Guid("1645135B-FD1D-4626-7957-08DD34F1B353"), new Guid("9B396A95-EBC8-496B-DF37-08DD8C8EEB97"), new Guid("B9F5A70C-53AA-4F85-DF35-08DD8C8EEB97"), 0, 0, 2 },
                    { new Guid("B27533A2-C612-4435-714C-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("7DFDDBF2-A8BF-4AF6-795E-08DD34F1B353"), new Guid("34D2C704-9903-44F7-DF36-08DD8C8EEB97"), new Guid("1E9475A7-AF66-4498-DF40-08DD8C8EEB97"), 2, 1, 2 },
                    { new Guid("2975AB97-9FBB-4065-714D-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(20, 0, 0), new Guid("23220D98-1152-4254-7962-08DD34F1B353"), new Guid("9005F0CC-26CA-449F-DF3C-08DD8C8EEB97"), new Guid("2F8461E5-FB06-4BD1-DF3E-08DD8C8EEB97"), 2, 2, 2 },
                    { new Guid("A7565412-5BC0-4AEF-714E-08DD8CD6453F"), new Guid("84A52203-E860-475F-62C6-08DD8C90086D"), new DateTime(2025, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(20, 0, 0), new Guid("D3050873-6C71-41EB-7959-08DD34F1B353"), new Guid("EA66793E-7AFD-41CD-91C7-08DD8C8FB6DF"), new Guid("368F1EB7-BE1A-442F-3162-08DD8CDFE199"), 1, 1, 2 },
                    { new Guid("56CE7704-CFD0-48FE-1C10-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("0121A9D9-BCF3-4734-7960-08DD34F1B353"), new Guid("368F1EB7-BE1A-442F-3162-08DD8CDFE199"), new Guid("34D2C704-9903-44F7-DF36-08DD8C8EEB97"), null, null, 0 },
                    { new Guid("F8156736-A15E-4F3E-1C11-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("D4FB31A3-397A-4067-795A-08DD34F1B353"), new Guid("2F8461E5-FB06-4BD1-DF3E-08DD8C8EEB97"), new Guid("821CE6CE-3203-4620-DF3A-08DD8C8EEB97"), null, null, 0 },
                    { new Guid("DC76106A-2B70-4614-1C12-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(21, 0, 0), new Guid("6436408C-A14C-460B-23C6-08DD2F25D992"), new Guid("B9F5A70C-53AA-4F85-DF35-08DD8C8EEB97"), new Guid("5CA14292-CA9B-4823-DF3B-08DD8C8EEB97"), null, null, 0 },
                    { new Guid("F2DDA750-7A82-48C8-1C13-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(16, 0, 0), new Guid("B34B22A0-E61E-4093-23C7-08DD2F25D992"), new Guid("39C6A03B-DEC0-4E21-DF34-08DD8C8EEB97"), new Guid("9005F0CC-26CA-449F-DF3C-08DD8C8EEB97"), null, null, 0 },
                    { new Guid("6A7E960B-7D32-4F5F-1C14-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(16, 0, 0), new Guid("50F93910-1396-407B-89B8-08DD30DEDAE9"), new Guid("45531351-E348-4057-91C9-08DD8C8FB6DF"), new Guid("C7D5269A-80D8-4412-91C8-08DD8C8FB6DF"), null, null, 0 },
                    { new Guid("2737C263-6C93-49D9-1C15-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 3, 0), new Guid("266E6EC0-1DAE-4D41-DD13-08DD7B7E8A7E"), new Guid("289EF14C-9A58-40DD-DF41-08DD8C8EEB97"), new Guid("BCA1295C-A1EA-42AB-DF3D-08DD8C8EEB97"), null, null, 0 },
                    { new Guid("005B5FD5-8BF6-4D63-1C16-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("630651B9-B97D-4F28-7955-08DD34F1B353"), new Guid("126E2C58-C339-4872-DF38-08DD8C8EEB97"), new Guid("AC871AA9-08AC-4C3F-91C6-08DD8C8FB6DF"), null, null, 0 },
                    { new Guid("344BB27E-8029-4045-1C17-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("902A93C4-AB1E-4148-DD14-08DD7B7E8A7E"), new Guid("D5AE20CE-9A38-4F33-DF39-08DD8C8EEB97"), new Guid("7B4FE02F-FDCD-46D5-DF33-08DD8C8EEB97"), null, null, 0 },
                    { new Guid("1234E72D-7D93-4144-1C18-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(18, 30, 0), new Guid("E01CA1BD-1533-4CE8-DD16-08DD7B7E8A7E"), new Guid("E77A8FB9-4B79-4070-DF3F-08DD8C8EEB97"), new Guid("9B396A95-EBC8-496B-DF37-08DD8C8EEB97"), null, null, 0 },
                    { new Guid("719E27C4-029C-4A3F-1C19-08DD923C97F5"), new Guid("AD7E6396-5E5C-4E5A-7482-08DD923C346E"), new DateTime(2025, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(20, 30, 0), new Guid("9882A91F-64AC-4404-DD15-08DD7B7E8A7E"), new Guid("1E9475A7-AF66-4498-DF40-08DD8C8EEB97"), new Guid("EA66793E-7AFD-41CD-91C7-08DD8C8FB6DF"), null, null, 0 },
                    { new Guid("6EF24864-7FDA-48BC-1C1A-08DD923C97F5"), new Guid("8544682F-CFA0-4929-7483-08DD923C346E"), new DateTime(2025, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(16, 0, 0), new Guid("23220D98-1152-4254-7962-08DD34F1B353"), new Guid("9005F0CC-26CA-449F-DF3C-08DD8C8EEB97"), new Guid("B9F5A70C-53AA-4F85-DF35-08DD8C8EEB97"), null, null, 0 },
                    { new Guid("AFDA835F-4A4D-4499-1C1B-08DD923C97F5"), new Guid("8544682F-CFA0-4929-7483-08DD923C346E"), new DateTime(2025, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(15, 0, 0), new Guid("05E55687-2B4E-4A0C-CBB8-08DD8CDEE95D"), new Guid("5CA14292-CA9B-4823-DF3B-08DD8C8EEB97"), new Guid("2F8461E5-FB06-4BD1-DF3E-08DD8C8EEB97"), null, null, 0 }
                });
            }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Jogos",
                keyColumns: new[] { "Id" },
                keyValues: new object[]
                {
                    new Guid("83A1D7BF-F2D6-4B7A-7145-08DD8CD6453F"),
                    new Guid("5A5C1401-0F5F-4A4A-7146-08DD8CD6453F"),
                    new Guid("E433085A-FA83-4450-7147-08DD8CD6453F"),
                    new Guid("B26CA879-0B3A-4F3A-7148-08DD8CD6453F"),
                    new Guid("ACF97B53-631F-4B44-7149-08DD8CD6453F"),
                    new Guid("4AA7EBC4-3F83-4EE5-714A-08DD8CD6453F"),
                    new Guid("A4AD06FB-84EA-4256-714B-08DD8CD6453F"),
                    new Guid("B27533A2-C612-4435-714C-08DD8CD6453F"),
                    new Guid("2975AB97-9FBB-4065-714D-08DD8CD6453F"),
                    new Guid("A7565412-5BC0-4AEF-714E-08DD8CD6453F"),
                    new Guid("56CE7704-CFD0-48FE-1C10-08DD923C97F5"),
                    new Guid("F8156736-A15E-4F3E-1C11-08DD923C97F5"),
                    new Guid("DC76106A-2B70-4614-1C12-08DD923C97F5"),
                    new Guid("F2DDA750-7A82-48C8-1C13-08DD923C97F5"),
                    new Guid("6A7E960B-7D32-4F5F-1C14-08DD923C97F5"),
                    new Guid("2737C263-6C93-49D9-1C15-08DD923C97F5"),
                    new Guid("005B5FD5-8BF6-4D63-1C16-08DD923C97F5"),
                    new Guid("344BB27E-8029-4045-1C17-08DD923C97F5"),
                    new Guid("1234E72D-7D93-4144-1C18-08DD923C97F5"),
                    new Guid("719E27C4-029C-4A3F-1C19-08DD923C97F5"),
                    new Guid("6EF24864-7FDA-48BC-1C1A-08DD923C97F5"),
                    new Guid("AFDA835F-4A4D-4499-1C1B-08DD923C97F5")
                });
        
            
        }
    }
}
