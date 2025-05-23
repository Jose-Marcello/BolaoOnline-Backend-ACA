using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Ufs;
using ApostasApp.Core.Presentation.ViewModels;
using ApostasApp.Core.PresentationViewModels;
using AutoMapper;
using DApostasApp.Core.Domain.Models.RankingRodadas;


namespace ApostasApp.Core.Presentation.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            
            CreateMap<Campeonato, CampeonatoViewModel>().ReverseMap();
            CreateMap<Rodada, RodadaViewModel>().ReverseMap();
            CreateMap<Equipe, EquipeViewModel>().ReverseMap();
            CreateMap<EquipeCampeonato, EquipeCampeonatoViewModel>().ReverseMap();
            CreateMap<ApostadorCampeonato, ApostadorCampeonatoViewModel>().ReverseMap();
            //CreateMap<ApostadorCampeonato, ApostasPorRodadaEApostadorViewModel>().ReverseMap();
            CreateMap<Uf, UfViewModel>().ReverseMap();
            CreateMap<Estadio, EstadioViewModel>().ReverseMap();
            CreateMap<Jogo, JogoViewModel>().ReverseMap();
            CreateMap<Apostador, ApostadorViewModel>().ReverseMap();
            CreateMap<Aposta, ApostaViewModel>().ReverseMap();
            CreateMap<Aposta, SalvarApostaViewModel>().ReverseMap();
            CreateMap<RankingRodada, RankingRodadaViewModel>().ReverseMap();
            
        }
    }
}