using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AlteraParaDataAnulavelEInsereApostas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PASSO 1: Tornar DataHoraAposta anulável
            migrationBuilder.AlterColumn<DateTime>(
                name: "DataHoraAposta",
                table: "Apostas",
                type: "datetime2",
                nullable: true, // Agora permite NULL
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            // PASSO 2: Inserir dados na tabela Apostas
            // Certifique-se de que os NULLs estão corretos para as apostas não enviadas
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('D990BA1F-1FC3-4F91-B0B2-0080C0FC98AB', 'F2DDA750-7A82-48C8-1C13-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('D2336047-25F0-4B3B-9121-0243577C8BA5', '4AA7EBC4-3F83-4EE5-714A-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('7D0A23F0-0E05-4B66-8109-043D4B12B2BA', '2737C263-6C93-49D9-1C15-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('16F587E7-7344-4B38-8403-05C0EAE6A6B8', '5A5C1401-0F5F-4A4A-7146-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.1825314', 2, 1, 1, 12);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('B5E2DB53-A593-48BC-8AA8-0631636B00E3', 'F8156736-A15E-4F3E-1C11-08DD923C97F5', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('19A3F673-B6E9-4AFF-B97A-0721C2798648', 'A7565412-5BC0-4AEF-714E-08DD8CD6453F', 'FF670299-D4A2-439B-984D-E71366E1946B', '2025-05-11 14:29:15.6471353', 1, 0, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('221C6E49-0232-44A6-BF59-07E15D2BF8E9', 'F8156736-A15E-4F3E-1C11-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('1DC73B08-CEE1-4E2A-A1C6-081F2837ECA7', 'A7565412-5BC0-4AEF-714E-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('7F311BE9-06F4-42F9-BDA8-0C35429149A3', '56CE7704-CFD0-48FE-1C10-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('756606F9-92C7-46E1-93A1-0E99284451E1', 'B26CA879-0B3A-4F3A-7148-08DD8CD6453F', '92743816-E057-449C-8D62-F973F584918A', '2025-05-11 14:16:33.7837271', 2, 1, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('6CC3DBD5-6301-4AD8-8D98-114031CC451E', '2737C263-6C93-49D9-1C15-08DD923C97F5', 'FF670299-D4A2-439B-984D-E71366E1946B', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('DA763CD8-44E2-4851-AF49-126A7FA4295A', 'ACF97B53-631F-4B44-7149-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.1944075', 1, 0, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('54C6C012-33EB-4A02-AD16-180793183BAA', '005B5FD5-8BF6-4D63-1C16-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('E663528E-23B1-4C25-A03F-1852085D0909', '2975AB97-9FBB-4065-714D-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.2479597', 2, 2, 1, 12);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('A6CC4DF1-8D42-4B4C-BD2B-18DC74212DA9', '344BB27E-8029-4045-1C17-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('89765D76-8F68-4F35-B86D-1AF20756BD5D', '719E27C4-029C-4A3F-1C19-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('4CAD39AE-5E11-4CC9-BD7B-1DCC216D801C', '5A5C1401-0F5F-4A4A-7146-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('440F97FF-7449-4B37-A080-203647A4BD6F', '1234E72D-7D93-4144-1C18-08DD923C97F5', '92743816-E057-449C-8D62-F973F584918A', '2025-05-21 19:52:37.1110841', 1, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('AF5E113D-74D9-4E79-B36C-20C7BCFA4F17', 'B27533A2-C612-4435-714C-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.6806502', 2, 2, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('71550D34-CEAA-4824-9BE3-22CF1292D17D', '6A7E960B-7D32-4F5F-1C14-08DD923C97F5', 'FF670299-D4A2-439B-984D-E71366E1946B', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('38275973-A926-4737-A222-237FACA80ADD', 'DC76106A-2B70-4614-1C12-08DD923C97F5', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('2CF89A85-74F1-4962-9AFF-243B1879310B', '2737C263-6C93-49D9-1C15-08DD923C97F5', '92743816-E057-449C-8D62-F973F584918A', '2025-05-21 19:52:37.1053297', 1, 2, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('37B2F444-663E-439D-AAE4-274DD5758BCA', '5A5C1401-0F5F-4A4A-7146-08DD8CD6453F', '92743816-E057-449C-8D62-F973F584918A', '2025-05-11 14:16:33.7895467', 3, 2, 1, 3);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('06AAB8AE-77E2-415B-8879-28675516F08E', '4AA7EBC4-3F83-4EE5-714A-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.6661877', 2, 2, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('0799877B-5140-41E6-92D5-2A5EF50556EC', 'E433085A-FA83-4450-7147-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.6220394', 2, 1, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('8138A182-FAF6-4CA7-9580-2F9A380EBBF2', 'F2DDA750-7A82-48C8-1C13-08DD923C97F5', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('30C5D296-71B9-4970-B4A4-31406E184471', '005B5FD5-8BF6-4D63-1C16-08DD923C97F5', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('8AACD70A-1E0A-4AB7-9971-322254C21C4A', '719E27C4-029C-4A3F-1C19-08DD923C97F5', 'FF670299-D4A2-439B-984D-E71366E1946B', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('7E1ACB4C-338A-4769-BFC8-328A755F7EA2', '2737C263-6C93-49D9-1C15-08DD923C97F5', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('AC606664-945C-4BF7-BA2F-348E4523A818', '1234E72D-7D93-4144-1C18-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('00694AFA-B426-4545-84D0-3A18DB5E9559', 'B27533A2-C612-4435-714C-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.2393149', 2, 2, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('9F21B964-7283-45F8-B424-3C3A01A542C9', 'B26CA879-0B3A-4F3A-7148-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('3A7F7EDA-823D-482E-B4E8-3C4CD7125BB6', '83A1D7BF-F2D6-4B7A-7145-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.2026480', 3, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('430DA219-3645-42B6-A534-3FA670D17AFF', 'A4AD06FB-84EA-4256-714B-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.6960606', 0, 0, 1, 12);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('D439A0CC-2C55-4AC2-BBC4-416374308AEF', '56CE7704-CFD0-48FE-1C10-08DD923C97F5', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('108AF18B-3E24-41EC-8DF8-43C02BD6250F', '1234E72D-7D93-4144-1C18-08DD923C97F5', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('D16F32B9-CAEE-4E03-9A74-44D10ACD0BC9', '1234E72D-7D93-4144-1C18-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('04C1B43C-A6DF-41DC-9C97-456B67A70683', '56CE7704-CFD0-48FE-1C10-08DD923C97F5', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('35E38B65-E7F8-49B7-B330-4CD2FF46F953', 'F2DDA750-7A82-48C8-1C13-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('DD099E67-9522-4499-8410-4CD969E4161E', 'DC76106A-2B70-4614-1C12-08DD923C97F5', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('94C1FFC0-9531-46CD-BA01-4D45F3CCDAD8', '2737C263-6C93-49D9-1C15-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('0DF74E17-E9F6-45D8-90AD-50ED51CB0F10', '4AA7EBC4-3F83-4EE5-714A-08DD8CD6453F', 'FF670299-D4A2-439B-984D-E71366E1946B', '2025-05-11 14:29:15.6283722', 1, 0, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('B62DA8E9-6D54-4E16-BB0D-537C6DAD2D25', 'ACF97B53-631F-4B44-7149-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('DBA36493-B5B3-4427-856D-53A3928B76C3', 'B26CA879-0B3A-4F3A-7148-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.5609385', 1, 2, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('B6984F69-6BA9-4F70-A501-53C20A7D0CE6', 'B27533A2-C612-4435-714C-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.7053508', 1, 0, 1, 3);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('A0ECEE98-15E3-4455-B916-55918A8EC4DC', 'DC76106A-2B70-4614-1C12-08DD923C97F5', 'FF670299-D4A2-439B-984D-E71366E1946B', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('AB7F3725-B937-48D4-93C5-55EF441F7B95', 'A4AD06FB-84EA-4256-714B-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.2348174', 1, 1, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('DD502E4A-655A-495F-9439-59ED7C884039', '719E27C4-029C-4A3F-1C19-08DD923C97F5', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('941D048C-6A51-4394-B11A-5B7D4BD4BA79', '83A1D7BF-F2D6-4B7A-7145-08DD8CD6453F', '92743816-E057-449C-8D62-F973F584918A', '2025-05-11 14:16:33.8013733', 2, 0, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('84F9526C-7F18-40B4-A587-5CD163790599', '6A7E960B-7D32-4F5F-1C14-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('43707916-5A4C-4744-9849-5DFB6CEFBA1F', '2975AB97-9FBB-4065-714D-08DD8CD6453F', '92743816-E057-449C-8D62-F973F584918A', '2025-05-11 14:16:33.8551223', 1, 0, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('541B152F-ABC3-4107-ABFD-622D9B7CAEC3', 'A7565412-5BC0-4AEF-714E-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.6871941', 2, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('89AB1170-8D8E-4CD9-9B4B-63026BC8A6E0', '719E27C4-029C-4A3F-1C19-08DD923C97F5', '92743816-E057-449C-8D62-F973F584918A', '2025-05-21 19:52:37.1143278', 1, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('9DDFC590-F6F0-43D1-A797-64C1586BE52A', '2975AB97-9FBB-4065-714D-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.7023050', 1, 1, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('C15D36D4-FB02-4385-ACA9-65C197B40BC8', '83A1D7BF-F2D6-4B7A-7145-08DD8CD6453F', 'FF670299-D4A2-439B-984D-E71366E1946B', '2025-05-11 14:29:15.5959897', 4, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('53DA1F2C-1BF7-44E3-980F-68F8CE0F8F1A', 'ACF97B53-631F-4B44-7149-08DD8CD6453F', 'FF670299-D4A2-439B-984D-E71366E1946B', '2025-05-11 14:29:15.6030078', 1, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('9A930EFE-27A2-4E51-9CEB-69F63866CEAD', '005B5FD5-8BF6-4D63-1C16-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('D1CEBF56-1229-4242-90B2-6B9FF1B61DF7', '2975AB97-9FBB-4065-714D-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.7182498', 1, 1, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('3C2E5329-4746-431B-99BA-6EF6BCA6FDCE', '005B5FD5-8BF6-4D63-1C16-08DD923C97F5', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('519C31BD-4B47-490B-B418-6F9CE3E16685', '1234E72D-7D93-4144-1C18-08DD923C97F5', 'FF670299-D4A2-439B-984D-E71366E1946B', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('94D77A4B-383E-4940-91EB-7292EC2A6C4D', '719E27C4-029C-4A3F-1C19-08DD923C97F5', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('0D2EBDC0-1A8F-4268-A256-72EFBAEC5506', '344BB27E-8029-4045-1C17-08DD923C97F5', 'FF670299-D4A2-439B-984D-E71366E1946B', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('D972D4F4-6C7C-4758-B06A-75844AE472ED', 'E433085A-FA83-4450-7147-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('2CD3B461-1BD4-4A78-A505-75BAF70EA0AB', 'F8156736-A15E-4F3E-1C11-08DD923C97F5', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('CDB810D7-8155-42F8-A467-765C5C0A213A', '344BB27E-8029-4045-1C17-08DD923C97F5', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('3D907EEC-487B-43F5-9C40-7758BB41D9D7', 'E433085A-FA83-4450-7147-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.6323067', 2, 1, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('AA875837-8A92-429F-A032-798C29BBACF7', 'A7565412-5BC0-4AEF-714E-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.2436405', 2, 2, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('F1F197EF-CCB3-4EBF-874F-7C663C875ED4', '344BB27E-8029-4045-1C17-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('CD07ED3D-692B-4709-9E35-7FFF884DB5B3', '1234E72D-7D93-4144-1C18-08DD923C97F5', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('AD448828-2F51-4DF9-8DB4-826C5C1B4FDD', '2975AB97-9FBB-4065-714D-08DD8CD6453F', 'FF670299-D4A2-439B-984D-E71366E1946B', '2025-05-11 14:29:15.6659272', 2, 2, 1, 12);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('AE0D9AEC-9BA1-417F-9181-8ACDF6C668D6', 'A7565412-5BC0-4AEF-714E-08DD8CD6453F', '92743816-E057-449C-8D62-F973F584918A', '2025-05-11 14:16:33.8499240', 2, 2, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('6639967B-FB21-47B8-A393-8ADFC66A7CC1', '2975AB97-9FBB-4065-714D-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('6C1DD8C8-319F-4A7F-9C37-8B8D90B09FEA', '5A5C1401-0F5F-4A4A-7146-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.6382535', 2, 1, 1, 12);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('19BD716B-468E-4393-88F7-8C20CCA8E6CB', 'A7565412-5BC0-4AEF-714E-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.7122548', 2, 2, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('40E71D73-584A-4449-8FF5-8E2134EF0309', '719E27C4-029C-4A3F-1C19-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('BD99C4A1-BBF8-47A3-BA3D-904665B3EC9A', 'DC76106A-2B70-4614-1C12-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('8C07D3C0-3035-40C0-898F-908BD06A88A6', '83A1D7BF-F2D6-4B7A-7145-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.6455412', 1, 0, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('61E3E11B-A8B5-4A3C-B5D1-95214AC6BE0C', 'DC76106A-2B70-4614-1C12-08DD923C97F5', '92743816-E057-449C-8D62-F973F584918A', '2025-05-21 19:52:37.1004692', 3, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('BB5584D9-EB4C-4F61-AC1D-956AB02EF092', '344BB27E-8029-4045-1C17-08DD923C97F5', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('7CE90210-0B01-461A-996B-961D0AD37EEC', 'ACF97B53-631F-4B44-7149-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.6416053', 2, 0, 1, 12);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('01EA95FC-A0B3-4672-9BCC-9B0A7F737A38', '344BB27E-8029-4045-1C17-08DD923C97F5', '92743816-E057-449C-8D62-F973F584918A', '2025-05-21 19:52:37.1068428', 1, 3, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('EE01FFC0-5E06-40FC-8E41-9F981B76CFB4', 'B26CA879-0B3A-4F3A-7148-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.2138774', 1, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('2A5CD52B-A371-4A60-B7BA-A0C647CD0B46', '6A7E960B-7D32-4F5F-1C14-08DD923C97F5', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('FDF00476-6BC6-45AE-AF1D-A0E4544F7FAD', 'B27533A2-C612-4435-714C-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('E4632F37-89C0-4CF0-8660-A17BF4A20CFF', '005B5FD5-8BF6-4D63-1C16-08DD923C97F5', 'FF670299-D4A2-439B-984D-E71366E1946B', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('A4E5015F-37F5-4BAC-A8FD-A4CF5DBD5543', 'B27533A2-C612-4435-714C-08DD8CD6453F', '92743816-E057-449C-8D62-F973F584918A', '2025-05-11 14:16:33.8443792', 1, 0, 1, 3);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('C73ADF40-BBE3-4237-AA78-A4D832A06D83', '4AA7EBC4-3F83-4EE5-714A-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.2304458', 2, 0, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('A44BB124-F09D-4DC2-94F1-A6A37C7431DD', 'E433085A-FA83-4450-7147-08DD8CD6453F', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', '2025-05-11 14:32:42.2222857', 1, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('DB02E087-F98B-4491-BB34-AF9CC9924A38', '56CE7704-CFD0-48FE-1C10-08DD923C97F5', 'FF670299-D4A2-439B-984D-E71366E1946B', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('9357D875-7E01-4ABE-991F-B01A5F1F3898', 'A4AD06FB-84EA-4256-714B-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.6740307', 1, 0, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('7C502704-252F-41FE-B090-B11C45609F69', 'B26CA879-0B3A-4F3A-7148-08DD8CD6453F', 'FF670299-D4A2-439B-984D-E71366E1946B', '2025-05-11 14:29:15.6093218', 3, 1, 1, 3);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('4B84CB5F-CE85-4577-9EA0-B283A9B5CB68', 'E433085A-FA83-4450-7147-08DD8CD6453F', 'FF670299-D4A2-439B-984D-E71366E1946B', '2025-05-11 14:29:15.6159406', 1, 0, 1, 4);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('92C4438D-9507-4F25-B3A6-B2ABF45EA529', 'ACF97B53-631F-4B44-7149-08DD8CD6453F', '92743816-E057-449C-8D62-F973F584918A', '2025-05-11 14:16:33.8123170', 1, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('94A08B6A-BEFB-4FBC-9380-B2F7DC1B7529', '56CE7704-CFD0-48FE-1C10-08DD923C97F5', '6B62054D-0849-4337-AF9D-005FEAE6C7EE', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('CFD94B55-4760-4053-B832-B425BE5958EE', '83A1D7BF-F2D6-4B7A-7145-08DD8CD6453F', '0E684317-1FDE-473E-AED5-6783CA527B30', NULL, 0, 0, 0, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('EFBCBC27-4D32-4BED-92E9-B93C02F2614D', 'B27533A2-C612-4435-714C-08DD8CD6453F', 'FF670299-D4A2-439B-984D-E71366E1946B', '2025-05-11 14:29:15.6409320', 1, 0, 1, 3);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('A6C4CEFD-BF2F-447A-97DD-BDFFD16FE803', 'ACF97B53-631F-4B44-7149-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.6542940', 1, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('28625292-38CD-4622-A3FB-C1FDE8B17CC6', 'F2DDA750-7A82-48C8-1C13-08DD923C97F5', '92743816-E057-449C-8D62-F973F584918A', '2025-05-21 19:52:37.1021674', 1, 1, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('BA365FE1-7044-4896-AB38-C356485F8FD7', '83A1D7BF-F2D6-4B7A-7145-08DD8CD6453F', '70A84E1C-0A8B-47D2-B8CC-82B8BDBB250F', '2025-05-11 14:27:00.6506424', 1, 0, 1, 0);
            ");
            migrationBuilder.Sql(@"
                INSERT INTO Apostas (Id, JogoId, ApostadorCampeonatoId, DataHoraAposta, PlacarApostaCasa, PlacarApostaVisita, Enviada, Pontos) VALUES
                ('8DCE9CAC-4A6B-4906-ACBB-CC8EF17B9D7C', '4AA7EBC4-3F83-4EE5-714A-08DD8CD6453F', '0C6F2CBB-1CDF-42A8-8414-D2C8B04EBAD6', '2025-05-11 14:32:00.6706740', 2, 0, 1, 0);
            ");
            // ... (TODOS os seus 120 INSERTs com NULL ou datas válidas) ...
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // PASSO 1: Remover dados da tabela Apostas
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'D990BA1F-1FC3-4F91-B0B2-0080C0FC98AB';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'D2336047-25F0-4B3B-9121-0243577C8BA5';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '7D0A23F0-0E05-4B66-8109-043D4B12B2BA';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '16F587E7-7344-4B38-8403-05C0EAE6A6B8';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'B5E2DB53-A593-48BC-8AA8-0631636B00E3';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '19A3F673-B6E9-4AFF-B97A-0721C2798648';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '221C6E49-0232-44A6-BF59-07E15D2BF8E9';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '1DC73B08-CEE1-4E2A-A1C6-081F2837ECA7';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '7F311BE9-06F4-42F9-BDA8-0C35429149A3';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '756606F9-92C7-46E1-93A1-0E99284451E1';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '6CC3DBD5-6301-4AD8-8D98-114031CC451E';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'DA763CD8-44E2-4851-AF49-126A7FA4295A';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '54C6C012-33EB-4A02-AD16-180793183BAA';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'E663528E-23B1-4C25-A03F-1852085D0909';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'A6CC4DF1-8D42-4B4C-BD2B-18DC74212DA9';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '89765D76-8F68-4F35-B86D-1AF20756BD5D';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '4CAD39AE-5E11-4CC9-BD7B-1DCC216D801C';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '440F97FF-7449-4B37-A080-203647A4BD6F';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'AF5E113D-74D9-4E79-B36C-20C7BCFA4F17';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '71550D34-CEAA-4824-9BE3-22CF1292D17D';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '38275973-A926-4737-A222-237FACA80ADD';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '2CF89A85-74F1-4962-9AFF-243B1879310B';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '37B2F444-663E-439D-AAE4-274DD5758BCA';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '06AAB8AE-77E2-415B-8879-28675516F08E';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '0799877B-5140-41E6-92D5-2A5EF50556EC';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '8138A182-FAF6-4CA7-9580-2F9A380EBBF2';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '30C5D296-71B9-4970-B4A4-31406E184471';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '8AACD70A-1E0A-4AB7-9971-322254C21C4A';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '7E1ACB4C-338A-4769-BFC8-328A755F7EA2';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'AC606664-945C-4BF7-BA2F-348E4523A818';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '00694AFA-B426-4545-84D0-3A18DB5E9559';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '9F21B964-7283-45F8-B424-3C3A01A542C9';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '3A7F7EDA-823D-482E-B4E8-3C4CD7125BB6';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '430DA219-3645-42B6-A534-3FA670D17AFF';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'D439A0CC-2C55-4AC2-BBC4-416374308AEF';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '108AF18B-3E24-41EC-8DF8-43C02BD6250F';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'D16F32B9-CAEE-4E03-9A74-44D10ACD0BC9';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '04C1B43C-A6DF-41DC-9C97-456B67A70683';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '35E38B65-E7F8-49B7-B330-4CD2FF46F953';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'DD099E67-9522-4499-8410-4CD969E4161E';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '94C1FFC0-9531-46CD-BA01-4D45F3CCDAD8';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '0DF74E17-E9F6-45D8-90AD-50ED51CB0F10';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'B62DA8E9-6D54-4E16-BB0D-537C6DAD2D25';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'DBA36493-B5B3-4427-856D-53A3928B76C3';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'B6984F69-6BA9-4F70-A501-53C20A7D0CE6';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'A0ECEE98-15E3-4455-B916-55918A8EC4DC';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'AB7F3725-B937-48D4-93C5-55EF441F7B95';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'DD502E4A-655A-495F-9439-59ED7C884039';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '941D048C-6A51-4394-B11A-5B7D4BD4BA79';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '84F9526C-7F18-40B4-A587-5CD163790599';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '43707916-5A4C-4744-9849-5DFB6CEFBA1F';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '541B152F-ABC3-4107-ABFD-622D9B7CAEC3';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '89AB1170-8D8E-4CD9-9B4B-63026BC8A6E0';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '9DDFC590-F6F0-43D1-A797-64C1586BE52A';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'C15D36D4-FB02-4385-ACA9-65C197B40BC8';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '53DA1F2C-1BF7-44E3-980F-68F8CE0F8F1A';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '9A930EFE-27A2-4E51-9CEB-69F63866CEAD';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'D1CEBF56-1229-4242-90B2-6B9FF1B61DF7';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '3C2E5329-4746-431B-99BA-6EF6BCA6FDCE';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '519C31BD-4B47-490B-B418-6F9CE3E16685';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '94D77A4B-383E-4940-91EB-7292EC2A6C4D';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '0D2EBDC0-1A8F-4268-A256-72EFBAEC5506';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'D972D4F4-6C7C-4758-B06A-75844AE472ED';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '2CD3B461-1BD4-4A78-A505-75BAF70EA0AB';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'CDB810D7-8155-42F8-A467-765C5C0A213A';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '3D907EEC-487B-43F5-9C40-7758BB41D9D7';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'AA875837-8A92-429F-A032-798C29BBACF7';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'F1F197EF-CCB3-4EBF-874F-7C663C875ED4';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'CD07ED3D-692B-4709-9E35-7FFF884DB5B3';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'AD448828-2F51-4DF9-8DB4-826C5C1B4FDD';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'AE0D9AEC-9BA1-417F-9181-8ACDF6C668D6';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '6639967B-FB21-47B8-A393-8ADFC66A7CC1';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '6C1DD8C8-319F-4A7F-9C37-8B8D90B09FEA';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '19BD716B-468E-4393-88F7-8C20CCA8E6CB';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '40E71D73-584A-4449-8FF5-8E2134EF0309';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'BD99C4A1-BBF8-47A3-BA3D-904665B3EC9A';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '8C07D3C0-3035-40C0-898F-908BD06A88A6';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '61E3E11B-A8B5-4A3C-B5D1-95214AC6BE0C';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'BB5584D9-EB4C-4F61-AC1D-956AB02EF092';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '7CE90210-0B01-461A-996B-961D0AD37EEC';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '01EA95FC-A0B3-4672-9BCC-9B0A7F737A38';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'EE01FFC0-5E06-40FC-8E41-9F981B76CFB4';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '2A5CD52B-A371-4A60-B7BA-A0C647CD0B46';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'FDF00476-6BC6-45AE-AF1D-A0E4544F7FAD';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'E4632F37-89C0-4CF0-8660-A17BF4A20CFF';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'A4E5015F-37F5-4BAC-A8FD-A4CF5DBD5543';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'C73ADF40-BBE3-4237-AA78-A4D832A06D83';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'A44BB124-F09D-4DC2-94F1-A6A37C7431DD';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'DB02E087-F98B-4491-BB34-AF9CC9924A38';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '9357D875-7E01-4ABE-991F-B01A5F1F3898';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '7C502704-252F-41FE-B090-B11C45609F69';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '4B84CB5F-CE85-4577-9EA0-B283A9B5CB68';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '92C4438D-9507-4F25-B3A6-B2ABF45EA529';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '94A08B6A-BEFB-4FBC-9380-B2F7DC1B7529';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'CFD94B55-4760-4053-B832-B425BE5958EE';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'EFBCBC27-4D32-4BED-92E9-B93C02F2614D';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'A6C4CEFD-BF2F-447A-97DD-BDFFD16FE803';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '28625292-38CD-4622-A3FB-C1FDE8B17CC6';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = 'BA365FE1-7044-4896-AB38-C356485F8FD7';");
            migrationBuilder.Sql(@"DELETE FROM Apostas WHERE Id = '8DCE9CAC-4A6B-4906-ACBB-CC8EF17B9D7C';");
            // ... (TODOS os seus 120 DELETEs) ...
        }
    }
}
