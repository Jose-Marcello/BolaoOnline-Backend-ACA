using ApostasApp.Core.Application.Services;
using ApostasApp.Core.Application.Services.Apostadores;
using ApostasApp.Core.Application.Services.Apostas;
using ApostasApp.Core.Application.Services.Campeonatos;
using ApostasApp.Core.Application.Services.Financeiro;
using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Application.Services.Interfaces.Apostadores;
using ApostasApp.Core.Application.Services.Interfaces.Apostas;
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Email;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Application.Services.Interfaces.Palpites;
using ApostasApp.Core.Application.Services.Interfaces.RankingRodadas;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Application.Services.Interfaces.Usuarios;
using ApostasApp.Core.Application.Services.Palpites;
using ApostasApp.Core.Application.Services.Rodadas;
using ApostasApp.Core.Application.Services.Usuarios;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces.Identity;
using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces.RankingRodadas;
using ApostasApp.Core.Domain.Models.Configuracoes;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.Infrastructure.Data.Repository.Apostas;
using ApostasApp.Core.Infrastructure.Data.Repository.Jogos;
using ApostasApp.Core.Infrastructure.Identity;
using ApostasApp.Core.Infrastructure.Notificacoes;
using ApostasApp.Core.Infrastructure.Services.Email;
using ApostasApp.Core.InfraStructure.Data.Repository.Apostadores;
using ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos;
using ApostasApp.Core.InfraStructure.Data.Repository.Financeiro;
using ApostasApp.Core.InfraStructure.Data.Repository.Rodadas;
using ApostasApp.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SendGrid;


namespace ApostasApp.Web.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();            

            services.AddScoped<IIdentityService, IdentityService>();          

            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // <<-- CORREÇÃO: REGISTRO DO SERVIÇO DE E-MAIL SENDGRID EXISTENTE -->>
            // 
            
            //services.AddScoped<IMercadoPagoService, MercadoPagoService>();
            services.AddScoped<IPagSeguroService, PagSeguroService>();

            services.AddScoped<IApostadorRepository, ApostadorRepository>();
            services.AddScoped<IApostaRodadaRepository, ApostaRodadaRepository>();
            services.AddScoped<ITransacaoFinanceiraRepository, TransacaoFinanceiraRepository>();
            services.AddScoped<ISaldoRepository, SaldoRepository>();
            services.AddScoped<IJogoRepository, JogoRepository>();
            services.AddScoped<IPalpiteRepository, PalpiteRepository>();
            services.AddScoped<ICampeonatoRepository, CampeonatoRepository>();
            services.AddScoped<IRodadaRepository, RodadaRepository>();
            services.AddScoped<IApostadorCampeonatoRepository, ApostadorCampeonatoRepository>();
            services.AddScoped<IRankingRodadaRepository, RankingRodadaRepository>();

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IApostadorService, ApostadorService>();
            services.AddScoped<ICampeonatoService, CampeonatoService>();
            services.AddScoped<IFinanceiroService, FinanceiroService>();
            services.AddScoped<IRodadaService, RodadaService>();
            services.AddScoped<IApostadorCampeonatoService, ApostadorCampeonatoService>();
            services.AddScoped<IApostaRodadaService, ApostaRodadaService>();
            services.AddScoped<IRankingService, RankingService>();
            services.AddScoped<IPalpiteService, PalpiteService>();
            services.AddScoped<IRankingRodadaService, RankingRodadaService>();

         
           

            //services.AddScoped<IUrlHelper, UrlHelper>();             

            //services.AddScoped<IUfRepository, UfRepository>();


            //services.AddScoped<IEquipeRepository, EquipeRepository>(); 
            //services.AddScoped<IImageUploadService, ImageUploadService>();

            //services.AddScoped<IEquipeCampeonatoService, EquipeCampeonatoService>();
            //services.AddScoped<IEquipeService, EquipeService>();
            //services.AddScoped<IEstadioService, EstadioService>();
            //services.AddScoped<IUfService, UfService>();


            return services;
        }
    }
}