// MappingProfile.cs
// ... (seus usings existentes) ...

using ApostasApp.Application.DTOs.Jogos;
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos;
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.DTOs.Equipes;
using ApostasApp.Core.Application.DTOs.Estadios;
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.DTOs.Jogos;
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Usuarios;
using AutoMapper;

namespace ApostasApp.Core.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ... (Seus outros mapeamentos existentes) ...

            CreateMap<Notificacao, NotificationDto>();

            // Mapeamentos para a entidade Jogo e seus DTOs
            CreateMap<Jogo, JogoDetalheDto>();
            CreateMap<Jogo, JogoDto>();

            // =========================================================================================
            // Mapeamentos ATUALIZADOS para Rodada e seus DTOs
            // =========================================================================================
            CreateMap<Rodada, RodadaDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.NumeroRodada, opt => opt.MapFrom(src => src.NumeroRodada))
                .ForMember(dest => dest.NumJogos, opt => opt.MapFrom(src => src.NumJogos))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Campeonato, opt => opt.MapFrom(src => src.Campeonato));


            // =========================================================================================
            // Mapeamentos ATUALIZADOS para Campeonato e seus DTOs (CORREÇÃO AQUI!)
            // =========================================================================================
            
            CreateMap<Campeonato, CampeonatoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString())) // <<-- ESTA LINHA ESTÁ CORRETA SE DTO.Id FOR string!
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.DataInicio, opt => opt.MapFrom(src => src.DataInic))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim))
                .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Ativo))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString())); // Entidade Tipo (enum) -> DTO Status (string)


            // Mapeamentos para a entidade Equipe e seus DTOs
            CreateMap<Equipe, EquipeDto>();
            CreateMap<EquipeCampeonato, EquipeCampeonatoDto>();

            // Mapeamentos para a entidade Estadio e seus DTOs
            CreateMap<Estadio, EstadioDto>();

            // =========================================================================================
            // Mapeamentos ATUALIZADOS para Apostador e seus DTOs
            // =========================================================================================
            CreateMap<Apostador, ApostadorDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId.ToString()))
                .ForMember(dest => dest.Apelido, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Apelido : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.Saldo));

            // =========================================================================================
            // Mapeamentos ATUALIZADOS para Saldo e seus DTOs
            // =========================================================================================
            CreateMap<Saldo, SaldoDto>()
            .ForMember(dest => dest.ApostadorId, opt => opt.MapFrom(src => src.ApostadorId.ToString()));


            // Mapeamentos para a entidade ApostadorCampeonato e seus DTOs
            CreateMap<ApostadorCampeonato, ApostadorCampeonatoDto>();

            // Mapeamentos para a entidade Palpite e seus DTOs
            CreateMap<Palpite, PalpiteDto>();

            // Mapeamentos para a entidade Usuario e seus DTOs de autenticação/perfil
            CreateMap<RegisterRequestDto, Usuario>();
            CreateMap<Usuario, UsuarioProfileDto>();
        }
    }
}
