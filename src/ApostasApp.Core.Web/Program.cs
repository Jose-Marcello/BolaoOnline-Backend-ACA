// Usings para componentes do ASP.NET Core
// Usings para seus projetos e namespaces específicos
using ApostasApp.Core.Application.MappingProfiles;
using ApostasApp.Core.Application.Services;
using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Infrastructure.Identity.Seed;
using ApostasApp.Core.Infrastructure.Services;
using ApostasApp.Core.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
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
using System.Reflection;


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
