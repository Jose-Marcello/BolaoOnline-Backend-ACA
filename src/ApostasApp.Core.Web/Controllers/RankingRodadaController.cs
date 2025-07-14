// Localização: ApostasApp.Core.Web/Controllers/RankingRodadaController.cs

using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.DTOs.Rodadas; // Para RodadaDto
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos; // Para ApostadorCampeonatoDto
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos; // Para IApostadorCampeonatoService
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Web.Controllers; // Para BaseController
using AutoMapper; // Se precisar de mapeamento aqui, senão, o serviço já retorna DTOs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApostasApp.Core.Application.Services.Interfaces.RankingRodadas; // Para IRankingRodadaService (se for usar)

namespace ApostasApp.Core.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingRodadaController : BaseController
    {
        private readonly IMapper _mapper; // Mantido para flexibilidade, mas o ideal é que serviços retornem DTOs finais
        private readonly IRodadaService _rodadaService;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        // Se houver um serviço específico para RankingRodada, injete-o aqui:
        private readonly IRankingRodadaService _rankingRodadaService; // Removido o comentário se for usar

        public RankingRodadaController(IMapper mapper,
                                       IRodadaService rodadaService,
                                       IApostadorCampeonatoService apostadorCampeonatoService,
                                       IRankingRodadaService rankingRodadaService, // Se injetado
                                       INotificador notificador
                                       /* REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor */)
            : base(notificador) // Passa apenas o notificador para a BaseController
        {
            _mapper = mapper;
            _rodadaService = rodadaService;
            _apostadorCampeonatoService = apostadorCampeonatoService;
            _rankingRodadaService = rankingRodadaService; // Se injetado
        }

        /// <summary>
        /// Obtém o ranking de apostadores para uma rodada específica de um campeonato.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma lista de ApostadorCampeonatoDto com informações de ranking.</returns>
        [HttpGet("ranking-por-rodada/{rodadaId:guid}")]
        public async Task<IActionResult> ObterRankingRodada(Guid rodadaId)
        {
            // Primeiro, vamos obter a rodada para obter o CampeonatoId e garantir que a rodada existe.
            var rodadaResponse = await _rodadaService.ObterRodadaPorId(rodadaId);

            // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Rodada não encontrada.");
                return CustomResponse<IEnumerable<ApostadorCampeonatoDto>>(); // Retorna um erro genérico com as notificações
            }

            var rodada = rodadaResponse.Data;

            // Agora, obtenha os apostadores do campeonato associados à rodada, com seus rankings.
            // Idealmente, haveria um método em IRankingRodadaService como ObterRankingDaRodada(rodadaId)
            // que retornaria DTOs de ranking.
            // Por simplicidade, vamos usar ObterApostadoresDoCampeonato que já traz dados de pontuação.
            // Se _rankingRodadaService for usado, a chamada seria:
            // var apostadoresRankingResponse = await _rankingRodadaService.ObterRankingDaRodada(rodadaId);
            var apostadoresRankingResponse = await _apostadorCampeonatoService.ObterApostadoresDoCampeonato(rodada.CampeonatoId);

            // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
            if (!apostadoresRankingResponse.Success || apostadoresRankingResponse.Data == null || !apostadoresRankingResponse.Data.Any())
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Nenhum ranking encontrado para esta rodada/campeonato.");
                return CustomResponse<IEnumerable<ApostadorCampeonatoDto>>(); // Retorna um erro genérico com as notificações
            }

            // Retorne os dados como JSON encapsulados em ApiResponse de sucesso
            return CustomResponse(apostadoresRankingResponse.Data);
        }

        // Você pode adicionar outros métodos para gerenciar rankings aqui, se necessário.
    }
}
