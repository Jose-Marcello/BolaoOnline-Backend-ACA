using ApostasApp.Application.Services.Rodadas;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Interfaces.RankingRodadas;
using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces.Estadios;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Models.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Domain.Services.Apostadores;
using ApostasApp.Core.Domain.Services.Apostas;
using ApostasApp.Core.Domain.Services.Jogos;
using ApostasApp.Core.Domain.Services.RankingRodadas;
using ApostasApp.Core.Domain.Services.Rodadas;
using ApostasApp.Core.Domains.Models.Interfaces.Apostas;
using ApostasApp.Core.Infrastructure;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Repository.Apostadores;
using ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos;
using ApostasApp.Core.InfraStructure.Services.Apostadores;
using ApostasApp.Core.InfraStructure.Services.Apostas;
using ApostasApp.Core.InfraStructure.Services.Campeonatos;
using ApostasApp.Core.InfraStructure.Services.Jogos;
using ApostasApp.Core.InfraStructure.Services.Usuarios;
using ApostasApp.Infrastructure.Data.Repository;

namespace ApostasApp.Core.Presentation.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {

            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IApostadorRepository, ApostadorRepository>();
            services.AddScoped<IJogoRepository, JogoRepository>();
            services.AddScoped<IApostaRepository, ApostaRepository>();
            services.AddScoped<IApostadorService, ApostadorService>();
            services.AddScoped<IApostadorCampeonatoRepository, ApostadorCampeonatoRepository>();
            services.AddScoped<IEstadioRepository, EstadioRepository>();  
            services.AddScoped<IApostadorCampeonatoService, ApostadorCampeonatoService>();
            services.AddScoped<ICampeonatoRepository, CampeonatoRepository>();
            services.AddScoped<IEquipeCampeonatoRepository, EquipeCampeonatoRepository>();
            services.AddScoped<IRodadaRepository, RodadaRepository>();
            services.AddScoped<IRankingRodadaRepository, RankingRodadaRepository>();
            services.AddScoped<IApostaService, ApostaService>();
            services.AddScoped<IRankingRodadaService, RankingRodadaService>();
            services.AddScoped<IJogoService, JogoService>();
            services.AddScoped<IRodadaService, RodadaService>();


            //services.AddScoped<IUrlHelper, UrlHelper>();             
           
            //services.AddScoped<IUfRepository, UfRepository>();
                      
           
            //services.AddScoped<IEquipeRepository, EquipeRepository>(); 
            //services.AddScoped<IImageUploadService, ImageUploadService>();
            //services.AddScoped<ICampeonatoService, CampeonatoService>();
            //services.AddScoped<IEquipeCampeonatoService, EquipeCampeonatoService>();
            //services.AddScoped<IEquipeService, EquipeService>();
            //services.AddScoped<IEstadioService, EstadioService>();
            //services.AddScoped<IUfService, UfService>();


            return services;
        }
    }
}