using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.Presentation.Configurations;
using ApostasApp.Core.Presentation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;

namespace ApostasApp.Core.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDataProtection()
            .PersistKeysToAzureBlobStorage(new Uri(builder.Configuration["AzureBlobStorage:DataProtectionBlobUri"]))
            .SetApplicationName("ApostasApp"); // Use um nome exclusivo para sua aplicação


            builder.Services.AddDbContext<MeuDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            /*
            builder.Services.AddDbContext<MeuDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Número máximo de tentativas
                maxRetryDelay: TimeSpan.FromSeconds(30), // Intervalo máximo entre tentativas
                errorNumbersToAdd: null); // Códigos de erro SQL específicos para retry (opcional)
            }));
            */


            builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
            {
                // Aqui você pode adicionar suas configurações de senha, lockout, etc.
                // Configurações de senha
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true; 
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 1;               

                options.SignIn.RequireConfirmedAccount = false; // Por enquanto, desabilitamos a confirmação de conta para facilitar o teste
            })
            .AddEntityFrameworkStores<MeuDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<PortugueseIdentityErrorDescriber>();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24); // Defina a duração desejada para os tokens
            });

            builder.Services.AddTransient<ISendGridClient>(services =>
            {
                var sendGridConfig = builder.Configuration.GetSection("SendGrid");
                var apiKey = sendGridConfig["ApiKey"];
                return new SendGridClient(apiKey);
            });

            builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));

            //veja configuracoes (profile)
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //builder.Services.AddControllersWithViews()
            //   .AddJsonOptions(x =>
            //      x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);


            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo limite de inatividade da sessão
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; // Tornar o cookie de sessão essencial para funcionalidades básicas
            });

            builder.Services.AddControllersWithViews()
               .AddJsonOptions(x =>
                     x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve)
               .AddJsonOptions(options =>
                  options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
            // Opcional: para usar camelCase
                                                                                                   //.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); 
               //.AddNewtonsoftJson(options =>
               //{
                //   options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                 //  options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // Use esta opção para Newtonsoft.Json
              // }); // Use esta opção para Newtonsoft.Json


            // DI
            builder.Services.ResolveDependencies();

            builder.Services.AddAuthorization();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // ADICIONE ESTA LINHA
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

                      
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Certifique-se de que esta linha está presente e na ordem correta


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                //endpoints.MapRazorPages();
                //

                app.MapControllerRoute(
                name: "EsqueciSenha",
                pattern: "Account/EsqueciSenha",
                defaults: new { controller = "Account", action = "EsqueciSenha" });

                app.MapControllerRoute(
                name: "PainelUsuario",
                pattern: "PainelUsuario",
                defaults: new { controller = "Account", action = "PainelUsuario" });

                app.MapControllerRoute(
                name: "Register",
                pattern: "Account/Register",
                defaults: new { controller = "Account", action = "Register" });

                app.MapControllerRoute(
                name: "Login",
                pattern: "Account/Login",
                defaults: new { controller = "Account", action = "Login" });

                app.MapControllerRoute(
                name: "RedefinirSenha",
                pattern: "Account/RedefinirSenha/{userId}/{token}", // Inclua os parâmetros esperados na URL
                defaults: new { controller = "Account", action = "RedefinirSenha" });


                endpoints.MapControllerRoute(
                name: "ListarRodadas",
                pattern: "ListarRodadas",
                defaults: new { controller = "RankingRodada", action = "ListarRodadasComRanking" });

                app.MapControllerRoute(
                name: "SalvarApostas",
                pattern: "ApostadorCampeonato/SalvarApostas",
                defaults: new { controller = "ApostadorCampeonato", action = "SalvarApostas" });

                endpoints.MapControllerRoute(
                name: "ApostadorCampeonato/ExibirInterfaceDaRodadaEmApostas",
                pattern: "ExibirInterfaceEmApostas/{Id}",
                defaults: new { controller = "ApostadorCampeonato", action = "ExibirInterfaceDaRodadaEmApostas" });

                endpoints.MapControllerRoute(
                name: "ExibirInterfaceResultados",
                pattern: "ApostadorCampeonato/ExibirInterfaceDaRodadaCorrente/{Id}",
                defaults: new { controller = "ApostadorCampeonato", action = "ExibirInterfaceDaRodadaCorrente" });

                endpoints.MapControllerRoute(
                name: "ListarApostasDoApostadorNaRodada",
                pattern: "ApostadorCampeonato/ListarApostasDoApostadorNaRodada/{apostadorCampeonatoId}/{rodadaId}", // Adicione o parâmetro {id} se necessário
                defaults: new { controller = "ApostadorCampeonato", action = "ListarApostasDoApostadorNaRodada" });

                endpoints.MapControllerRoute(
                name: "ListarApostasDoApostadorNaRodadaEmApostas",
                pattern: "ApostadorCampeonato/ListarApostasDoApostadorNaRodadaEmApostas/{id}", // Adicione o parâmetro {id} se necessário
                defaults: new { controller = "ApostadorCampeonato", action = "ListarApostasDoApostadorNaRodadaEmApostas" });
               
                /*app.MapControllerRoute(
                name: "BuscarApostasDoApostadorNaRodada",
                pattern: "BuscarApostasDoApostadorNaRodada/{apostadorCampeonatoId}/{rodadaId}",
                defaults: new { controller = "ApostadorController", action = "BuscarApostasDoApostadorNaRodadaSelecionada" });
*/
                app.MapControllerRoute(
                name: "Apostar",
                pattern: "ApostadorCampeonato/Apostar", // <-- Rota especfica para Apostar
                defaults: new { controller = "ApostadorCampeonato", action = "Apostar" });

                endpoints.MapControllerRoute(
                name: "ExibirRanking",
                pattern: "RankingRodada/ExibirRanking/{id}",
                defaults: new { controller = "RankingRodada", action = "ExibirRanking" });


                /*                
                endpoints.MapControllerRoute(
                name: "ListarJogosDaRodada",
                pattern: "RankingRodada/ListarJogosDaRodada/{id}",
                defaults: new { controller = "RankingRodada", action = "ListarJogosDaRodada" });*/


                endpoints.MapControllerRoute(
                name: "ListarCampeonatos",
                pattern: "ListarCampeonatos",
                defaults: new { controller = "Campeonato", action = "ListarCampeonatos" });


                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
           

            app.Run();
        }
    }
}
