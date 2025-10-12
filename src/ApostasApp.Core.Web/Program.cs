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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// === DEBUG: CONFIRMAÇÃO DA CONNECTION STRING (NÃO REMOVER, É ÚTIL) ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"DEBUG: ConnectionString = {connectionString}");
// =================================================


// ===================================================================================================
// Configurações de Serviços - Services
// ===================================================================================================

builder.Services.AddDbContext<MeuDbContext>(options =>
{
  options.UseSqlServer(connectionString, // Usamos a variável local (connectionString)
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

  // Configuração CORS mais aberta para DEV
  builder.Services.AddCors(options =>
  {
    options.AddPolicy("AllowLocalhost",
      policy => policy.WithOrigins("http://localhost:4200") // Permite o ambiente de dev local
              .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
  });
}
else // Produção (Azure)
{
  builder.Services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      });

  // Configuração CORS especial para o cenário Docker/App Service
  builder.Services.AddCors(options =>
  {
    options.AddPolicy("AllowSameHost",
      policy => policy
  // Permite qualquer origem, o que é necessário para este cenário de host único
              .SetIsOriginAllowed(origin => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
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


var app = builder.Build();

// ===================================================================================================
// Pipeline de Requisições HTTP - Middleware
// ===================================================================================================

// === AQUI ESTAVA O BLOCO DE MIGRAÇÃO/SEED DE BANCO QUE PROVAVELMENTE TRAVA O SERVIDOR ===
// VAMOS ENVOLVER O STARTUP EM UM TRY/CATCH PARA FORÇAR O LOG DA EXCEÇÃO FATAL.
try
{
  // Tente rodar o bloco de inicialização do banco de dados (se existir)
  // ******************************************************************************
  // IMPORTANTE: SE VOCÊ SABE ONDE ESTÁ SEU CÓDIGO DE SEED/MIGRATE, COMENTE-O AQUI
  // ******************************************************************************

  // Se não houver código de migração aqui, ele apenas segue para o UseForwardedHeaders.

  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
    // Usa a política de localhost para desenvolvimento
    app.UseCors("AllowLocalhost");
  }
  else // Produção
  {
    // Usa a política de 'AllowSameHost' para o ambiente Azure App Service
    app.UseCors("AllowSameHost"); // <<-- ESSA LINHA É CRUCIAL PARA PRODUÇÃO
  }

  app.UseForwardedHeaders(new ForwardedHeadersOptions
  {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
  });
  app.UseHttpsRedirection();


  app.UseDefaultFiles();
  // Serve arquivos estáticos da wwwroot e de outros diretórios
  app.UseStaticFiles();

  app.UseRouting();

  // app.UseCors("AllowSpecificOrigins"); // <<-- REMOVIDO E MOVIDO PARA CIMA E DENTRO DO IF/ELSE

  app.UseAuthentication();
  app.UseAuthorization();

  // As requisições são mapeadas para os controladores
  app.MapControllers();

  // Se o seu frontend for um SPA, essa configuração é crucial.
  // Ele serve o index.html como fallback para todas as rotas do Angular
  app.MapFallbackToFile("index.html");

  app.Run();

}
catch (Exception ex)
{
  // LOG DE EMERGÊNCIA: Se a aplicação falhar, isso forçará a impressão da exceção completa
  Console.WriteLine($"FATAL EXCEPTION DURING STARTUP: {ex.Message}");
  Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
  // Se a exceção for Entity Framework, ela deve aparecer aqui.
}
