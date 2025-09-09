// Localização: ApostasApp.Core.Web/Controllers/CampeonatoController.cs (Assumindo que está na mesma pasta dos outros controllers web)

using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada (modelo de domínio)
using ApostasApp.Core.Web.Controllers; // Para BaseController
using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Application.DTOs.Campeonatos; // Adicionado para CampeonatoDto
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Logging;
using ApostasApp.Core.Application.Services.Interfaces.Apostadores;

namespace ApostasApp.Core.Web.Controllers // Namespace CORRIGIDO para ApostasApp.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Se precisar de autenticação para todos os métodos no controlador
    public class CampeonatoController : BaseController
    {
        private readonly ICampeonatoService _campeonatoService;
        private readonly IRodadaService _rodadaService;
        private readonly ILogger<CampeonatoController> _logger;
        private readonly IApostadorService _apostadorService;

        public CampeonatoController(
            ICampeonatoService campeonatoService,
            IRodadaService rodadaService,
            INotificador notificador,
            // REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor
            ILogger<CampeonatoController> logger,
            IApostadorService apostadorService)
            : base(notificador) // Passa apenas o notificador para a BaseController
        {
            _campeonatoService = campeonatoService;
            _rodadaService = rodadaService;
            _logger = logger;
            _apostadorService = apostadorService;
        }

        [HttpGet("{campeonatoId}/RodadaCorrente")]
        public async Task<IActionResult> GetRodadaCorrente(Guid campeonatoId)
        {
            try
            {
                var rodadasResponse = await _rodadaService.ObterRodadaCorrentePorCampeonato(campeonatoId);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                if (!rodadasResponse.Success)
                {
                    return CustomResponse(rodadasResponse); // Retorna a resposta de erro do serviço
                }

                var rodadaCorrente = rodadasResponse.Data;

                if (rodadaCorrente == null)
                {
                    // CORRIGIDO: Usando NotificarAlerta do BaseController
                    NotificarAlerta("Nenhuma rodada corrente encontrada para este campeonato.");
                    return CustomResponse<Rodada>(); // Retorna um erro genérico com as notificações
                }

                return CustomResponse(rodadaCorrente); // Retorna a Rodada encapsulada em ApiResponse de sucesso
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodada corrente.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Erro interno ao obter rodada corrente: {ex.Message}");
                return CustomResponse<Rodada>(); // Retorna um erro genérico com as notificações
            }
        }

        [HttpGet("{campeonatoId}/RodadasEmApostas")]
        public async Task<IActionResult> GetRodadasEmApostas(Guid campeonatoId)
        {
            try
            {
                var rodadasResponse = await _rodadaService.ObterRodadasEmApostaPorCampeonato(campeonatoId);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                if (!rodadasResponse.Success)
                {
                    return CustomResponse(rodadasResponse); // Retorna a resposta de erro do serviço
                }

                var rodadas = rodadasResponse.Data;

                if (rodadas == null || !rodadas.Any())
                {
                    // CORRIGIDO: Usando NotificarAlerta do BaseController
                    NotificarAlerta("Nenhuma rodada 'Em Apostas' encontrada para este campeonato.");
                    return CustomResponse<IEnumerable<Rodada>>(); // Retorna um erro genérico com as notificações
                }

                return CustomResponse(rodadas); // Retorna as Rodadas encapsuladas em ApiResponse de sucesso
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodadas 'Em Apostas'.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Erro interno ao obter rodadas 'Em Apostas': {ex.Message}");
                return CustomResponse<IEnumerable<Rodada>>(); // Retorna um erro genérico com as notificações
            }
        }

        [HttpGet("Detalhes/{id}")]
        public async Task<IActionResult> GetDetalhes(Guid id)
        {
            try
            {
                var campeonatoResponse = await _campeonatoService.GetDetalhesCampeonato(id);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                if (!campeonatoResponse.Success)
                {
                    return CustomResponse(campeonatoResponse); // Retorna a resposta de erro do serviço
                }

                var campeonatoDto = campeonatoResponse.Data;

                if (campeonatoDto == null)
                {
                    // CORRIGIDO: Usando NotificarErro do BaseController
                    NotificarErro($"Campeonato com ID '{id}' não encontrado.");
                    return CustomResponse<CampeonatoDto>(); // Retorna um erro genérico com as notificações
                }

                return CustomResponse(campeonatoDto); // Retorna o CampeonatoDto encapsulado em ApiResponse de sucesso
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes do campeonato.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Erro interno ao obter detalhes do campeonato: {ex.Message}");
                return CustomResponse<CampeonatoDto>(); // Retorna um erro genérico com as notificações
            }
        }

        [HttpGet("Available")]
        public async Task<IActionResult> GetAvailableCampeonatos([FromQuery] string? userId = null)
        {
            try
            {
                var campeonatosResponse = await _campeonatoService.GetAvailableCampeonatos(userId);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                if (!campeonatosResponse.Success)
                {
                    return CustomResponse(campeonatosResponse); // Retorna a resposta de erro do serviço
                }

                var campeonatosDto = campeonatosResponse.Data;

                return CustomResponse(campeonatosDto); // Retorna os CampeonatosDto encapsulados em ApiResponse de sucesso
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter campeonatos disponíveis.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Erro interno ao obter campeonatos disponíveis: {ex.Message}");
                return CustomResponse<IEnumerable<CampeonatoDto>>(); // Retorna um erro genérico com as notificações
            }
        }

        [HttpPost("VincularApostador")]
        public async Task<IActionResult> VincularApostador([FromBody] VincularApostadorCampeonatoDto vincularDto)
        {
            try
            {
                Guid campeonatoIdGuid;
                if (!Guid.TryParse(vincularDto.CampeonatoId, out campeonatoIdGuid))
                {
                    // CORRIGIDO: Usando NotificarErro do BaseController
                    NotificarErro("ID do Campeonato fornecido é inválido.");
                    return CustomResponse<object>(); // Retorna um erro genérico com as notificações
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    // CORRIGIDO: Usando NotificarErro do BaseController
                    NotificarErro("Usuário não autenticado ou ID do usuário não encontrado.");
                    return CustomResponse<object>(); // Retorna um erro genérico com as notificações
                }
                string userId = userIdClaim.Value;

                var apostador = await _apostadorService.GetApostadorByUserIdAsync(userId);
                if (apostador == null)
                {
                    // CORRIGIDO: Usando NotificarErro do BaseController
                    NotificarErro("Apostador não encontrado para o usuário logado. Verifique o registro do apostador.");
                    return CustomResponse<object>(); // Retorna um erro genérico com as notificações
                }

                Guid apostadorIdParaAdesao = apostador.Id;

                var sucessoAdesaoResponse = await _campeonatoService.AderirCampeonatoAsync(apostadorIdParaAdesao, campeonatoIdGuid);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                if (!sucessoAdesaoResponse.Success)
                {
                    return CustomResponse(sucessoAdesaoResponse); // Retorna a resposta de erro do serviço
                }

                // CORRIGIDO: Usando NotificarSucesso do BaseController
                NotificarSucesso("Apostador vinculado ao campeonato com sucesso!", "SUCESSO_VINCULO");
                return CustomResponse<object>(); // Retorna uma resposta de sucesso genérica com a notificação
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao vincular apostador ao campeonato.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Erro interno ao vincular apostador: {ex.Message}");
                return CustomResponse<object>(); // Retorna um erro genérico com as notificações
            }
        }

        [HttpGet("{campeonatoId}/TodasAsRodadas")]
        public async Task<IActionResult> GetAllRodadasByCampeonatoId(Guid campeonatoId)
        {
            try
            {
                var rodadasResponse = await _rodadaService.ObterTodasAsRodadasDoCampeonato(campeonatoId);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                if (!rodadasResponse.Success)
                {
                    return CustomResponse(rodadasResponse); // Retorna a resposta de erro do serviço
                }

                var rodadas = rodadasResponse.Data;

                if (rodadas == null || !rodadas.Any())
                {
                    // CORRIGIDO: Usando NotificarAlerta do BaseController
                    NotificarAlerta("Nenhuma rodada encontrada para este campeonato.");
                    return CustomResponse<IEnumerable<Rodada>>(); // Retorna um erro genérico com as notificações
                }

                return CustomResponse(rodadas); // Retorna as Rodadas encapsuladas em ApiResponse de sucesso
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as rodadas do campeonato.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Erro interno ao obter todas as rodadas: {ex.Message}");
                return CustomResponse<IEnumerable<Rodada>>(); // Retorna um erro genérico com as notificações
            }
        }
    }
}
