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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization; // Corrigido para System.Text.Json.Serialization
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json; // Mantenha este import para System.Text.Json
using System.Globalization; // Adicionado para parsing da Connection String do Heroku
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// === CONFIGURAÇÃO DE HEADERS DE PROXY ===
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
  options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
  options.KnownNetworks.Clear();
  options.KnownProxies.Clear();
});

// === CONFIGURAÇÃO DA CONNECTION STRING PARA POSTGRESQL (SUPORTE HEROKU) ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// LÓGICA CRÍTICA: Se estiver no Heroku, a Connection String é injetada como URL e precisa ser convertida.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
  // Converte a URL do Heroku (postgres://user:pass@host:port/db) para a string de conexão padrão do Npgsql
  var uri = new Uri(databaseUrl);
  var userInfo = uri.UserInfo.Split(':');

  // Adicionado SslMode=Require (mais seguro) ou mantido Prefer
  connectionString = $"Host={uri.Host};Port={uri.Port};Username={userInfo[0]};Password={userInfo[1]};Database={uri.LocalPath.Substring(1)};Pooling=true;SSL Mode=Prefer;TrustServerCertificate=true";
}

Console.WriteLine($"DEBUG: ConnectionString = {connectionString}");
// ===========================================

// ===================================================================================================
// Configurações de Serviços - Services
// ===================================================================================================

// === DBContext E IDENTITY - MIGRADO PARA NPGSQL (POSTGRESQL) ===
builder.Services.AddDbContext<MeuDbContext>(options =>
{
  options.UseNpgsql(connectionString,
    npgsqlOptionsAction: sqlOptions =>
    {
      sqlOptions.EnableRetryOnFailure(
      maxRetryCount: 10,
      maxRetryDelay: TimeSpan.FromSeconds(30),
      errorCodesToAdd: new string[0]

 );
    })
    .LogTo(Console.WriteLine, LogLevel.Information);
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

// === RESOLVE DEPENDENCIES ===
builder.Services.ResolveDependencies();
// ======================================


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
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidAudience = builder.Configuration["Jwt:Audience"],
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
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
builder.Services.AddControllers().AddJsonOptions(options =>
{
  // 🛑 CORREÇÃO FINAL: Força o Back-end a aceitar JSON em camelCase (padrão do Angular/Front-end)
  options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  // Mantém a correção de tipagem que já estava presente
  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: Permitir acesso APENAS do Front-end SWA (usando a URL de teste localhost e a futura URL do Azure Static Web Apps)
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
    policy => policy.WithOrigins(
      "http://localhost:4200",
      "https://thankful-pond-04be1170f.2.azurestaticapps.net",
      "https://app.palpitesbolao.com.br" // Adicione esta linha
       )
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
});

var app = builder.Build();

// ===================================================================================================
// INÍCIO: BLOCO DE MIGRAÇÃO AUTOMÁTICA DE BANCO DE DADOS (EF CORE)
// Este bloco garante que as migrações sejam aplicadas na inicialização, de forma idempotente e segura.
// ===================================================================================================
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  try
  {
    // Encontra o DbContext e força a aplicação das migrações pendentes
    var db = services.GetRequiredService<MeuDbContext>();
    db.Database.Migrate();

    // Opcional: Aqui você pode rodar seeds de dados, se tiver algum.
    // Por exemplo: await SeedIdentity.SeedAsync(userManager, roleManager);
  }
  catch (Exception ex)
  {
    // Se a migração falhar (por exemplo, problema de conexão com o DB), loga o erro e o app continuará
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Ocorreu um erro ao tentar aplicar as migrações do banco de dados.");
  }
}
// ===================================================================================================
// FIM: BLOCO DE MIGRAÇÃO AUTOMÁTICA
// ===================================================================================================


// ===================================================================================================
// Pipeline de Requisições HTTP - Middleware
// ===================================================================================================

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseForwardedHeaders();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// As requisições são mapeadas para os controladores
app.MapControllers();

// ===================================================================================================
// INICIALIZAÇÃO DA APLICAÇÃO (SUPORTE HEROKU/AMBIENTE)
// ===================================================================================================

// LÓGICA CRÍTICA: Usa a porta injetada pelo Heroku ($PORT) ou o padrão 8080/80
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://0.0.0.0:{port}";
app.Run(url);
