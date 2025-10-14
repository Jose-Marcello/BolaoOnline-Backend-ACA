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
using Microsoft.Extensions.FileProviders;
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


var builder = WebApplication.CreateBuilder(args);

// === LEITURA E DEBUG DA CONNECTION STRING ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"DEBUG: ConnectionString = {connectionString}");
// ===========================================


// ===================================================================================================
// Configurações de Serviços - Services
// ===================================================================================================

// === REATIVANDO DBContext E IDENTITY ===
builder.Services.AddDbContext<MeuDbContext>(options =>
{
  options.UseSqlServer(connectionString,
    sqlServerOptionsAction: sqlOptions =>
    {
      sqlOptions.EnableRetryOnFailure(
          maxRetryCount: 10,
          maxRetryDelay: TimeSpan.FromSeconds(30),
          errorNumbersToAdd: null);
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

// === RESOLVE DEPENDENCIES REATIVADO ===
builder.Services.ResolveDependencies();
// ======================================


// Configuração JWT Bearer Authentication (MANTIDA)
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
// builder.Services.ResolveDependencies(); 

builder.Services.AddAutoMapper(cfg =>
{
  cfg.AddMaps(typeof(MappingProfile).Assembly);
});

// Configuração de Controladores, Swagger e CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: Permitir acesso APENAS do Front-end SWA (usando a URL de teste localhost e a futura URL do Azure Static Web Apps)
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
    policy => policy.WithOrigins(
      "http://localhost:4200",
           "https://thankful-pond-04be1170f.2.azurestaticapps.net"
        )
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials());
});

var app = builder.Build();

// ===================================================================================================
// Pipeline de Requisições HTTP - Middleware
// ===================================================================================================

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
  ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();

// ROTAS DO FRONTEND REMOVIDAS
app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// As requisições são mapeadas para os controladores
app.MapControllers();

app.Run();
