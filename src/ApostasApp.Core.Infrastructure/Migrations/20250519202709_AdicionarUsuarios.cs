using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUsuarios : Migration
    {
        /// <inheritdoc />
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "CPF", "Celular", "Apelido", "RegistrationDate", "LastLoginDate", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount" },
                values: new object[,]
                {
                    { "07d59dc8-f02c-458e-be85-90c442f6c945", "14852123756", "(21) 99973-4775", "RivasBolão", new DateTime(2025, 5, 7, 19, 45, 0, 485, DateTimeKind.Unspecified).AddTicks(7256), null, "jmgalem@gmail.com", "JMGALEM@GMAIL.COM", "jmgalem@gmail.com", "JMGALEM@GMAIL.COM", true, "AQAAAAIAAYagAAAAEKrEeiA+TKTzocXkUsYW7+yz7RT108FWoxHX3dquo9bUuwaZqj07e4COwgCFuesVPw==", "XUOK33H3INF6TLN7J54QE6GWFVWQ3UOT", "fc2d1144-71fe-4e5b-8976-909c9628d752", null, false, false, null, true, 0 },
                    { "155af3ee-c56b-495e-b5a1-f34e5d8e380f", "96701958028", "(21) 99780-9098", "Vini2025", new DateTime(2025, 5, 7, 18, 15, 48, 115, DateTimeKind.Unspecified).AddTicks(6504), null, "jmgalem4@gmail.com", "JMGALEM4@GMAIL.COM", "jmgalem4@gmail.com", "JMGALEM4@GMAIL.COM", true, "AQAAAAIAAYagAAAAEIhpc0uGDotyBL0A0kR6fKGbdJ2hVXjWY127P5WQUYERkzwZSmmO61bCO1jzaN9g9A==", "HLZ332RBI2L2NF6GCLW3R7PBR6RCUDSI", "ac22ae08-bd6c-4880-98a5-65830a6e87bb", null, false, false, null, true, 0 },
                    { "42aca2c4-ca07-48f9-992a-983ee5aa5344", "84062274787", "21999734776", "Josè Marcello(Admin)", new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "jmgalem2@gmail.com", "JMGALEM2@GMAIL.COM", "jmgalem2@gmail.co,", "JMGALEM2@GMAIL.COM", true, "AQAAAAIAAYagAAAAEGNOk2DGWQ/DVHWOgZwveW04u8VQDCt41aXaWBeeuSI4Jcd4U4u91Sb/uL/CK4Jziw==", "2ZROXQOO7PLLZ7PATTPVDOWK76FWAF52", "1b5f6de0-23e2-4d85-975f-4d8e9f877358", null, false, false, null, true, 0 },
                    { "641d31f8-5b3c-40ec-a968-eb13c154d336", "84062274787", "(21) 99973-4776", "ZeMarcello", new DateTime(2025, 5, 5, 16, 14, 24, 235, DateTimeKind.Unspecified).AddTicks(2586), null, "josemarcellogardeldealemar@gmail.com", "JOSEMARCELLOGARDELDEALEMAR@GMAIL.COM", "josemarcellogardeldealemar@gmail.com", "JOSEMARCELLOGARDELDEALEMAR@GMAIL.COM", true, "AQAAAAIAAYagAAAAEF7Zrp2yKOW4neDQiyHsfZwrHamYr+De0SD4RFKtMf0GIu1cwcuYIX4tMoWcEqne6g==", "IJMTLPRCQANCRFVTCQS35PA7H3DVQOC6", "b859b43b-bf9f-44f7-b4bc-fca2ea4613d5", null, false, false, null, true, 0 },
                    { "725831c6-db84-463c-bc2e-ec2a86f69f4b", "54052610091", "(21) 99780-9098", "JocaDoBolão", new DateTime(2025, 5, 7, 16, 42, 54, 425, DateTimeKind.Unspecified).AddTicks(1915), null, "jmgalem3@gmail.com", "JMGALEM3@GMAIL.COM", "jmgalem3@gmail.com", "JMGALEM3@GMAIL.COM", true, "AQAAAAIAAYagAAAAEDbhX5UoqSF5v37aMoijgdnODXj0AgpwGJ6SXAuiY0Oa7ENzy05CepRii5kHSnCqbg==", "X2MXDXWTOXRRLXSEBUXSX765ROOUYNDW", "8f2e4747-4089-4e59-a193-9519a93a4da1", null, false, false, null, true, 0 },
                    { "846a9285-bf07-455c-9d1a-fe4ea4a826fb", "70244693072", "(21) 99973-4775", "GigiMengão", new DateTime(2025, 5, 6, 22, 5, 31, 5, DateTimeKind.Unspecified).AddTicks(9960), null, "giovannaalemar@gmail.com", "GIOVANNAALEMAR@GMAIL.COM", "giovannaalemar@gmail.com", "GIOVANNAALEMAR@GMAIL.COM", true, "AQAAAAIAAYagAAAAEI0buEnwAgTfczsXxEejyitF95FGxHTocUPznRusmQh5bsq7NabJlK2H5RpUY65tbw==", "L4QPU3LFZKJY2T4RUNDOG6KMUAIY3LQJ", "3f33216a-bbb5-47c2-bac0-2a09950cc36a", null, false, false, null, true, 0 },
                    { "ce028b4a-156c-4714-ab13-11e0303d1c73", "70244693072", "(21) 99973-4731", "MarcellaMengo", new DateTime(2025, 5, 11, 14, 51, 29, 21, DateTimeKind.Unspecified).AddTicks(8296), null, "macelladealemar@gmail.com", "MACELLADEALEMAR@GMAIL.COM", "macelladealemar@gmail.com", "MACELLADEALEMAR@GMAIL.COM", true, "AQAAAAIAAYagAAAAEM7GS7Ya8YUGFRnU5pgVEGDdTMxrqV8/fl6JSgPm35qgENQYwKOZHuIOZUdAdQM9lw==", "4ULSMUTM4OVUWDLMU27DJ6LNECE3WY6Q", "9c51b05d-4083-4114-b016-e8cf4a898c16", null, false, false, null, true, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE Id IN ('07d59dc8-f02c-458e-be85-90c442f6c945', '155af3ee-c56b-495e-b5a1-f34e5d8e380f', '42aca2c4-ca07-48f9-992a-983ee5aa5344', '641d31f8-5b3c-40ec-a968-eb13c154d336', '725831c6-db84-463c-bc2e-ec2a86f69f4b', '846a9285-bf07-455c-9d1a-fe4ea4a826fb', 'ce028b4a-156c-4714-ab13-11e0303d1c73')");
        }
    }
}
