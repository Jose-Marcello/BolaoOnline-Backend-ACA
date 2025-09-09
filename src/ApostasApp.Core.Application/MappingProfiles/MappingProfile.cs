// Localização: ApostasApp.Core.Application.MappingProfiles/MappingProfile.cs
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos;
using ApostasApp.Core.Application.DTOs.Apostadores;
using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.DTOs.Conferencia;
using ApostasApp.Core.Application.DTOs.Equipes;
using ApostasApp.Core.Application.DTOs.Estadios;
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.DTOs.Jogos;
using ApostasApp.Core.Application.DTOs.Palpites;
using ApostasApp.Core.Application.DTOs.Ranking;
using ApostasApp.Core.Application.DTOs.RankingRodadas;
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Application.DTOs.Usuarios;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Relatorios;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Equipes;
using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.RankingRodadas;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Models.Usuarios;
using AutoMapper;
using System;

namespace ApostasApp.Core.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===================================================================================================
            // Mapeamentos para DTOs de Saída (Backend -> Frontend)
            // ===================================================================================================
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<Campeonato, CampeonatoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.CustoAdesao, opt => opt.MapFrom(src => src.CustoAdesao))
                .ForMember(dest => dest.AderidoPeloUsuario, opt => opt.Ignore());

            // Mapeamento para RodadaDto
            CreateMap<Rodada, RodadaDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.CampeonatoId, opt => opt.MapFrom(src => src.CampeonatoId.ToString()))
                .ForMember(dest => dest.DataInic, opt => opt.MapFrom(src => src.DataInic))
                .ForMember(dest => dest.DataFim, opt => opt.MapFrom(src => src.DataFim))
                .ForMember(dest => dest.CustoApostaRodada, opt => opt.MapFrom(src => src.CustoApostaRodada))
                .ForMember(dest => dest.Campeonato, opt => opt.MapFrom(src => src.Campeonato));

            // Mapeamento para Saldo
            CreateMap<Saldo, SaldoDto>();

            // Mapeamento para ApostadorCampeonato
            CreateMap<ApostadorCampeonato, ApostadorCampeonatoDto>()
                .ForMember(dest => dest.CampeonatoId, opt => opt.MapFrom(src => src.CampeonatoId.ToString()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Apostador, opt => opt.MapFrom(src => src.Apostador))
                .ForMember(dest => dest.Campeonato, opt => opt.MapFrom(src => src.Campeonato));

            // Mapeamento de ApostadorDto para Apostador
            CreateMap<ApostadorDto, Apostador>()
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());

            // Mapeamento para Saldo
            CreateMap<Saldo, SaldoDto>();
            CreateMap<SaldoDto, Saldo>();

            // Mapeamento para ApostadorDto
            CreateMap<Apostador, ApostadorDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId.ToString()))
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.Saldo))
                .ForMember(dest => dest.CampeonatosAderidos, opt => opt.MapFrom(src => src.ApostadoresCampeonatos))
                .ForMember(dest => dest.Apelido, opt => opt.MapFrom(src => src.Usuario.Apelido))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.Celular, opt => opt.MapFrom(src => src.Usuario.Celular))
                .ForMember(dest => dest.FotoPerfil, opt => opt.MapFrom(src => src.Usuario.FotoPerfil));

            // <<-- NOVOS MAPEAMENTOS PARA JOGO, EQUIPE E ESTADIO -->>
            CreateMap<Equipe, EquipeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<EquipeCampeonato, EquipeCampeonatoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.EquipeId, opt => opt.MapFrom(src => src.EquipeId.ToString()))
                .ForMember(dest => dest.CampeonatoId, opt => opt.MapFrom(src => src.CampeonatoId.ToString()))
                .ForMember(dest => dest.Equipe, opt => opt.MapFrom(src => src.Equipe));

            CreateMap<Estadio, EstadioDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            // <<-- CORREÇÃO AQUI: Mapeamento de Jogo para JogoDto -->>
            CreateMap<Jogo, JogoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.RodadaId, opt => opt.MapFrom(src => src.RodadaId.ToString()))
                .ForMember(dest => dest.EquipeCasaCampeonatoId, opt => opt.MapFrom(src => src.EquipeCasaId.ToString()))
                .ForMember(dest => dest.EquipeVisitanteCampeonatoId, opt => opt.MapFrom(src => src.EquipeVisitanteId.ToString()))
                .ForMember(dest => dest.EstadioId, opt => opt.MapFrom(src => src.EstadioId.ToString()))
                .ForMember(dest => dest.DataHora, opt => opt.MapFrom(src => src.DataJogo.Add(src.HoraJogo)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PlacarCasa, opt => opt.MapFrom(src => src.PlacarCasa))
                .ForMember(dest => dest.PlacarFora, opt => opt.MapFrom(src => src.PlacarVisita))
                .ForMember(dest => dest.EquipeCasaNome, opt => opt.MapFrom(src => src.EquipeCasa.Equipe.Nome))
                .ForMember(dest => dest.EquipeCasaEscudoUrl, opt => opt.MapFrom(src => src.EquipeCasa.Equipe.Escudo))
                .ForMember(dest => dest.EquipeVisitanteNome, opt => opt.MapFrom(src => src.EquipeVisitante.Equipe.Nome))
                .ForMember(dest => dest.EquipeVisitanteEscudoUrl, opt => opt.MapFrom(src => src.EquipeVisitante.Equipe.Escudo))
                .ForMember(dest => dest.EstadioNome, opt => opt.MapFrom(src => src.Estadio.Nome));

            // Mapeamento para PalpiteDto (agora incluindo o Jogo)
            CreateMap<Palpite, PalpiteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.JogoId, opt => opt.MapFrom(src => src.JogoId.ToString()))
                .ForMember(dest => dest.ApostaRodadaId, opt => opt.MapFrom(src => src.ApostaRodadaId.ToString()))
                .ForMember(dest => dest.PlacarApostaCasa, opt => opt.MapFrom(src => src.PlacarApostaCasa))
                .ForMember(dest => dest.PlacarApostaVisita, opt => opt.MapFrom(src => src.PlacarApostaVisita))
                .ForMember(dest => dest.Pontos, opt => opt.MapFrom(src => src.Pontos))
                .ForMember(dest => dest.Jogo, opt => opt.MapFrom(src => src.Jogo));

            // Mapeamento para ApostaRodadaDto
            CreateMap<ApostaRodada, ApostaRodadaDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.RodadaId, opt => opt.MapFrom(src => src.RodadaId.ToString()))
                .ForMember(dest => dest.ApostadorCampeonatoId, opt => opt.MapFrom(src => src.ApostadorCampeonatoId.ToString()))
                .ForMember(dest => dest.DataHoraSubmissao, opt => opt.MapFrom(src => src.DataHoraSubmissao.HasValue ? src.DataHoraSubmissao.Value.ToString("o") : null))
                .ForMember(dest => dest.Palpites, opt => opt.MapFrom(src => src.Palpites));

            // Mapeamento para ApostaRodadaStatusDto
            CreateMap<ApostaRodada, ApostaRodadaStatusDto>()
                .ForMember(dest => dest.StatusAposta, opt => opt.MapFrom(src => src.Enviada ? 2 : 1))
                .ForMember(dest => dest.DataAposta, opt => opt.MapFrom(src => src.DataHoraSubmissao.HasValue ? src.DataHoraSubmissao.Value.ToString("o") : null))
                .ForMember(dest => dest.ApostaRodadaId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.RodadaId, opt => opt.MapFrom(src => src.RodadaId.ToString()))
                .ForMember(dest => dest.ApostadorCampeonatoId, opt => opt.MapFrom(src => src.ApostadorCampeonatoId.ToString()))
                .ForMember(dest => dest.Enviada, opt => opt.MapFrom(src => src.Enviada));

            // ===================================================================================================
            // Mapeamentos para DTOs de Entrada (Frontend -> Backend)
            // ===================================================================================================

            // Mapeamento para SalvarPalpiteRequestDto
            CreateMap<SalvarPalpiteRequestDto, Palpite>()
                .ForMember(dest => dest.JogoId, opt => opt.MapFrom(src => Guid.Parse(src.JogoId)))
                .ForMember(dest => dest.PlacarApostaCasa, opt => opt.MapFrom(src => src.PlacarApostaCasa))
                .ForMember(dest => dest.PlacarApostaVisita, opt => opt.MapFrom(src => src.PlacarApostaVisita))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ApostaRodadaId, opt => opt.Ignore())
                .ForMember(dest => dest.Pontos, opt => opt.MapFrom(src => 0));

            // Mapeamento para SalvarApostaRequestDto
            CreateMap<SalvarApostaRequestDto, ApostaRodada>()
                .ForMember(dest => dest.RodadaId, opt => opt.MapFrom(src => Guid.Parse(src.RodadaId)))
                .ForMember(dest => dest.ApostadorCampeonatoId, opt => opt.MapFrom(src => Guid.Parse(src.ApostadorCampeonatoId)))
                .ForMember(dest => dest.EhApostaCampeonato, opt => opt.MapFrom(src => src.EhCampeonato))
                .ForMember(dest => dest.IdentificadorAposta, opt => opt.MapFrom(src => src.IdentificadorAposta))
                .ForMember(dest => dest.Palpites, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataHoraSubmissao, opt => opt.Ignore())
                .ForMember(dest => dest.Enviada, opt => opt.Ignore())
                .ForMember(dest => dest.PontuacaoTotalRodada, opt => opt.Ignore())
                .ForMember(dest => dest.EhApostaIsolada, opt => opt.MapFrom(src => src.ehApostaIsolada));

            // **CORREÇÃO FINAL AQUI:** Mapeamento de RankingRodada (Entidade) para RankingRodadaDto (DTO)
            // O mapeamento de Apelido e FotoPerfil está causando o erro porque o destino, RankingRodadaDto, não tem essas propriedades.
            // Para consertar, você precisa adicionar essas propriedades ao RankingRodadaDto ou mapear para um DTO que as contenha.
            // A solução mais simples é mapear para RankingDto, se ele já tem as propriedades. Se não, adicione as propriedades ao RankingRodadaDto.

            // O que você tinha no seu código:
            // CreateMap<RankingRodada, RankingRodadaDto>()
            //    .ForMember(dest => dest.ApostadorId, opt => opt.MapFrom(src => src.ApostadorCampeonato.Apostador.Id))
            //    .ForMember(dest => dest.NomeApostador, opt => opt.MapFrom(src => src.ApostadorCampeonato.Apostador.NomeCompleto))
            //    .ForMember(dest => dest.Apelido, opt => opt.MapFrom(src => src.ApostadorCampeonato.Apostador.Usuario.Apelido))
            //    .ForMember(dest => dest.FotoPerfil, opt => opt.MapFrom(src => src.ApostadorCampeonato.Apostador.Usuario.FotoPerfil));

            // **SUGESTÃO DE CORREÇÃO:**
            // Se RankingRodadaDto não tem Apelido e FotoPerfil, adicione-os lá.
            // Ou, se a API de Ranking da Rodada precisa retornar um DTO mais completo, mude o tipo de retorno.
            // Vamos assumir que você precisa de um mapeamento para RankingDto, que já contém essas propriedades.
            // Isso resolveria o problema do "ranking por rodada" que usa um DTO diferente do "ranking por campeonato".

            // NOVO E CORRETO MAPEAMENTO: RankingRodada -> RankingRodadaDto
            CreateMap<RankingRodada, RankingRodadaDto>()
                .ForMember(dest => dest.RodadaId, opt => opt.MapFrom(src => src.RodadaId))
                .ForMember(dest => dest.ApostadorCampeonatoId, opt => opt.MapFrom(src => src.ApostadorCampeonatoId))
                .ForMember(dest => dest.ApostadorId, opt => opt.MapFrom(src => src.ApostadorCampeonato.Apostador.Id))
                .ForMember(dest => dest.NomeApostador, opt => opt.MapFrom(src => src.ApostadorCampeonato.Apostador.NomeCompleto))
                .ForMember(dest => dest.Apelido, opt => opt.MapFrom(src => src.ApostadorCampeonato.Apostador.Usuario.Apelido))
                .ForMember(dest => dest.FotoPerfil, opt => opt.MapFrom(src => src.ApostadorCampeonato.Apostador.Usuario.FotoPerfil))
                .ForMember(dest => dest.Pontuacao, opt => opt.MapFrom(src => src.Pontuacao))
                .ForMember(dest => dest.Posicao, opt => opt.MapFrom(src => src.Posicao))
                .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(src => src.DataAtualizacao));

            // NOVO E CORRETO MAPEAMENTO PARA RANKING DO CAMPEONATO
            // Mapeia da interface IRankingResult para o DTO de Ranking
            CreateMap<IRankingResult, RankingDto>()
                .ForMember(dest => dest.Posicao, opt => opt.Ignore());

            // REMOVA O CÓDIGO ABAIXO QUE ESTÁ CAUSANDO O ERRO.
            // O AutoMapper já sabe como mapear a partir da interface
            /*
            CreateMap<RankingDataModel, RankingDto>()
                .ForMember(dest => dest.Apelido, opt => opt.MapFrom(src => src.Apelido))
                .ForMember(dest => dest.FotoPerfil, opt => opt.MapFrom(src => src.FotoPerfil))
                .ForMember(dest => dest.TotalPontos, opt => opt.MapFrom(src => src.TotalPontos))
                .ForMember(dest => dest.NomeApostador, opt => opt.MapFrom(src => src.NomeApostador));
            */

            // Mapeamento de RankingRodadaDto para RankingRodada
            CreateMap<RankingRodadaDto, RankingRodada>();

            // Adicione este mapeamento para a planilha de conferência
            CreateMap<IConferenciaPalpite, ConferenciaPalpiteDto>();
            //CreateMap<ConferenciaPalpiteDataModel, IConferenciaPalpite>();

        }
    }
}