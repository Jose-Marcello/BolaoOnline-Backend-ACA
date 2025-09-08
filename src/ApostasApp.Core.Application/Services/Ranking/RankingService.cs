// Localização: ApostasApp.Core.Application.Services/RankingService.cs
using ApostasApp.Core.Application.DTOs.Ranking;
using ApostasApp.Core.Application.DTOs.RankingRodadas;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Apostas;
using ApostasApp.Core.Application.Services.Interfaces;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces.RankingRodadas;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services
{
    public class RankingService : BaseService, IRankingService
    {
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IRankingRodadaRepository _rankingRodadaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApostaRodadaService> _logger;

        public RankingService(
            IFinanceiroService financeiroService,
            IUnitOfWork uow,
            IApostaRodadaRepository apostaRodadaRepository,
            IRankingRodadaRepository rankingRodadaRepository,
            IApostadorCampeonatoRepository apostadorCampeonatoRepository,
            IMapper mapper,
            ILogger<ApostaRodadaService> logger,
            INotificador notificador) : base(notificador, uow)
        {
            _apostaRodadaRepository = apostaRodadaRepository;
            _rankingRodadaRepository = rankingRodadaRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // Método para o ranking do campeonato
        // CORRIGIDO AQUI: O retorno agora é um ApiResponse, compatível com o controller.
        // Localização: ApostasApp.Core.Application.Services/RankingService.cs

        public async Task<ApiResponse<IEnumerable<RankingDto>>> ObterRankingCampeonatoAsync(Guid campeonatoId)
        {
            var rankingData = await _apostaRodadaRepository.ObterDadosRankingCampeonatoAsync(campeonatoId);

            if (rankingData == null || !rankingData.Any())
            {
                NotificarAlerta("Ranking do campeonato não encontrado.");
                return new ApiResponse<IEnumerable<RankingDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<RankingDto>()
                };
            }

            // A CORREÇÃO É AQUI: Calculamos a posição no service
            var rankingComPosicao = rankingData
                .Select((item, index) =>
                {
                    var dto = _mapper.Map<RankingDto>(item);
                    dto.Posicao = index + 1;
                    return dto;
                })
                .ToList();

            return new ApiResponse<IEnumerable<RankingDto>>
            {
                Success = true,
                Message = "Ranking do campeonato retornado com sucesso.",
                Data = rankingComPosicao
            };
        }

        // Método para o ranking da rodada
        public async Task<ApiResponse<IEnumerable<RankingRodadaDto>>> ObterRankingDaRodada(Guid rodadaId)
        {
            var ranking = await _rankingRodadaRepository.ObterRankingDaRodada(rodadaId);

            if (ranking == null || !ranking.Any())
            {
                NotificarAlerta("Ranking da rodada não encontrado.");
                return new ApiResponse<IEnumerable<RankingRodadaDto>>
                {
                    Success = true,
                    Data = Enumerable.Empty<RankingRodadaDto>()
                };
            }

            // O mapeamento já está configurado no MappingProfile.
            var rankingDto = _mapper.Map<IEnumerable<RankingRodadaDto>>(ranking);

            return new ApiResponse<IEnumerable<RankingRodadaDto>>
            {
                Success = true,
                Message = "Ranking da rodada retornado com sucesso.",
                Data = rankingDto
            };
        }
    }
}