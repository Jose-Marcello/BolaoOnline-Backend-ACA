// Usings para componentes do ASP.NET Core
// Usings para seus projetos e namespaces específicos
using ApostasApp.Core.Application.MappingProfiles;
using ApostasApp.Core.Application.Services;
using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Application.Services.Interfaces.Email;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Infrastructure.Data.Context;
using ApostasApp.Core.Infrastructure.Identity.Seed;
using ApostasApp.Core.Infrastructure.Services;
using ApostasApp.Core.Infrastructure.Services.Email;
using ApostasApp.Core.Web.Configurations;
using ApostasApp.Core.Web.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURAÇÃO DE HEADERS DE PROXY ===
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
  options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
  options.KnownNetworks.Clear();
  options.KnownProxies.Clear();
});

// === CONFIGURAÇÃO DO DBCONTEXT (POSTGRESQL LOCAL) ===

// LER A CONNECTION STRING DIRETAMENTE DA CONFIGURAÇÃO 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// Use LogError para garantir que apareça nas logs do ACA, mesmo em produção
builder.Logging.AddConsole();
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogError($"VERIFICACAO CRITICA: Connection String Lida: {connectionString}"); // ESTA É A LINHA CHAVE

// 🛑 SOLUÇÃO PARA O ERRO NPGSQL/DATETIME 🛑
// Isso instrui o Npgsql a tratar DateTime sem TimeZone (Kind=Unspecified), evitando a exceção.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


// Injeção do DbContext
builder.Services.AddDbContext<MeuDbContext>(options =>
{
  // === MUDANÇA CRÍTICA: Trocando para UseNpgsql ===
  options.UseNpgsql(connectionString,
   npgsqlOptionsAction: sqlOptions =>
   {
     // Configura a retentativa padrão (Execution Strategy) para o PostgreSQL
     sqlOptions.EnableRetryOnFailure(
    maxRetryCount: 10,
    maxRetryDelay: TimeSpan.FromSeconds(30),
    errorCodesToAdd: null
  );
   })
   .LogTo(Console.WriteLine, LogLevel.Information);

});

/* --conflitando com JWT ? 
builder.Services.AddAuthentication()
  .AddBearerToken(IdentityConstants.BearerScheme, options =>
  {
    // Define o tempo de vida do Bearer Token para 3 horas
    options.BearerTokenExpiration = TimeSpan.FromHours(3);
  });
*/


builder.Services.AddHealthChecks();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
  // Define o tempo de vida padrão dos tokens para 3 horas
  options.TokenLifespan = TimeSpan.FromHours(3);
});

// Configuração do ASP.NET Core Identity
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
  options.SignIn.RequireConfirmedAccount = true;
  options.Password.RequiredLength = 6;
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
  options.Lockout.MaxFailedAccessAttempts = 5;
  options.Lockout.AllowedForNewUsers = true;
  options.User.RequireUniqueEmail = true;

})
.AddEntityFrameworkStores<MeuDbContext>()
.AddDefaultTokenProviders();


// === CORREÇÃO CRÍTICA FINAL: DATA PROTECTION EM MEMÓRIA ===
builder.Services.AddDataProtection();
// =========================================================


// === RESOLVE DEPENDENCIES ===
builder.Services.ResolveDependencies();
// ======================================

// 🛑 CORREÇÃO CRÍTICA JWT E PONTOS DE DEPURACÃO 🛑
// 1. Lê a chave da configuração (Jwt:SecretKey)
// Assumindo a estrutura: builder.Configuration.GetSection("Jwt").GetValue<string>("SecretKey")
string jwtSecretKey = builder.Configuration.GetSection("Jwt").GetValue<string>("SecretKey")?.Trim() ??
                     "LONGSUPERSECRETLONGSUPERSECRETSXXX";

// >>> BREAKPOINT 1: PARE AQUI (Linha ~105): Inspecione 'jwtSecretKey' (Valor LIDO)
logger.LogWarning($"[C# DEBUG] JWT Secret LIDO da Configuração (ANTES DO IF): {jwtSecretKey}");

// 2. Garante que a chave é válida e complexa
if (string.IsNullOrWhiteSpace(jwtSecretKey) || jwtSecretKey.Length < 16)
{
  // Se a chave lida for inválida, usa o fallback.
  jwtSecretKey = "LONGSUPERSECRETLONGSUPERSECRETSXXX";
  logger.LogWarning("Chave JWT ('Jwt:SecretKey') inválida ou muito curta. Usando fallback segura de 32 caracteres.");
}

