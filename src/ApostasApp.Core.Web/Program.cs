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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services; 
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging; // Adicionado para ILogger no bloco de migração
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization; // Adicionado para parsing da Connection String do Heroku
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json; // Mantenha este import para System.Text.Json
using System.Text.Json.Serialization; // Corrigido para System.Text.Json.Serialization
using Npgsql.EntityFrameworkCore.PostgreSQL; // Adicionado para o PostgreSQL!
using Microsoft.AspNetCore.DataProtection;

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

/*
if (string.IsNullOrEmpty(connectionString))
{
    // Se a string não for encontrada (ex: no ACA sem Segredo), esta exceção ocorre.
    throw new InvalidOperationException("A Connection String 'DefaultConnection' não foi encontrada. Verifique o appsettings.json ou os Segredos do Azure.");
}
*/

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
              errorCodesToAdd: null // null usa o conjunto padrão de erros transientes do PostgreSQL
          );

        // Remova a lógica de CockroachDB/Npgsql, pois não é necessária
        // (MinBatchSize, ExecutionStrategy manual, etc.)
      })
      .LogTo(Console.WriteLine, LogLevel.Information);

});


builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme, options =>
    {
      // Define o tempo de vida do Bearer Token para 3 horas
      options.BearerTokenExpiration = TimeSpan.FromHours(3);
    });

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
// Resolve o erro 'Storing keys in a directory... that may not be persisted' no Azure Container Apps (ACA).
// Esta linha é NECESSÁRIA para o Identity funcionar em um ambiente contêinerizado sem volume persistente.


builder.Services.AddDataProtection();
  //.PersistKeysToMemory();

// =========================================================



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
builder.Services.AddControllers()
    // 🎯 CORREÇÃO CRÍTICA FINAL: Força a descoberta de Controllers no Assembly que contém a AccountController
    .AddApplicationPart(typeof(AccountController).Assembly)
    .AddJsonOptions(options =>
    {
      // 🛑 CORREÇÃO FINAL 1: Força o Back-end a aceitar JSON em camelCase (padrão do Angular/Front-end)
      options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
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
/*
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
*/

// ===================================================================================================
// Pipeline de Requisições HTTP - Middleware
// ===================================================================================================

// 1. CORS (Deve vir logo após UseRouting)
app.UseCors("AllowFrontend"); // Certifique-se que você usou "AllowFrontend" ou "CorsPolicy" no AddCors


// 2. Configurações de Roteamento (Deve vir antes de tudo que tem rotas)
app.UseRouting();


// 3. Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

// 4. Endpoints Personalizados (Health Checks)
// Estes devem vir antes de MapControllers, que é o último catch-all.
app.MapHealthChecks("/health");

// 5. Swagger (Interface)
// O bloco UseSwagger/UseSwaggerUI DEVE vir aqui no pipeline.
// Nota: Certifique-se que UseSwagger() está ANTES de UseSwaggerUI().

app.UseSwagger(); // GERA o JSON (Definição da API)
app.UseSwaggerUI(options =>
{
  // Usa o JSON gerado acima
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "Banco de Itens V1");
  options.RoutePrefix = "swagger"; // ou string.Empty
});

// 6. Controllers (O Roteamento Final - Catch All)
app.MapControllers();

app.Run();
