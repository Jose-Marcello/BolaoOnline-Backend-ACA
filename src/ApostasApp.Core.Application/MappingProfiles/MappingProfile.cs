// ApostasApp.Core.Application.MappingProfiles/MappingProfile.cs
using AutoMapper;
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos;
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Usuarios;
using System.Text.Json.Serialization;

namespace ApostasApp.Core.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===================================================================================================
            // Mapeamentos para DTOs de Saída (Backend -> Frontend)
            // ===================================================================================================
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<Campeonato, CampeonatoDto>();
            CreateMap<Rodada, RodadaDto>();

            // Mapeamento para Saldo
            CreateMap<Saldo, SaldoDto>();

            // Mapeamento para ApostadorCampeonato
            CreateMap<ApostadorCampeonato, ApostadorCampeonatoDto>()
                .ForMember(dest => dest.CampeonatoId, opt => opt.MapFrom(src => src.CampeonatoId.ToString()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Apostador, opt => opt.MapFrom(src => src.Apostador))
                .ForMember(dest => dest.Campeonato, opt => opt.MapFrom(src => src.Campeonato));


            // Mapeamento para ApostadorDto
            CreateMap<Apostador, ApostadorDto>()
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.Saldo))
                .ForMember(dest => dest.CampeonatosAderidos, opt => opt.MapFrom(src => src.ApostadoresCampeonatos))
                // <<-- CORRIGIDO: Mapear Apelido da sua propriedade Apelido -->>
                .ForMember(dest => dest.Apelido, opt => opt.MapFrom(src => src.Usuario.Apelido))
                // <<-- CORRIGIDO: Mapear Email da propriedade Email herdada do IdentityUser -->>
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email)); // Ou src.Usuario.UserName se preferir

            // Mapeamento para ApostaJogoDto (DTO de saída para visualização na tela de apostas)
            CreateMap<Palpite, ApostaJogoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.IdJogo, opt => opt.MapFrom(src => src.JogoId.ToString()))
                .ForMember(dest => dest.PlacarMandante, opt => opt.MapFrom(src => src.PlacarApostaCasa.ToString()))
                .ForMember(dest => dest.PlacarVisitante, opt => opt.MapFrom(src => src.PlacarApostaVisita.ToString()))
                .ForMember(dest => dest.EquipeMandante, opt => opt.Ignore())
                .ForMember(dest => dest.SiglaMandante, opt => opt.Ignore())
                .ForMember(dest => dest.EscudoMandante, opt => opt.Ignore())
                .ForMember(dest => dest.EquipeVisitante, opt => opt.Ignore())
                .ForMember(dest => dest.SiglaVisitante, opt => opt.Ignore())
                .ForMember(dest => dest.EscudoVisitante, opt => opt.Ignore())
                .ForMember(dest => dest.DataJogo, opt => opt.Ignore())
                .ForMember(dest => dest.HoraJogo, opt => opt.Ignore())
                .ForMember(dest => dest.CampeonatoId, opt => opt.Ignore())
                .ForMember(dest => dest.RodadaId, opt => opt.Ignore())
                .ForMember(dest => dest.PlacarOficialCasa, opt => opt.Ignore())
                .ForMember(dest => dest.PlacarOficialVisitante, opt => opt.Ignore())
                .ForMember(dest => dest.StatusApostaJogo, opt => opt.Ignore())
                .ForMember(dest => dest.PontosGanhos, opt => opt.Ignore());

            // Mapeamento para ApostaRodadaStatusDto
            CreateMap<ApostaRodada, ApostaRodadaStatusDto>()
                .ForMember(dest => dest.StatusAposta, opt => opt.MapFrom(src => src.Enviada ? 2 : 1))
                .ForMember(dest => dest.DataAposta, opt => opt.MapFrom(src => src.DataHoraSubmissao))
                .ForMember(dest => dest.ApostaRodadaId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Enviada, opt => opt.MapFrom(src => src.Enviada))
                .ForMember(dest => dest.DataHoraSubmissao, opt => opt.MapFrom(src => src.DataHoraSubmissao));


            // ===================================================================================================
            // Mapeamentos para DTOs de Entrada (Frontend -> Backend)
            // ===================================================================================================

            // Mapeamento para SalvarPalpiteRequestDto (DTO de entrada para um item de aposta)
            CreateMap<SalvarPalpiteRequestDto, Palpite>()
                .ForMember(dest => dest.JogoId, opt => opt.MapFrom(src => Guid.Parse(src.JogoId)))
                .ForMember(dest => dest.PlacarApostaCasa, opt => opt.MapFrom(src => src.PlacarApostaCasa))
                .ForMember(dest => dest.PlacarApostaVisita, opt => opt.MapFrom(src => src.PlacarApostaVisita))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ApostaRodadaId, opt => opt.Ignore())
                .ForMember(dest => dest.Pontos, opt => opt.Ignore());

            // Mapeamento para SalvarApostaRequestDto (DTO de entrada para salvar a aposta completa da rodada)
            CreateMap<SalvarApostaRequestDto, ApostaRodada>()
                .ForMember(dest => dest.RodadaId, opt => opt.MapFrom(src => Guid.Parse(src.RodadaId)))
                .ForMember(dest => dest.ApostadorCampeonatoId, opt => opt.MapFrom(src => Guid.Parse(src.ApostadorCampeonatoId)))
                .ForMember(dest => dest.EhApostaCampeonato, opt => opt.MapFrom(src => src.EhCampeonato))
                .ForMember(dest => dest.IdentificadorAposta, opt => opt.MapFrom(src => Guid.Parse(src.IdentificadorAposta)))
                .ForMember(dest => dest.Palpites, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataHoraSubmissao, opt => opt.Ignore())
                .ForMember(dest => dest.Enviada, opt => opt.Ignore())
                .ForMember(dest => dest.PontuacaoTotalRodada, opt => opt.Ignore())
                .ForMember(dest => dest.EhApostaIsolada, opt => opt.Ignore());
        }
    }
}
