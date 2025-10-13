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
using Microsoft.Extensions.FileProviders; // NOVO USANDO
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

// === DEBUG: CONFIRMAÇÃO DA CONNECTION STRING ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"DEBUG: ConnectionString = {connectionString}");
// =================================================


// ===================================================================================================
// Configurações de Serviços - Services
// ===================================================================================================

// === BLOCOS DE CÓDIGO CRÍTICOS COMENTADOS TEMPORARIAMENTE PARA DEBUG DE INFRA ===
// builder.Services.AddDbContext<MeuDbContext>(options => { ... });
// builder.Services.AddIdentity<Usuario, IdentityRole>(options => { ... })...;
// builder.Services.ResolveDependencies(); // COMENTADO: Pode ter dependências de DB que falham o startup


// Configuração JWT Bearer Authentication (Mantida, pois pode ser essencial para outros serviços)
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


// Configurações de Pagamento e E-mail (Mantidas, se não bloquearem o startup)
builder.Services.Configure<PagSeguroSettings>(builder.Configuration.GetSection("PagSeguroSettings"));
builder.Services.AddHttpClient<IPagSeguroService, PagSeguroService>((serviceProvider, client) => { /* ... */ });
// ... outros serviços que não dependem do DB.


builder.Services.AddAutoMapper(cfg =>
{
  cfg.AddMaps(typeof(MappingProfile).Assembly);
});

// Configuração de Controladores, Swagger e CORS (Sem alteração)
builder.Services.AddControllers()
     .AddJsonOptions(options => { /* ... */ })
     .AddApplicationPart(typeof(ApostasApp.Core.Web.Controllers.TestController).Assembly); // Mantido o AddApplicationPart do TestController para garantir que a API básica carregue

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { /* ... */ });
builder.Services.AddCors(options => { /* ... */ });


var app = builder.Build();

// ===================================================================================================
// Pipeline de Requisições HTTP - Middleware
// ===================================================================================================

// === VERIFICAÇÃO DE PATH DE ARQUIVOS ESTÁTICOS (CORREÇÃO DE ENTREGA) ===
if (app.Environment.IsProduction())
{
  // NOVO: Força o uso do wwwroot para evitar erros de caminho no Linux
  app.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
}
// ======================================================================


if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseCors("AllowLocalhost");
}
else // Produção
{
  app.UseCors("AllowSameHost");
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

app.UseAuthentication();
app.UseAuthorization();

// As requisições são mapeadas para os controladores
app.MapControllers();

// Se o seu frontend for um SPA, essa configuração é crucial.
// Ele serve o index.html como fallback para todas as rotas do Angular
app.MapFallbackToFile("index.html");

app.Run();
