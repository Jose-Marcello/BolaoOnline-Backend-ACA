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

[Route("api/[controller]")] // Define a rota base para este controlador: /api/Campeonato
[ApiController] // Habilita funcionalidades de API
// [Authorize] // Se precisar de autenticação para todos os métodos no controlador
public class CampeonatoController : BaseController
{
    private readonly ICampeonatoService _campeonatoService;
    private readonly IRodadaService _rodadaService;

    public CampeonatoController(
        ICampeonatoService campeonatoService,
        IRodadaService rodadaService,
        INotificador notificador,
        IUnitOfWork uow) : base(notificador, uow)
    {
        _campeonatoService = campeonatoService;
        _rodadaService = rodadaService;
    }

    [HttpGet("{campeonatoId}/RodadaCorrente")]
    public async Task<IActionResult> GetRodadaCorrente(Guid campeonatoId)
    {
        // Se ObterRodadasCorrentePorCampeonato retorna um Rodada de domínio,
        // você precisará mapeá-lo para RodadaDto antes de retornar no ApiResponse.
        // Por enquanto, mantenho Rodada, mas idealmente seria RodadaDto.
        var rodadas = await _rodadaService.ObterRodadasCorrentePorCampeonato(campeonatoId); // Assumindo que o método é este

        if (_rodadaService.TemNotificacao())
        {
            // Retorna ApiResponse<Rodada> ou <RodadaDto> conforme a necessidade do frontend
            return Ok(new ApiResponse<Rodada>
            {
                Success = false,
                Data = null,
                Notifications = _rodadaService.ObterNotificacoesParaResposta().ToList()
            });
        }

        var rodadaCorrente = rodadas?.FirstOrDefault();

        if (rodadaCorrente == null && !_rodadaService.TemNotificacao())
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

    [HttpGet("{campeonatoId}/RodadasEmApostas")]
    public async Task<IActionResult> GetRodadasEmApostas(Guid campeonatoId)
    {
        // Se ObterRodadasEmApostaPorCampeonato retorna Rodada de domínio,
        // você precisará mapeá-lo para RodadaDto antes de retornar no ApiResponse.
        // Por enquanto, mantenho Rodada, mas idealmente seria RodadaDto.
        var rodadas = await _rodadaService.ObterRodadasEmApostaPorCampeonato(campeonatoId);

        if (_rodadaService.TemNotificacao())
        {
            return Ok(new ApiResponse<IEnumerable<Rodada>>
            {
                Success = false,
                Data = null,
                Notifications = _rodadaService.ObterNotificacoesParaResposta().ToList()
            });
        }

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

    [HttpGet("Detalhes/{id}")]
    public async Task<IActionResult> GetDetalhes(Guid id)
    {
        var campeonatoDto = await _campeonatoService.GetDetalhesCampeonato(id);

        if (_campeonatoService.TemNotificacao())
        {
            return Ok(new ApiResponse<CampeonatoDto>
            {
                Success = false,
                Data = null,
                Notifications = _campeonatoService.ObterNotificacoesParaResposta().ToList()
            });
        }

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
            Data = campeonatoDto.Data,
            Notifications = new List<NotificationDto>()
        });
    }

    [HttpGet("Available")]
    public async Task<IActionResult> GetAvailableCampeonatos([FromQuery] string? userId = null)
    {
        var campeonatosDto = await _campeonatoService.GetAvailableCampeonatos(userId);

        if (_campeonatoService.TemNotificacao())
        {
            return Ok(new ApiResponse<IEnumerable<CampeonatoDto>>
            {
                Success = false,
                Data = null,
                Notifications = _campeonatoService.ObterNotificacoesParaResposta().ToList()
            });
        }

        return Ok(new ApiResponse<IEnumerable<CampeonatoDto>>
        {
            Success = true,
            Data = campeonatosDto.Data,
            Notifications = new List<NotificationDto>()
        });
    }

    [HttpPost("VincularApostador")]
    public async Task<IActionResult> VincularApostador([FromBody] VincularApostadorCampeonatoDto vincularDto)
    {
        // Tenta converter as strings para Guid
        Guid campeonatoIdGuid;
        Guid apostadorIdGuid;

        if (!Guid.TryParse(vincularDto.CampeonatoId, out campeonatoIdGuid))
        {
            Notificar("Erro", "ID do Campeonato fornecido é inválido.");
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }

        if (!Guid.TryParse(vincularDto.ApostadorId, out apostadorIdGuid))
        {
            Notificar("Erro", "ID do Apostador fornecido é inválido.");
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Notifications = ObterNotificacoesParaResposta().ToList()
            });
        }

        // Passa os GUIDs convertidos na ordem correta
        var sucessoAdesao = await _campeonatoService.AderirCampeonatoAsync(campeonatoIdGuid, apostadorIdGuid);

        // <<-- CORRIGIDO: Linha 199 -->>
        if (!sucessoAdesao.Success || _campeonatoService.TemNotificacao())
        {
            return Ok(new ApiResponse<object>
            {
                Success = false,
                Notifications = _campeonatoService.ObterNotificacoesParaResposta().ToList()
            });
        }

        // CORREÇÃO AQUI: Adiciona o "Codigo" ao construtor de NotificationDto
        // O construtor agora é NotificationDto(string codigo, string tipo, string mensagem, string nomeCampo = null)
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Notifications = new List<NotificationDto> { new NotificationDto("SUCESSO_VINCULO", "Sucesso", "Apostador vinculado ao campeonato com sucesso!") }
        });
    }

    [HttpGet("{campeonatoId}/TodasAsRodadas")]
    public async Task<IActionResult> GetAllRodadasByCampeonatoId(Guid campeonatoId)
    {
        var rodadas = await _rodadaService.ObterTodasAsRodadasDoCampeonato(campeonatoId);

        if (_rodadaService.TemNotificacao())
        {
            return Ok(new ApiResponse<IEnumerable<Rodada>>
            {
                Success = false,
                Data = null,
                Notifications = _rodadaService.ObterNotificacoesParaResposta().ToList()
            });
        }

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
}
