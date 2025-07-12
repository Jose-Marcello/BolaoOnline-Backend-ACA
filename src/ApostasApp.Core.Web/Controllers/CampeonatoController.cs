// SeuProjetoBackend/Controllers/CampeonatoController.cs
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Notificacoes; // Para NotificationDto
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada (modelo de domínio, se RodadaService ainda retornar ele)
using ApostasApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Application.DTOs.Campeonatos; // Adicionado para CampeonatoDto
using System.Collections.Generic; // Para List
using System.Linq; // Para .ToList(), .Any()
using System.Security.Claims; // Para User.FindFirstValue
using System; // Para Guid
using Microsoft.Extensions.Logging;
using ApostasApp.Core.Application.Services.Interfaces.Apostadores; // <<-- ADICIONADO: Para IApostadorService

[Route("api/[controller]")] // Define a rota base para este controlador: /api/Campeonato
[ApiController] // Habilita funcionalidades de API
// [Authorize] // Se precisar de autenticação para todos os métodos no controlador
public class CampeonatoController : BaseController
{
    private readonly ICampeonatoService _campeonatoService;
    private readonly IRodadaService _rodadaService;
    private readonly ILogger<CampeonatoController> _logger;
    private readonly IApostadorService _apostadorService; // <<-- ADICIONADO: Injetar ApostadorService

    public CampeonatoController(
        ICampeonatoService campeonatoService,
        IRodadaService rodadaService,
        INotificador notificador,
        IUnitOfWork uow,
        ILogger<CampeonatoController> logger,
        IApostadorService apostadorService) : base(notificador, uow) // <<-- ADICIONADO logger e apostadorService -->>
    {
        _campeonatoService = campeonatoService;
        _rodadaService = rodadaService;
        _logger = logger;
        _apostadorService = apostadorService; // <<-- ATRIBUÍDO -->>
    }

