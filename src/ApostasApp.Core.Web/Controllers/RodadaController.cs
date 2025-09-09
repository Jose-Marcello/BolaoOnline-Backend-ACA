// Localização: ApostasApp.Api.Controllers/RodadaController.cs
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Adicionado para INotificador
using ApostasApp.Core.Domain.Models.Notificacoes; // Adicionado para NotificationDto
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada (se usado em outros endpoints)
using ApostasApp.Core.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RodadaController : BaseController
    {
        private readonly IRodadaService _rodadaService;
        private readonly ILogger<RodadaController> _logger;

        public RodadaController(INotificador notificador, // Adicionado INotificador
                                 IRodadaService rodadaService,
                                 ILogger<RodadaController> logger)
            : base(notificador) // Passa o notificador para a BaseController
        {
            _rodadaService = rodadaService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todas as rodadas com status 'Em Apostas' para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">ID do Campeonato.</param>
        /// <returns>Uma lista de RodadaDto.</returns>
        [HttpGet("ListarEmApostas/{campeonatoId}")] // Endpoint para listar rodadas em apostas por campeonato
        public async Task<IActionResult> ListarRodadasEmApostas(string campeonatoId)
        {
            try
            {
                if (!Guid.TryParse(campeonatoId, out Guid campeonatoIdGuid))
                {
                    NotificarErro("ID do campeonato inválido.");
                    return CustomResponse<IEnumerable<RodadaDto>>(); // Retorna erro padronizado
                }

                var response = await _rodadaService.ObterRodadasEmApostaPorCampeonato(campeonatoIdGuid);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                return CustomResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar rodadas EM APOSTA por campeonato.");
                NotificarErro($"Erro interno ao listar rodadas: {ex.Message}");
                return CustomResponse<IEnumerable<RodadaDto>>(); // Retorna erro padronizado
            }
        }


        /// <summary>
        /// Lista todas as rodadas com status 'Em Apostas' para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">ID do Campeonato.</param>
        /// <returns>Uma lista de RodadaDto.</returns>
        [HttpGet("ListarCorrentes/{campeonatoId}")] // Endpoint para listar rodadas em apostas por campeonato
        public async Task<IActionResult> ListarRodadasCorrentes(string campeonatoId)
        {
            try
            {
                if (!Guid.TryParse(campeonatoId, out Guid campeonatoIdGuid))
                {
                    NotificarErro("ID do campeonato inválido.");
                    return CustomResponse<IEnumerable<RodadaDto>>(); // Retorna erro padronizado
                }

                var response = await _rodadaService.ObterRodadasCorrentePorCampeonato(campeonatoIdGuid);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                return CustomResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar rodadas CORRENTES por campeonato.");
                NotificarErro($"Erro interno ao listar rodadas: {ex.Message}");
                return CustomResponse<IEnumerable<RodadaDto>>(); // Retorna erro padronizado
            }
        }


        /// <summary>
        /// Lista todas as rodadas com status 'Em Apostas' para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">ID do Campeonato.</param>
        /// <returns>Uma lista de RodadaDto.</returns>
        [HttpGet("ListarFinalizadas/{campeonatoId}")] // Endpoint para listar rodadas em apostas por campeonato
        public async Task<IActionResult> ListarRodadasFinalizadas(string campeonatoId)
        {
            try
            {
                if (!Guid.TryParse(campeonatoId, out Guid campeonatoIdGuid))
                {
                    NotificarErro("ID do campeonato inválido.");
                    return CustomResponse<IEnumerable<RodadaDto>>(); // Retorna erro padronizado
                }

                var response = await _rodadaService.ObterRodadasFinalizadasPorCampeonato(campeonatoIdGuid);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                return CustomResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar rodadas FINALIZADAS por campeonato.");
                NotificarErro($"Erro interno ao listar rodadas: {ex.Message}");
                return CustomResponse<IEnumerable<RodadaDto>>(); // Retorna erro padronizado
            }
        }

        [HttpGet("ListarTodas/{campeonatoId}")] // Endpoint para listar rodadas em apostas por campeonato
        public async Task<IActionResult> ListarTodasAsRodadas(string campeonatoId)
        {
            try
            {
                if (!Guid.TryParse(campeonatoId, out Guid campeonatoIdGuid))
                {
                    NotificarErro("ID do campeonato inválido.");
                    return CustomResponse<IEnumerable<RodadaDto>>(); // Retorna erro padronizado
                }

                var response = await _rodadaService.ObterTodasAsRodadasDoCampeonato(campeonatoIdGuid);

                // Usa CustomResponse para retornar a ApiResponse do serviço de forma consistente
                return CustomResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar rodadas por campeonato.");
                NotificarErro($"Erro interno ao listar rodadas: {ex.Message}");
                return CustomResponse<IEnumerable<RodadaDto>>(); // Retorna erro padronizado
            }
        }



        // Outros endpoints do RodadaController devem ser mantidos e adaptados para CustomResponse
        // Exemplo:
        [HttpPost]
        public async Task<IActionResult> AdicionarRodada([FromBody] Rodada rodada)
        {
            try
            {
                // Assumindo que o serviço de rodada tem um método Adicionar(Rodada)
                // Se o DTO de entrada for diferente, adapte o mapeamento no serviço.
                // Se você não tiver um método Adicionar, este pode ser um Atualizar ou outro método de escrita.
                var response = await _rodadaService.Atualizar(rodada); // Exemplo: usando Atualizar para um objeto Rodada

                return CustomResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar rodada.");
                NotificarErro($"Erro interno ao adicionar rodada: {ex.Message}");
                return CustomResponse(); // Retorna erro padronizado
            }
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarRodada([FromBody] Rodada rodada)
        {
            try
            {
                var response = await _rodadaService.Atualizar(rodada);
                return CustomResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar rodada.");
                NotificarErro($"Erro interno ao atualizar rodada: {ex.Message}");
                return CustomResponse();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterRodadaPorId(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid rodadaIdGuid))
                {
                    NotificarErro("ID da rodada inválido.");
                    return CustomResponse<RodadaDto>();
                }

                var response = await _rodadaService.ObterRodadaPorId(rodadaIdGuid);
                return CustomResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodada por ID.");
                NotificarErro($"Erro interno ao obter rodada: {ex.Message}");
                return CustomResponse<RodadaDto>();
            }
        }



        [HttpGet("conferencia/{rodadaId:guid}")]
        public async Task<IActionResult> ObterPlanilhaConferencia(Guid rodadaId)
        {
            var result = await _rodadaService.GerarPlanilhaConferencia(rodadaId);

            // CORREÇÃO: Altere o CustomResponse para retornar o array diretamente
            // se o sucesso for verdadeiro. Isso evita o objeto `PreservedCollection`.
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return CustomResponse(result);
        }

    }
}