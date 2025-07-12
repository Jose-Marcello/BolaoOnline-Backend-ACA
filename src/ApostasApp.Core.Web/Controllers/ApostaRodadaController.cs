// src/ApostasApp.Api/Controllers/ApostaRodadaController.cs

using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class ApostaRodadaController : BaseController
{
    private readonly IApostaRodadaService _apostaRodadaService;
    private readonly ILogger<ApostaRodadaController> _logger;

    public ApostaRodadaController(INotificador notificador,
                                  IUnitOfWork uow,
                                  IApostaRodadaService apostaRodadaService,
                                  ILogger<ApostaRodadaController> logger) : base(notificador, uow)
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
    public async Task<IActionResult> StatusAposta([FromQuery] Guid rodadaId, [FromQuery] Guid apostadorCampeamentoId)
    {
        try
        {
            var statusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodadaId, apostadorCampeamentoId);

            if (statusResponse.Success)
            {
                return Ok(new ApiResponse<ApostaRodadaStatusDto>
                {
                    Success = true,
                    Message = statusResponse.Message,
                    Data = statusResponse.Data,
                    Notifications = statusResponse.Notifications?.ToList() ?? new List<NotificationDto>()
                });
            }
            else
            {
                return Ok(new ApiResponse<ApostaRodadaStatusDto>
                {
                    Success = false,
                    Message = statusResponse.Message,
                    Data = null,
                    Notifications = statusResponse.Notifications?.ToList() ?? new List<NotificationDto>()
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter status da aposta da rodada.");
            Notificar("Erro", $"Erro interno ao obter status da aposta da rodada: {ex.Message}");
            return StatusCode(500, new ApiResponse<ApostaRodadaStatusDto>
            {
                Success = false,
                Message = "Ocorreu um erro interno no servidor ao obter o status da aposta da rodada.",
                Data = null,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }
    }
}