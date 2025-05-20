using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApostasApp.Core.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAzure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CPF = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    Celular = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Apelido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campeonatos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataInic = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumRodadas = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campeonatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ufs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(40)", nullable: false),
                    Sigla = table.Column<string>(type: "varchar(2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ufs", x => x.Id);
                });

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


            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Apostadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apostadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apostadores_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rodadas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroRodada = table.Column<int>(type: "int", nullable: false),
                    DataInic = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumJogos = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rodadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rodadas_Campeonatos_CampeonatoId",
                        column: x => x.CampeonatoId,
                        principalTable: "Campeonatos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Equipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(40)", nullable: false),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Escudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    UfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipes_Ufs_UfId",
                        column: x => x.UfId,
                        principalTable: "Ufs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Estadios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(60)", nullable: false),
                    UfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estadios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estadios_Ufs_UfId",
                        column: x => x.UfId,
                        principalTable: "Ufs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApostadoresCampeonatos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pontuacao = table.Column<int>(type: "int", nullable: false),
                    Posicao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApostadoresCampeonatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApostadoresCampeonatos_Apostadores_ApostadorId",
                        column: x => x.ApostadorId,
                        principalTable: "Apostadores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApostadoresCampeonatos_Campeonatos_CampeonatoId",
                        column: x => x.CampeonatoId,
                        principalTable: "Campeonatos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EquipesCampeonatos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipesCampeonatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipesCampeonatos_Campeonatos_CampeonatoId",
                        column: x => x.CampeonatoId,
                        principalTable: "Campeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipesCampeonatos_Equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RankingRodadas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RodadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorCampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pontuacao = table.Column<int>(type: "int", nullable: false),
                    Posicao = table.Column<int>(type: "int", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingRodadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RankingRodadas_ApostadoresCampeonatos_ApostadorCampeonatoId",
                        column: x => x.ApostadorCampeonatoId,
                        principalTable: "ApostadoresCampeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RankingRodadas_Rodadas_RodadaId",
                        column: x => x.RodadaId,
                        principalTable: "Rodadas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Jogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RodadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataJogo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraJogo = table.Column<TimeSpan>(type: "time", nullable: false),
                    EstadioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipeCasaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipeVisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlacarCasa = table.Column<int>(type: "int", nullable: true),
                    PlacarVisita = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jogos_EquipesCampeonatos_EquipeCasaId",
                        column: x => x.EquipeCasaId,
                        principalTable: "EquipesCampeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jogos_EquipesCampeonatos_EquipeVisitanteId",
                        column: x => x.EquipeVisitanteId,
                        principalTable: "EquipesCampeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jogos_Estadios_EstadioId",
                        column: x => x.EstadioId,
                        principalTable: "Estadios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Jogos_Rodadas_RodadaId",
                        column: x => x.RodadaId,
                        principalTable: "Rodadas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Apostas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApostadorCampeonatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataHoraAposta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlacarApostaCasa = table.Column<int>(type: "int", nullable: false),
                    PlacarApostaVisita = table.Column<int>(type: "int", nullable: false),
                    Enviada = table.Column<bool>(type: "bit", nullable: false),
                    Pontos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apostas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apostas_ApostadoresCampeonatos_ApostadorCampeonatoId",
                        column: x => x.ApostadorCampeonatoId,
                        principalTable: "ApostadoresCampeonatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Apostas_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apostadores_UsuarioId",
                table: "Apostadores",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApostadoresCampeonatos_ApostadorId",
                table: "ApostadoresCampeonatos",
                column: "ApostadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ApostadoresCampeonatos_CampeonatoId",
                table: "ApostadoresCampeonatos",
                column: "CampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_ApostadorCampeonatoId",
                table: "Apostas",
                column: "ApostadorCampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Apostas_JogoId",
                table: "Apostas",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Equipes_UfId",
                table: "Equipes",
                column: "UfId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipesCampeonatos_CampeonatoId",
                table: "EquipesCampeonatos",
                column: "CampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipesCampeonatos_EquipeId",
                table: "EquipesCampeonatos",
                column: "EquipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Estadios_UfId",
                table: "Estadios",
                column: "UfId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_EquipeCasaId",
                table: "Jogos",
                column: "EquipeCasaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_EquipeVisitanteId",
                table: "Jogos",
                column: "EquipeVisitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_EstadioId",
                table: "Jogos",
                column: "EstadioId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_RodadaId",
                table: "Jogos",
                column: "RodadaId");

            migrationBuilder.CreateIndex(
                name: "IX_RankingRodadas_ApostadorCampeonatoId",
                table: "RankingRodadas",
                column: "ApostadorCampeonatoId");

            migrationBuilder.CreateIndex(
                name: "IX_RankingRodadas_RodadaId",
                table: "RankingRodadas",
                column: "RodadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rodadas_CampeonatoId",
                table: "Rodadas",
                column: "CampeonatoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apostas");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "RankingRodadas");

            migrationBuilder.DropTable(
                name: "Jogos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ApostadoresCampeonatos");

            migrationBuilder.DropTable(
                name: "EquipesCampeonatos");

            migrationBuilder.DropTable(
                name: "Estadios");

            migrationBuilder.DropTable(
                name: "Rodadas");

            migrationBuilder.DropTable(
                name: "Apostadores");

            migrationBuilder.DropTable(
                name: "Equipes");

            migrationBuilder.DropTable(
                name: "Campeonatos");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Ufs");
        }
    }
}
