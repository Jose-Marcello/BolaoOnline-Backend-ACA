using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.DTOs.Rodadas; // Para RodadaDto
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos; // Para ApostadorCampeonatoDto
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos; // Para IApostadorCampeonatoService
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Notificacoes; // Para NotificationDto
using AutoMapper; // Se precisar de mapeamento aqui, senão, o serviço já retorna DTOs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Removidos usings de ViewModels e repositórios diretos
// using ApostasApp.Core.Presentation.ViewModels;
// using ApostasApp.Core.Infrastructure.Data.Repository; // Exemplo de repositório direto

namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingRodadaController : BaseController
    {
        private readonly IMapper _mapper; // Mantido para flexibilidade, mas o ideal é que serviços retornem DTOs finais
        private readonly IRodadaService _rodadaService;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        // Se houver um serviço específico para RankingRodada, injete-o aqui:
        // private readonly IRankingRodadaService _rankingRodadaService;


        public RankingRodadaController(IMapper mapper,
                                       IRodadaService rodadaService,
                                       IApostadorCampeonatoService apostadorCampeonatoService,
                                       // IRankingRodadaService rankingRodadaService, // Se injetado
                                       INotificador notificador,
                                       IUnitOfWork uow) : base(notificador, uow)
        {
            _mapper = mapper;
            _rodadaService = rodadaService;
            _apostadorCampeonatoService = apostadorCampeonatoService;
            // _rankingRodadaService = rankingRodadaService; // Se injetado
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

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                Notificar("Alerta", "Rodada não encontrada.");
                // Retorna as notificações do serviço, se existirem, ou as notificações do controlador
                return NotFound(new ApiResponse<IEnumerable<ApostadorCampeonatoDto>>
                {
                    Success = false,
                    Data = null,
                    Notifications = rodadaResponse.Notifications.Any() ? rodadaResponse.Notifications.ToList() : ObterNotificacoesParaResposta().ToList() // <<-- CORRIGIDO .ToList() -->>
                });
            }

            var rodada = rodadaResponse.Data; // Obtém a entidade Rodada do ApiResponse

            // Agora, obtenha os apostadores do campeonato associados à rodada, com seus rankings.
            // Idealmente, haveria um método em IRankingRodadaService como ObterRankingDaRodada(rodadaId)
            // que retornaria DTOs de ranking.
            // Por simplicidade, vamos usar ObterApostadoresDoCampeonato que já traz dados de pontuação.
            var apostadoresRankingResponse = await _apostadorCampeonatoService.ObterApostadoresDoCampeonato(rodada.CampeonatoId);

            if (!apostadoresRankingResponse.Success || apostadoresRankingResponse.Data == null || !apostadoresRankingResponse.Data.Any())
            {
                Notificar("Alerta", "Nenhum ranking encontrado para esta rodada/campeonato.");
                // Retorna as notificações do serviço, se existirem, ou as notificações do controlador
                return NotFound(new ApiResponse<IEnumerable<ApostadorCampeonatoDto>>
                {
                    Success = false,
                    Data = null,
                    Notifications = apostadoresRankingResponse.Notifications.Any() ? apostadoresRankingResponse.Notifications.ToList() : ObterNotificacoesParaResposta().ToList() // <<-- CORRIGIDO .ToList() -->>
                });
            }

            // Retorne os dados como JSON
            return Ok(new ApiResponse<IEnumerable<ApostadorCampeonatoDto>>
            {
                Success = true,
                Data = apostadoresRankingResponse.Data,
                Notifications = new List<NotificationDto>() // Notificações de sucesso, se houver
            });
        }

        // Você pode adicionar outros métodos para gerenciar rankings aqui, se necessário.
    }
}
