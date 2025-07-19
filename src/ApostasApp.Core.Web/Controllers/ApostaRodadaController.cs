// Localização: ApostasApp.Core.Web/Controllers/ApostaRodadaController.cs (Assumindo que está na mesma pasta dos outros controllers web)

using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Web.Controllers; // Para BaseController
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.Web.Controllers // Namespace CORRIGIDO para ApostasApp.Core.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApostaRodadaController : BaseController
    {
        private readonly IApostaRodadaService _apostaRodadaService;
        private readonly ILogger<ApostaRodadaController> _logger;

        public ApostaRodadaController(INotificador notificador,
                                      // REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor
                                      IApostaRodadaService apostaRodadaService,
                                      ILogger<ApostaRodadaController> logger)
            : base(notificador) // Passa apenas o notificador para a BaseController
        {
            _apostaRodadaService = apostaRodadaService;
            _logger = logger;
        }

        /// <summary>
        /// Busca o status da aposta de uma rodada para um apostador.
        /// </summary>
        /// <param name="rodadaId">ID da rodada.</param>
        /// <param name="apostadorCampeonatoId">ID do apostador no campeonato.</param>
        /// <returns>ApiResponse contendo o status da aposta da rodada.</returns>
        [HttpGet("StatusAposta")]
        public async Task<IActionResult> StatusAposta([FromQuery] Guid rodadaId, [FromQuery] Guid apostadorCampeonatoId)
        {
            try
            {
                var statusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodadaId, apostadorCampeonatoId);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                return CustomResponse(statusResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter status da aposta da rodada.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Erro interno ao obter status da aposta da rodada: {ex.Message}");
                // Usa CustomResponse para retornar a resposta padronizada com as notificações
                return CustomResponse<ApostaRodadaStatusDto>(); // Retorna um erro genérico com as notificações
            }
        }
    }
}
