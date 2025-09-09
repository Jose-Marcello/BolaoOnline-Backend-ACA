// Localização: ApostasApp.Core.Web/Controllers/RankingController.cs

using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.DTOs.Ranking;
using ApostasApp.Core.Application.DTOs.RankingRodadas;
using ApostasApp.Core.Application.Services.Interfaces; // Para IRankingService
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingController : BaseController
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService, INotificador notificador)
            : base(notificador)
        {
            _rankingService = rankingService;
        }

        /// <summary>
        /// Obtém o ranking de apostadores para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma lista de RankingRodadaDto com informações de ranking.</returns>
        [HttpGet("rodada/{rodadaId:guid}")]
        public async Task<IActionResult> ObterRankingRodada(Guid rodadaId)
        {
            // O serviço já retorna um ApiResponse, então apenas o chamamos.
            var rankingResponse = await _rankingService.ObterRankingDaRodada(rodadaId);

            // Usa CustomResponse que já trata o retorno e as notificações do BaseController
            return CustomResponse(rankingResponse);
        }

        /// <summary>
        /// Obtém o ranking total do campeonato.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma lista de RankingDto com o ranking total.</returns>
        [HttpGet("campeonato/{campeonatoId:guid}")]
        public async Task<IActionResult> ObterRankingCampeonato(Guid campeonatoId)
        {
            // Chama o método no serviço que retorna a ApiResponse com o ranking total
            var rankingResponse = await _rankingService.ObterRankingCampeonatoAsync(campeonatoId);

            // Usa CustomResponse para tratar a resposta de forma consistente
            return CustomResponse(rankingResponse);
        }
    }
}