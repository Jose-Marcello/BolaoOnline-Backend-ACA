// Localização: Program.cs (no projeto da API
// Usings para componentes do ASP.NET Core
// Usings para seus projetos e namespaces específicos
using ApostasApp.Core.Application.MappingProfiles;
using ApostasApp.Core.Application.Services;
using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Usuarios; // Para a classe Usuario do Identity
using ApostasApp.Core.Infrastructure.Identity.Seed;
using ApostasApp.Core.Infrastructure.Services;
using ApostasApp.Core.Infrastructure.Data.Context;
//using ApostasApp.Web.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens; // Para SendGridEmailSender
using Microsoft.OpenApi.Models;
//using SendGrid; // Para ISendGridClient
using Swashbuckle.AspNetCore.SwaggerGen; // Necessário para AddSwaggerGen
using Swashbuckle.AspNetCore.SwaggerUI; // Necessário para UseSwaggerUI
using System; // Para TimeSpan, Guid, etc.
using System.Collections.Generic; // Para List
using System.Net.Http.Headers;
using System.Text; // Para Encoding
using System.Text.Json;
using ApostasApp.Core.Application.Services.Interfaces.Email;
using ApostasApp.Core.Infrastructure.Services.Email;
using ApostasApp.Core.Web.Configurations;


var builder = WebApplication.CreateBuilder(args);


// ===================================================================================================
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .LogTo(Console.WriteLine, LogLevel.Information) // Adicionado para ver o SQL no console
           .EnableSensitiveDataLogging()); // Adicionado para ver os valores dos parâmetros


// ===================================================================================================
// Configuração do Banco de Dados de Identidade (IdentityDbContext)
// ===================================================================================================
//builder.Services.AddDbContext<IdentityDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

// ===================================================================================================
// Configuração do ASP.NET Core Identity
// ===================================================================================================

builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
    //options.Stores.ProtectPersonalData = false; //provisório

})
.AddEntityFrameworkStores<MeuDbContext>()
.AddDefaultTokenProviders(); // <- O único ponto e vírgula aqui

//builder.Services.AddDataProtection();
//builder.Services.AddScoped<IPersonalDataProtector, PersonalDataProtectorService>();


// ===================================================================================================
// Configuração JWT Bearer Authentication
// ===================================================================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Em produção, deve ser true
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

// Adicione esta linha!
System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;


//builder.Services.Configure<MercadoPagoSettings>(builder.Configuration.GetSection("MercadoPagoSettings"));

// 1. Configura o PagSeguroSettings
builder.Services.Configure<PagSeguroSettings>(builder.Configuration.GetSection("PagSeguroSettings"));

// 2. Registra o PagSeguroService e configura o HttpClient
builder.Services.AddHttpClient<IPagSeguroService, PagSeguroService>((serviceProvider, client) =>
{
    var pagSeguroSettings = serviceProvider.GetRequiredService<IOptions<PagSeguroSettings>>().Value;

    client.BaseAddress = new Uri("https://api.sandbox.pagseguro.com/charges");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pagSeguroSettings.Token);
});



builder.Services.ResolveDependencies();

// ===================================================================================================
// Lógica para Injeção Condicional do Serviço de E-mail
// ===================================================================================================
var emailSimulationMode = builder.Configuration.GetValue<bool>("EmailSettings:EmailSimulationMode");

if (emailSimulationMode)
{
    // Se a flag for TRUE, registre o serviço de simulação
    builder.Services.AddTransient<IBolaoEmailSender, MockEmailSender>();
}
else
{
    // Se a flag for FALSE, use o seu serviço SMTP real (Mailtrap ou SendGrid)
    builder.Services.AddTransient<IBolaoEmailSender, SmtpEmailSender>();
}

/*

// <<-- NOVO: CONFIGURAÇÃO E REGISTRO DO SENDGRID E EMAILSENDER AQUI -->>
// Configurações do SendGrid (lê do appsettings.json)
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));

// Registra o cliente SendGrid
builder.Services.AddTransient<ISendGridClient>(s =>
{
    var apiKey = s.GetRequiredService<IOptions<SendGridSettings>>().Value.ApiKey;
    return new SendGridClient(apiKey);
});

// Registra sua implementação de IEmailSender (usando a interface padrão do Identity UI)
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();
*/


// ===================================================================================================
// Configuração do AutoMapper
// ===================================================================================================
// Adiciona AutoMapper e busca o MappingProfile na assembly do projeto de Application
// Usando a sobrecarga que aceita uma Action para configuração, que é mais robusta.
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MappingProfile).Assembly); // MappingProfile está em ApostasApp.Core.Application
});




if (builder.Environment.IsDevelopment())
{
    // Adicione os controllers da sua aplicação E O TestController em uma só linha
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        })
        .AddApplicationPart(typeof(ApostasApp.Core.Web.Controllers.TestController).Assembly);
}
else
{
    // Apenas os controllers de produção
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
}



// ===================================================================================================
// Configuração do Swagger/OpenAPI
// ===================================================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApostasApp API", Version = "v1" });

    // Configuração para JWT Bearer Authentication no Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


// ===================================================================================================
// Configuração CORS
// ===================================================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy.WithOrigins("http://localhost:4200") // Substitua pelo seu frontend Angular
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()); // Permitir cookies e credenciais
});

var app = builder.Build();

// ===================================================================================================
// Pipeline de Requisições HTTP
// ===================================================================================================

// Seed de dados de identidade (usuários e roles)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await IdentitySeed.SeedRolesAsync(roleManager);
        await IdentitySeed.SeedAdminUserAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao popular o banco de dados de identidade.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
// AQUI: O UseCors deve vir AGORA, depois de UseRouting
app.UseCors("AllowSpecificOrigin");

// Os middlewares de autenticação e autorização vêm em seguida
app.UseAuthentication();
app.UseAuthorization();

// As requisições são mapeadas para os controladores
app.MapControllers();

// Apenas para garantir que outros arquivos estáticos sejam servidos corretamente
app.UseStaticFiles();

app.UseRouting();

// Adicione esta linha para garantir que todas as rotas de frontend (Angular)
// sejam direcionadas para o index.html.
app.MapFallbackToFile("index.html");

app.Run();
