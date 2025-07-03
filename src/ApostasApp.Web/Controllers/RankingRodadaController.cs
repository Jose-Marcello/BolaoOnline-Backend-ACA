using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos; // Para IApostadorCampeonatoService
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Application.DTOs.Rodadas; // Para RodadaDto
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos; // Para ApostadorCampeonatoDto
using AutoMapper; // Se precisar de mapeamento aqui, senão, o serviço já retorna DTOs

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApostasApp.Core.Domain.Interfaces; // *** NOVO: Necessário para IUnitOfWork ***


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
                                       IUnitOfWork uow) : base(notificador, uow) // *** CORRIGIDO: Passando 'uow' para o construtor base ***
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
            // O serviço de apostador-campeonato deve ser capaz de obter os apostadores já com ranking.
            // Assumindo que ObterApostadoresDoCampeonato ou um método similar no IApostadorCampeonatoService
            // já traz os dados de ranking e usuário, ou que você tem um serviço de RankingRodada dedicado.

            // Primeiro, vamos obter a rodada para obter o CampeonatoId e garantir que a rodada existe.
            var rodada = await _rodadaService.ObterRodadaPorId(rodadaId);
            if (rodada == null)
            {
                Notificar("Alerta", "Rodada não encontrada.");
                return NotFound(new { success = false, errors = ObterNotificacoesParaResposta() }); // *** CORRIGIDO ***
            }

            // Agora, obtenha os apostadores do campeonato associados à rodada, com seus rankings.
            // Idealmente, haveria um método em IRankingRodadaService como ObterRankingDaRodada(rodadaId)
            // que retornaria DTOs de ranking.
            // Por simplicidade, vamos usar ObterApostadoresDoCampeonato que já traz dados de pontuação.
            var apostadoresRankingDto = await _apostadorCampeonatoService.ObterApostadoresDoCampeonato(rodada.CampeonatoId);

            if (!apostadoresRankingDto.Any())
            {
                Notificar("Alerta", "Nenhum ranking encontrado para esta rodada/campeonato.");
                return NotFound(new { success = false, errors = ObterNotificacoesParaResposta() }); // *** CORRIGIDO ***
            }

            // Filtrar os apostadores pelo ID da rodada específica, se o DTO ou entidade ApostadorCampeonato
            // tiver uma referência à rodada, ou se o serviço de ranking for usado.
            // Por enquanto, o ObterApostadoresDoCampeonato traz todos os apostadores do campeonato.
            // Você pode querer um DTO mais específico para ranking de rodada se houver a Posicao.
            // Para o exemplo, vamos assumir que o ApostadorCampeonatoDto já tem a Pontuacao.

            // Retorne os dados como JSON
            return Ok(apostadoresRankingDto);
        }

        // Você pode adicionar outros métodos para gerenciar rankings aqui, se necessário.
    }
}
