using ApostasApp.Core.Application.MappingProfiles;
using ApostasApp.Core.Domain.Models.Configuracoes; // <<<--- ADICIONE ESTE USING
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Infrastructure.Services.Email;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Web.Configurations;
using ApostasApp.Web.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.

// Configuração do DbContext
builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Identity
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

// Configuração do AutoMapper
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
        Console.WriteLine("WARNING: SendGrid API Key está vazia na SendGridSettings. Verifique appsettings.json.");
    }
    return new SendGridClient(sendGridSettings.ApiKey);
});
builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();

// ===================================================================================================

// ===================================================================================================
// INJEÇÃO DE DEPENDÊNCIA DAS URLS DO FRONTEND
// ===================================================================================================

// Registra as configurações das URLs do Frontend da seção "FrontendUrls" para a classe FrontendUrlsSettings
builder.Services.Configure<FrontendUrlsSettings>(builder.Configuration.GetSection("FrontendUrls")); // <<<--- NOVA LINHA

// ===================================================================================================

// Chamada para seu método de extensão ResolveDependencies
builder.Services.ResolveDependencies();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