// >>> BREAKPOINT 2: PARE AQUI (Linha ~114): Inspecione 'jwtSecretKey' (Valor FINAL)
logger.LogError($"[C# DEBUG] JWT Secret FINAL (APÓS O IF): {jwtSecretKey}");

var securityKey = Encoding.UTF8.GetBytes(jwtSecretKey);

// Configuração JWT Bearer Authentication
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.SaveToken = true;
  options.RequireHttpsMetadata = false;
  options.TokenValidationParameters = new TokenValidationParameters()
  {
    ClockSkew = TimeSpan.Zero,
    RequireExpirationTime = false,

    // 🛑 MUDANÇAS CRÍTICAS DE TESTE 🛑
    ValidateIssuer = false, // <-- MUDE PARA FALSE TEMPORARIAMENTE
    ValidateAudience = false, // <-- MUDE PARA FALSE TEMPORARIAMENTE
    ValidateLifetime = false, // Manter o tempo de vida
                              // 🛑 FIM DAS MUDANÇAS CRÍTICAS 🛑

    ValidateIssuerSigningKey = true,
    // Manter as linhas abaixo comentadas já que não são usadas com ValidateIssuer/Audience=false
    // ValidAudience = builder.Configuration["Jwt:Audience"],
    // ValidIssuer = builder.Configuration["Jwt:Issuer"],

    IssuerSigningKey = new SymmetricSecurityKey(securityKey),

    // ✅ ADICIONE ESTA LINHA:
    // Isso diz ao .NET: "O que vier como 'nameid' no JSON, trate como NameIdentifier"
    NameClaimType = "nameid"

  };
});


System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

builder.Services.Configure<PagSeguroSettings>(builder.Configuration.GetSection("PagSeguroSettings"));

builder.Services.AddHttpClient<IPagSeguroService, PagSeguroService>((serviceProvider, client) =>
{
  var pagSeguroSettings = serviceProvider.GetRequiredService<IOptions<PagSeguroSettings>>().Value;

  client.BaseAddress = new Uri("https://api.sandbox.pagseguro.com/charges");
  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pagSeguroSettings.Token);
});

// Outras injeções de serviços
// A linha abaixo está duplicada no código original, mas foi mantida por estar lá.
builder.Services.ResolveDependencies();

builder.Services.AddAutoMapper(cfg =>
{
  cfg.AddMaps(typeof(MappingProfile).Assembly);
});

// Configuração de Controladores, Swagger e CORS
builder.Services.AddControllers()
  .AddApplicationPart(typeof(AccountController).Assembly)
  .AddJsonOptions(options =>
  {
    // 🛑 CORREÇÃO FINAL 1: Força o Back-end a aceitar JSON em camelCase (padrão do Angular/Front-end)
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    // 🛑 ADICIONE ESTA LINHA PARA EVITAR O CICLO:
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

  });


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// CORS: Permitir acesso APENAS do Front-end SWA
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
 policy => policy.WithOrigins(
  "http://localhost:4200",
  "https://lemon-plant-05b6fdb1e.3.azurestaticapps.net",
  "https://app.palpitesbolao.com.br"
    )
  .AllowAnyHeader()
  .AllowAnyMethod()
  .AllowCredentials());
});

// Adicione isso antes de builder.Build() para forçar rotas minúsculas
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

var app = builder.Build();

// ===================================================================================================
// Pipeline de Requisições HTTP - Middleware
// ===================================================================================================


// 1. Primeiro habilita o uso de arquivos estáticos padrão
app.UseStaticFiles();

// 2. Se a pasta 'uploads' estiver fora da wwwroot ou precisar de mapeamento:
// (Opcional, mas garante que o .NET ache a pasta perfis)
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
  RequestPath = "/uploads"
});

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();

// 3. Autenticação e Autorização
app.UseAuthorization();

// 4. Endpoints Personalizados (Health Checks)
app.MapHealthChecks("/health");

// 5. Swagger (Interface)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
  // O nome aqui é apenas um rótulo, mas o caminho "/swagger/v1/swagger.json" deve ser o padrão
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bolão Online V1");

  // Deixe o RoutePrefix vazio se quiser que o Swagger abra na raiz da API 
  // ou mantenha "swagger" para acessar via /swagger
  options.RoutePrefix = "swagger";
});

// 6. Controllers (O Roteamento Final - Catch All)
app.MapControllers();

app.Run();
