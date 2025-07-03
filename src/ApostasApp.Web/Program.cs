// Program.cs - Configuração e pipeline de requisições para a API ASP.NET Core

// Usings necessários para o projeto
using ApostasApp.Core.Application.MappingProfiles;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Infrastructure.Services.Email;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Web.Configurations;
using ApostasApp.Web.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder; // Necessário para WebApplication
using Microsoft.AspNetCore.Http; // Necessário para context.Response.WriteAsync
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; // Necessário para AddCors
using Microsoft.Extensions.Hosting; // Necessário para app.Environment.IsDevelopment()
using Microsoft.Extensions.Logging; // <-- Adicionado para o ILogger
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner de injeção de dependência

// ===================================================================================================
// Configuração do DbContext
// ===================================================================================================
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===================================================================================================
// Configuração do Identity
// ===================================================================================================
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<MeuDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<PortugueseIdentityErrorDescriber>();

// ===================================================================================================
// INJEÇÃO DE DEPENDÊNCIA JWT (ESSENCIAL PARA AUTENTICAÇÃO DE API)
// ===================================================================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        // As chaves "Jwt:Issuer", "Jwt:Audience" e "Jwt:SecretKey" vêm do appsettings.json
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
        ClockSkew = TimeSpan.Zero // Remove o tempo de tolerância para expiração do token
    };

    // Obter logger para TokenValidationParameters (pode ser usado diretamente do builder.Services)
    // Para logs aqui, vamos usar o Console.WriteLine ou injetar ILoggerFactory para criar um ILogger.
    // Para simplificar e garantir que apareça, usaremos Console.WriteLine temporariamente aqui.
    // Uma abordagem mais robusta seria usar IOptionsMonitor ou IOptionsSnapshot para acessar configurações
    // ou obter ILoggerFactory via service provider.

    // Logging de validação - estes são configurados no início da app.
    // Não podem usar o logger injetado diretamente aqui, mas os eventos abaixo podem.
    // Para visualização rápida, podemos usar Console.WriteLine, mas em produção, use ILogger.
    // Console.WriteLine($"[JWT_VALIDA] SecretKey para validação: '{builder.Configuration["Jwt:SecretKey"]}'");
    // Console.WriteLine($"[JWT_VALIDA] Issuer para validação: '{builder.Configuration["Jwt:Issuer"]}'");
    // Console.WriteLine($"[JWT_VALIDA] Audience para validação: '{builder.Configuration["Jwt:Audience"]}'");


    // Eventos para garantir que a API retorne 401/403 em vez de redirecionar
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
            var userEmail = context.Principal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            logger?.LogInformation($"[JWT_VALIDA_SUCESSO] Token validado com sucesso para o usuário: {userEmail ?? "N/A"}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
            logger?.LogWarning($"[JWT_VALIDA_FALHA] OnChallenge disparado. Status: {context.Response.StatusCode}. Motivo: {context.ErrorDescription}. Uri: {context.Request.Path}");
            context.HandleResponse(); // Impede o redirecionamento padrão
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"message\":\"Não autorizado - Token inválido ou ausente.\"}");
        },
        OnForbidden = context =>
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
            logger?.LogWarning($"[JWT_VALIDA_PROIBIDO] OnForbidden disparado. Status: {context.Response.StatusCode}. Uri: {context.Request.Path}");
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"message\":\"Acesso proibido.\"}");
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
            logger?.LogError(context.Exception, $"[JWT_AUTENTICACAO_FALHOU] Erro na autenticação JWT: {context.Exception.Message}. Uri: {context.Request.Path}");
            return Task.CompletedTask;
        }
    };
});

// ===================================================================================================

// ===================================================================================================
// Configuração do AutoMapper
// ===================================================================================================
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ===================================================================================================
// INJEÇÃO DE DEPENDÊNCIA DO EMAIL SENDER (SendGrid)
// ===================================================================================================
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddSingleton<ISendGridClient>(sp =>
{
    var sendGridSettings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SendGridSettings>>().Value;
    if (string.IsNullOrEmpty(sendGridSettings.ApiKey))
    {
        Console.WriteLine("AVISO: SendGrid API Key está vazia na SendGridSettings. Verifique appsettings.json.");
    }
    return new SendGridClient(sendGridSettings.ApiKey);
});
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();

// ===================================================================================================
// INJEÇÃO DE DEPENDÊNCIA DAS URLS DO FRONTEND (para links de confirmação/reset de senha)
// ===================================================================================================
builder.Services.Configure<FrontendUrlsSettings>(builder.Configuration.GetSection("FrontendUrls"));

// ===================================================================================================
// Chamada para seu método de extensão ResolveDependencies (registra seus serviços e repositórios)
// ===================================================================================================
builder.Services.ResolveDependencies();

// ===================================================================================================
// Configuração de Controllers, API Explorer e Swagger/OpenAPI
// ===================================================================================================

// Adicione serviços ao contêiner.
builder.Services.AddControllers()
    .AddJsonOptions(options => // Adiciona as opções de serialização JSON
    {
        // Esta linha é a correção para o erro de ciclo de objeto!
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        // Opcional: Se você quiser que o nome das propriedades no JSON seja em camelCase (padrão para JSON)
        // options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        // Opcional: Para formatação bonita (indentado) no JSON de saída (bom para desenvolvimento)
        // options.JsonSerializerOptions.WriteIndented = true;
    });


builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();

// Configuração do JWT Bearer Authentication para o Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApostasApp.Web", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // Ajustar a descrição para ser mais didática
        Description = "Insira **APENAS O TOKEN** aqui. O prefixo 'Bearer ' será adicionado automaticamente pelo Swagger.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer" // <<< ESTE 'Bearer' É CRÍTICO para o Swagger adicionar o prefixo.
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
});

// ===================================================================================================
// Configuração de CORS (Cross-Origin Resource Sharing)
// = ==================================================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("http://localhost:4200") // <<-- CORRIGIDO: Especificando a origem do frontend
                        .AllowAnyMethod() // Permite todos os métodos HTTP (GET, POST, PUT, DELETE, OPTIONS)
                        .AllowAnyHeader() // Permite todos os cabeçalhos HTTP
                        .AllowCredentials()); // <<-- MUITO IMPORTANTE: Permite credenciais (JWT, cookies)
});

// ===================================================================================================
// Construção da Aplicação e Configuração da Pipeline de Requisições (Middleware)
// ===================================================================================================
var app = builder.Build();

// Obter uma instância do logger para ser usada fora do escopo de serviços
var appLogger = app.Services.GetRequiredService<ILogger<Program>>();

// Configurações para ambiente de desenvolvimento (Swagger UI)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP para HTTPS (geralmente via HSTS)
app.UseHttpsRedirection();

// ===================================================================================================
// ORDEM DOS MIDDLEWARES É CRUCIAL PARA CORS E AUTENTICAÇÃO!
// ===================================================================================================
app.UseRouting(); // 1. Primeiro: Define como as requisições serão roteadas para os endpoints.

app.UseCors("CorsPolicy"); // 2. SEGUNDO: Aplica a política de CORS.
                           // ESTA LINHA DEVE VIR ANTES DE app.UseAuthentication() e app.UseAuthorization()
                           // para que as requisições OPTIONS (preflight) sejam respondidas corretamente.

app.UseAuthentication(); // 3. Terceiro: Tenta autenticar o usuário (lê o token JWT).
app.UseAuthorization();  // 4. Quarto: Verifica as permissões do usuário autenticado (através de [Authorize]).

app.MapControllers(); // 5. Último: Mapeia e executa o endpoint do controlador que corresponde à rota.

// Inicia a aplicação
app.Run();
