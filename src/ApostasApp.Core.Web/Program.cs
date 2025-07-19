// Localização: Program.cs (no projeto da API)

// Usings para componentes do ASP.NET Core
// Usings para seus projetos e namespaces específicos
using ApostasApp.Core.Application.MappingProfiles;
using ApostasApp.Core.Application.Services.Interfaces.Email;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Usuarios; // Para a classe Usuario do Identity
using ApostasApp.Core.Infrastructure.Identity.Seed;
using ApostasApp.Core.Infrastructure.Services.Email;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.InfraStructure.Data.Repository.Apostadores;
using ApostasApp.Web.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http; // Necessário para IHttpContextAccessor
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid;
using Swashbuckle.AspNetCore.SwaggerGen; // Necessário para AddSwaggerGen
using Swashbuckle.AspNetCore.SwaggerUI; // Necessário para UseSwaggerUI
using System; // Para TimeSpan, Guid, etc.
using System.Collections.Generic; // Para List
using System.Text; // Para Encoding
using Microsoft.AspNetCore.Identity.UI.Services; // Para a interface IEmailSender
using SendGrid; // Para ISendGridClient
using Microsoft.Extensions.Options; // Para IOptions<SendGridSettings>
using ApostasApp.Core.Domain.Models.Configuracoes; // Para SendGridSettings
using ApostasApp.Core.Infrastructure.Services.Email;
using IEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender; // Para SendGridEmailSender


var builder = WebApplication.CreateBuilder(args);


// ===================================================================================================
// Configuração do Banco de Dados Principal (ApostasAppDbContext)
// ===================================================================================================
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    options.SignIn.RequireConfirmedAccount = false; // true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
})
//.AddEntityFrameworkStores<IdentityDbContext>()
.AddEntityFrameworkStores<MeuDbContext>() // <<-- MUDANÇA CRÍTICA AQUI!
.AddDefaultTokenProviders();

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

builder.Services.ResolveDependencies();

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




// ===================================================================================================
// Configuração do AutoMapper
// ===================================================================================================
// Adiciona AutoMapper e busca o MappingProfile na assembly do projeto de Application
// Usando a sobrecarga que aceita uma Action para configuração, que é mais robusta.
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MappingProfile).Assembly); // MappingProfile está em ApostasApp.Core.Application
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // ADICIONE ESTA LINHA PARA TRATAR REFERÊNCIAS CIRCULARES
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        // Opcional: para melhor legibilidade no desenvolvimento
        options.JsonSerializerOptions.WriteIndented = true;
    });



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

app.UseCors("AllowSpecificOrigin"); // Usar a política CORS definida
//app.UseCors(); // Usar a política CORS definida

app.UseAuthentication(); // Deve vir antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