    [HttpGet("{campeonatoId}/RodadaCorrente")]
    public async Task<IActionResult> GetRodadaCorrente(Guid campeonatoId)
    {
        try
        {
            var rodadasResponse = await _rodadaService.ObterRodadaCorrentePorCampeonato(campeonatoId);

            if (!rodadasResponse.Success)
            {
                return Ok(new ApiResponse<Rodada>
                {
                    Success = false,
                    Data = null,
                    Notifications = rodadasResponse.Notifications.ToList()
                });
            }

            var rodadaCorrente = rodadasResponse.Data;

            if (rodadaCorrente == null)
            {
                Notificar("Alerta", "Nenhuma rodada corrente encontrada para este campeonato.");
                return Ok(new ApiResponse<Rodada>
                {
                    Success = false,
                    Data = null,
                    Notifications = ObterNotificacoesParaResposta().ToList()
                });
            }

            return Ok(new ApiResponse<Rodada>
            {
                Success = true,
                Data = rodadaCorrente,
                Notifications = new List<NotificationDto>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter rodada corrente.");
            Notificar("Erro", $"Erro interno ao obter rodada corrente: {ex.Message}");
            return StatusCode(500, new ApiResponse<Rodada>
            {
                Success = false,
                Data = null,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }
    }

    [HttpGet("{campeonatoId}/RodadasEmApostas")]
    public async Task<IActionResult> GetRodadasEmApostas(Guid campeonatoId)
    {
        try
        {
            var rodadasResponse = await _rodadaService.ObterRodadasEmApostaPorCampeonato(campeonatoId);

            if (!rodadasResponse.Success)
            {
                return Ok(new ApiResponse<IEnumerable<Rodada>>
                {
                    Success = false,
                    Data = null,
                    Notifications = rodadasResponse.Notifications.ToList()
                });
            }

            var rodadas = rodadasResponse.Data;

            if (rodadas == null || !rodadas.Any())
            {
                Notificar("Alerta", "Nenhuma rodada 'Em Apostas' encontrada para este campeonato.");
                return Ok(new ApiResponse<IEnumerable<Rodada>>
                {
                    Success = false,
                    Data = null,
                    Notifications = ObterNotificacoesParaResposta().ToList()
                });
            }

            return Ok(new ApiResponse<IEnumerable<Rodada>>
            {
                Success = true,
                Data = rodadas,
                Notifications = new List<NotificationDto>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter rodadas 'Em Apostas'.");
            Notificar("Erro", $"Erro interno ao obter rodadas 'Em Apostas': {ex.Message}");
            return StatusCode(500, new ApiResponse<IEnumerable<Rodada>>
            {
                Success = false,
                Data = null,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }
    }

    [HttpGet("Detalhes/{id}")]
    public async Task<IActionResult> GetDetalhes(Guid id)
    {
        try
        {
            var campeonatoResponse = await _campeonatoService.GetDetalhesCampeonato(id);

            if (!campeonatoResponse.Success)
            {
                return Ok(new ApiResponse<CampeonatoDto>
                {
                    Success = false,
                    Data = null,
                    Notifications = campeonatoResponse.Notifications.ToList()
                });
            }

            var campeonatoDto = campeonatoResponse.Data;

            if (campeonatoDto == null)
            {
                Notificar("Erro", $"Campeonato com ID '{id}' não encontrado.");
                return NotFound(new ApiResponse<CampeonatoDto>
                {
                    Success = false,
                    Data = null,
                    Notifications = ObterNotificacoesParaResposta().ToList()
                });
            }

            return Ok(new ApiResponse<CampeonatoDto>
            {
                Success = true,
                Data = campeonatoDto,
                Notifications = new List<NotificationDto>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter detalhes do campeonato.");
            Notificar("Erro", $"Erro interno ao obter detalhes do campeonato: {ex.Message}");
            return StatusCode(500, new ApiResponse<CampeonatoDto>
            {
                Success = false,
                Data = null,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }
    }

    [HttpGet("Available")]
    public async Task<IActionResult> GetAvailableCampeonatos([FromQuery] string? userId = null)
    {
        try
        {
            var campeonatosResponse = await _campeonatoService.GetAvailableCampeonatos(userId);

            if (!campeonatosResponse.Success)
            {
                return Ok(new ApiResponse<IEnumerable<CampeonatoDto>>
                {
                    Success = false,
                    Data = null,
                    Notifications = campeonatosResponse.Notifications.ToList()
                });
            }

            var campeonatosDto = campeonatosResponse.Data;

            return Ok(new ApiResponse<IEnumerable<CampeonatoDto>>
            {
                Success = true,
                Data = campeonatosDto,
                Notifications = new List<NotificationDto>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter campeonatos disponíveis.");
            Notificar("Erro", $"Erro interno ao obter campeonatos disponíveis: {ex.Message}");
            return StatusCode(500, new ApiResponse<IEnumerable<CampeonatoDto>>
            {
                Success = false,
                Data = null,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }
    }

    [HttpPost("VincularApostador")]
    public async Task<IActionResult> VincularApostador([FromBody] VincularApostadorCampeonatoDto vincularDto)
    {
        try
        {
            // Tenta converter as strings para Guid
            Guid campeonatoIdGuid;
            // Guid apostadorIdGuid; // Não precisamos mais deste, pois obteremos o Apostador.Id

            if (!Guid.TryParse(vincularDto.CampeonatoId, out campeonatoIdGuid))
            {
                Notificar("Erro", "ID do Campeonato fornecido é inválido.");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Notifications = ObterNotificacoesParaResposta().ToList()
                });
            }

            // <<-- CORREÇÃO AQUI: Obter o ID do Apostador real a partir do UsuarioId logado -->>
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                Notificar("Erro", "Usuário não autenticado ou ID do usuário não encontrado.");
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Notifications = ObterNotificacoesParaResposta().ToList()
                });
            }
            string userId = userIdClaim.Value; // Este é o UsuarioId (AspNetUsers.Id)

            // Obter o Apostador (da tabela Apostadores) usando o UsuarioId
            var apostador = await _apostadorService.GetApostadorByUserIdAsync(userId);
            if (apostador == null)
            {
                Notificar("Erro", "Apostador não encontrado para o usuário logado. Verifique o registro do apostador.");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Notifications = ObterNotificacoesParaResposta().ToList()
                });
            }

            // Agora, use o Id da entidade Apostador para vincular
            Guid apostadorIdParaAdesao = apostador.Id;

            // Passa os GUIDs convertidos na ordem correta
            var sucessoAdesaoResponse = await _campeonatoService.AderirCampeonatoAsync(apostadorIdParaAdesao, campeonatoIdGuid);

            if (!sucessoAdesaoResponse.Success)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Notifications = sucessoAdesaoResponse.Notifications.ToList()
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Notifications = new List<NotificationDto> { new NotificationDto("SUCESSO_VINCULO", "Sucesso", "Apostador vinculado ao campeonato com sucesso!") }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao vincular apostador ao campeonato.");
            Notificar("Erro", $"Erro interno ao vincular apostador: {ex.Message}");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }
    }

    [HttpGet("{campeonatoId}/TodasAsRodadas")]
    public async Task<IActionResult> GetAllRodadasByCampeonatoId(Guid campeonatoId)
    {
        try
        {
            var rodadasResponse = await _rodadaService.ObterTodasAsRodadasDoCampeonato(campeonatoId);

            if (!rodadasResponse.Success)
            {
                return Ok(new ApiResponse<IEnumerable<Rodada>>
                {
                    Success = false,
                    Data = null,
                    Notifications = rodadasResponse.Notifications.ToList()
                });
            }

            var rodadas = rodadasResponse.Data;

            if (rodadas == null || !rodadas.Any())
            {
                Notificar("Alerta", "Nenhuma rodada encontrada para este campeonato.");
                return Ok(new ApiResponse<IEnumerable<Rodada>>
                {
                    Success = false,
                    Data = null,
                    Notifications = ObterNotificacoesParaResposta().ToList()
                });
            }

            return Ok(new ApiResponse<IEnumerable<Rodada>>
            {
                Success = true,
                Data = rodadas,
                Notifications = new List<NotificationDto>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as rodadas do campeonato.");
            Notificar("Erro", $"Erro interno ao obter todas as rodadas: {ex.Message}");
            return StatusCode(500, new ApiResponse<IEnumerable<Rodada>>
            {
                Success = false,
                Data = null,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }
    }
}
