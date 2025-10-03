// Localização: Program.cs (no projeto da API)

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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;


var builder = WebApplication.CreateBuilder(args);


// ===================================================================================================
// Configurações de Serviços - Services
// ===================================================================================================

builder.Services.AddDbContext<MeuDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
      sqlServerOptionsAction: sqlOptions =>
      {
        sqlOptions.EnableRetryOnFailure(
              maxRetryCount: 10,
              maxRetryDelay: TimeSpan.FromSeconds(30),
              errorNumbersToAdd: null);
      })
      .LogTo(Console.WriteLine, LogLevel.Information)
      .EnableSensitiveDataLogging()
      .LogTo(Console.WriteLine, LogLevel.Information);

});


// Configuração do ASP.NET Core Identity
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

})
.AddEntityFrameworkStores<MeuDbContext>()
.AddDefaultTokenProviders();


// Configuração JWT Bearer Authentication
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


System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

builder.Services.Configure<PagSeguroSettings>(builder.Configuration.GetSection("PagSeguroSettings"));

builder.Services.AddHttpClient<IPagSeguroService, PagSeguroService>((serviceProvider, client) =>
{
  var pagSeguroSettings = serviceProvider.GetRequiredService<IOptions<PagSeguroSettings>>().Value;

  client.BaseAddress = new Uri("https://api.sandbox.pagseguro.com/charges");
  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pagSeguroSettings.Token);
});

builder.Services.ResolveDependencies();


var emailSimulationMode = builder.Configuration.GetValue<bool>("EmailSettings:EmailSimulationMode");

if (emailSimulationMode)
{
  builder.Services.AddTransient<IBolaoEmailSender, MockEmailSender>();
}
else
{
  builder.Services.AddTransient<IBolaoEmailSender, SmtpEmailSender>();
}

builder.Services.AddAutoMapper(cfg =>
{
  cfg.AddMaps(typeof(MappingProfile).Assembly);
});

if (builder.Environment.IsDevelopment())
{
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
  builder.Services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      });
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApostasApp API", Version = "v1" });

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


// Configuração CORS - Agora fora dos blocos de desenvolvimento
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowSpecificOrigin",
      policy => policy.WithOrigins(builder.Configuration["FrontendUrls:BaseUrl"])
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials());
});

var app = builder.Build();

// ===================================================================================================
// Pipeline de Requisições HTTP - Middleware
// ===================================================================================================


/*
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
*/

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// <<-- CORREÇÃO FINAL DA ORDEM -->>
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();


// Serve arquivos estáticos da wwwroot e de outros diretórios
app.UseStaticFiles();

// Se o seu frontend for um SPA, essa configuração é crucial.
app.MapWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
  // Usa arquivos estáticos de dentro do "wwwroot"
  appBuilder.UseStaticFiles();
  appBuilder.Run(async context =>
  {
    context.Response.ContentType = "text/html";
    // Serve o index.html como fallback para todas as rotas do Angular
    await context.Response.SendFileAsync(
        Path.Combine(app.Environment.WebRootPath, "index.html")
    );
  });
});
// <<-- FIM DA SEÇÃO CORRIGIDA E CONSOLIDADA -->>


// As requisições são mapeadas para os controladores
app.MapControllers();

app.Run();
