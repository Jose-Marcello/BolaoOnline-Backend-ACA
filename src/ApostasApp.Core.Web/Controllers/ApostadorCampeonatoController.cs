// Localização: ApostasApp.Core.Web/Controllers/ApostadorCampeonatoController.cs

using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.DTOs.Rodadas;
using ApostasApp.Core.Application.Services.Interfaces.Apostas;
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Application.Services.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Domain.Models.Rodadas;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ApostasApp.Core.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApostadorCampeonatoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IRodadaService _rodadaService;
        private readonly IApostaRodadaService _apostaRodadaService;
        private readonly ILogger<ApostadorCampeonatoController> _logger;

        public ApostadorCampeonatoController(IMapper mapper,
                                             IApostadorCampeonatoService apostadorCampeonatoService,
                                             IUsuarioService usuarioService,
                                             IRodadaService rodadaService,
                                             IApostaRodadaService apostaRodadaService,
                                             ILogger<ApostadorCampeonatoController> logger,
                                             INotificador notificador)
            : base(notificador)
        {
            _mapper = mapper;
            _apostadorCampeonatoService = apostadorCampeonatoService;
            _usuarioService = usuarioService;
            _rodadaService = rodadaService;
            _apostaRodadaService = apostaRodadaService;
            _logger = logger;
        }

        // =========================================================================================================
        // CENÁRIO 1: RODADA EM APOSTAS (Onde o usuário pode fazer/editar as apostas)
        // =========================================================================================================

        // CORREÇÃO AQUI: A rota foi ajustada para corresponder ao que o frontend está enviando.
        // Agora a URL será: api/ApostadorCampeonato/{apostadorCampeonatoId}/RodadasEmApostas
        [HttpGet("{apostadorCampeonatoId}/RodadasEmApostas")] // Rota corrigida
        public async Task<IActionResult> ExibirInterfaceDaRodadaEmApostas(Guid apostadorCampeonatoId)
        {
            var apostadorCampeonatoResponse = await _apostadorCampeonatoService.ObterPorId(apostadorCampeonatoId);

            if (!apostadorCampeonatoResponse.Success || apostadorCampeonatoResponse.Data == null)
            {
                NotificarAlerta("Apostador Campeonato não encontrado.");
                return CustomResponse<object>();
            }
            var apostadorCampeonato = apostadorCampeonatoResponse.Data;

            var rodadaResponse = await _rodadaService.ObterRodadaEmApostasPorCampeonato(apostadorCampeonato.CampeonatoId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                NotificarAlerta("No momento NÃO HÁ uma RODADA em APOSTAS para este campeonato, fique atento ao final da RODADA CORRENTE e ao aviso da próxima abertura de APOSTAS !!");
                return CustomResponse<object>();
            }

            var rodada = rodadaResponse.Data;

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                NotificarErro("Usuário não logado ou sessão expirada.");
                return CustomResponse<object>();
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data;

            return CustomResponse(new
            {
                apostadorCampeonatoId = apostadorCampeonatoId,
                rodadaId = rodada.Id,
                apostadorApelido = usuario.Apelido,
                campeonatoNome = rodada.Campeonato?.Nome ?? "N/A",
                numeroRodada = rodada.NumeroRodada,
                statusEnvioAposta = apostaRodadaStatus?.Enviada == true ? "ENVIADA" : "NÃO ENVIADA",
                dataAposta = apostaRodadaStatus?.DataHoraSubmissao?.ToShortDateString(),
                horaAposta = apostaRodadaStatus?.DataHoraSubmissao?.ToShortTimeString(),
                apostaRodadaId = apostaRodadaStatus?.ApostaRodadaId
            });
        }

        [HttpGet("BuscarApostasParaEdicao/{apostadorCampeonatoId}/{rodadaId}")]
        public async Task<IActionResult> BuscarApostasParaEdicao(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            var listaApostasResponse = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaEdicao(rodadaId, apostadorCampeonatoId);
            var listaApostasDto = listaApostasResponse.Data;

            if (!listaApostasResponse.Success || listaApostasDto == null || !listaApostasDto.Any())
            {
                NotificarAlerta("Rodada não encontrada ou não está mais disponível para apostas, ou não há apostas para edição.");
                return CustomResponse<IEnumerable<ApostaJogoDto>>();
            }

            return CustomResponse(listaApostasDto);
        }


        // =========================================================================================================
        // CENÁRIO 2: RODADA CORRENTE (Onde o usuário vê os placares reais e suas apostas, mas não edita)
        // =========================================================================================================

        [HttpGet("RodadaCorrente/{apostadorCampeonatoId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaCorrente(Guid apostadorCampeonatoId)
        {
            var apostadorCampeonatoResponse = await _apostadorCampeonatoService.ObterPorId(apostadorCampeonatoId);

            if (!apostadorCampeonatoResponse.Success || apostadorCampeonatoResponse.Data == null)
            {
                NotificarAlerta("Apostador Campeonato não encontrado.");
                return CustomResponse<object>();
            }
            var apostadorCampeonato = apostadorCampeonatoResponse.Data;

            var rodadaResponse = await _rodadaService.ObterRodadaCorrentePorCampeonato(apostadorCampeonato.CampeonatoId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                NotificarAlerta("Ainda não há uma RODADA CORRENTE, com Jogos em Andamento!! no momento para este campeonato.");
                return CustomResponse<object>();
            }

            var rodada = rodadaResponse.Data;

            if (rodada.Status != StatusRodada.Corrente)
            {
                NotificarAlerta("A rodada encontrada não está no status 'Corrente'.");
                return CustomResponse<object>();
            }

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                NotificarErro("Usuário não logado ou sessão expirada.");
                return CustomResponse<object>();
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data;

            return CustomResponse(new
            {
                apostadorCampeonatoId = apostadorCampeonatoId,
                rodadaId = rodada.Id,
                apostadorApelido = usuario.Apelido,
                campeonatoNome = rodada.Campeonato?.Nome ?? "N/A",
                numeroRodada = rodada.NumeroRodada,
                statusEnvioAposta = apostaRodadaStatus?.Enviada == true ? "ENVIADA" : "NÃO ENVIADA",
                dataAposta = apostaRodadaStatus?.DataHoraSubmissao?.ToShortDateString(),
                horaAposta = apostaRodadaStatus?.DataHoraSubmissao?.ToShortTimeString()
            });
        }

        [HttpGet("BuscarApostasParaVisualizacaoCorrente/{apostadorCampeonatoId}/{rodadaId}")]
        public async Task<IActionResult> BuscarApostasParaVisualizacaoCorrente(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            var listaApostasResponse = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaEdicao(rodadaId, apostadorCampeonatoId);
            var listaApostasDto = listaApostasResponse.Data;

            if (!listaApostasResponse.Success || listaApostasDto == null || !listaApostasDto.Any())
            {
                NotificarAlerta("Rodada corrente não encontrada ou não está mais ativa para visualização, ou não há apostas para visualização.");
                return CustomResponse<IEnumerable<ApostaJogoDto>>();
            }

            return CustomResponse(listaApostasDto);
        }

        // =========================================================================================================
        // CENÁRIO 3: RODADA SELECIONADA (Onde o usuário vê os placares reais e suas apostas, mas não edita)
        // =========================================================================================================

        [HttpGet("RodadaSelecionada/{apostadorCampeonatoId}/{rodadaId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaSelecionada(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            var apostadorCampeonatoResponse = await _apostadorCampeonatoService.ObterPorId(apostadorCampeonatoId);

            if (!apostadorCampeonatoResponse.Success || apostadorCampeonatoResponse.Data == null)
            {
                NotificarAlerta("Apostador Campeonato não encontrado.");
                return CustomResponse<object>();
            }

            var rodadaResponse = await _rodadaService.ObterRodadaPorId(rodadaId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                NotificarAlerta("Rodada selecionada não encontrada.");
                return CustomResponse<object>();
            }

            var rodada = rodadaResponse.Data;

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                NotificarErro("Usuário não logado ou sessão expirada.");
                return CustomResponse<object>();
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data;

            return CustomResponse(new
            {
                apostadorCampeonatoId = apostadorCampeonatoId,
                rodadaId = rodada.Id,
                apostadorApelido = usuario.Apelido,
                campeonatoNome = rodada.Campeonato?.Nome ?? "N/A",
                numeroRodada = rodada.NumeroRodada,
                statusEnvioAposta = apostaRodadaStatus?.Enviada == true ? "ENVIADA" : "NÃO ENVIADA",
                dataAposta = apostaRodadaStatus?.DataHoraSubmissao?.ToShortDateString(),
                horaAposta = apostaRodadaStatus?.DataHoraSubmissao?.ToShortTimeString()
            });
        }

        [HttpGet("BuscarApostasParaVisualizacaoSelecionada/{apostadorCampeonatoId}/{rodadaId}")]
        public async Task<IActionResult> BuscarApostasParaVisualizacaoSelecionada(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            var listaApostasResponse = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaEdicao(rodadaId, apostadorCampeonatoId);
            var listaApostasDto = listaApostasResponse.Data;

            if (!listaApostasResponse.Success || listaApostasDto == null || !listaApostasDto.Any())
            {
                NotificarAlerta("Não foram encontradas apostas para a rodada selecionada ou o apostador.");
                return CustomResponse<IEnumerable<ApostaJogoDto>>();
            }

            return CustomResponse(listaApostasDto);
        }

        [HttpGet("StatusApostaDaRodada")]
        public async Task<IActionResult> BuscarStatusEDataHoraApostaDaRodada(
            [FromQuery] Guid apostadorCampeonatoId,
            [FromQuery] Guid rodadaId)
        {
            try
            {
                var apostaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodadaId, apostadorCampeonatoId);
                var apostaStatus = apostaStatusResponse.Data;

                if (apostaStatus != null)
                {
                    return CustomResponse(new
                    {
                        enviada = apostaStatus.Enviada,
                        dataHoraAposta = apostaStatus.DataHoraSubmissao?.ToString("o")
                    });
                }
                else
                {
                    return CustomResponse(new { enviada = false, dataHoraAposta = (string)null });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar status e data/hora da aposta.");
                NotificarErro($"Erro ao buscar status e data/hora da aposta: {ex.Message}");
                return CustomResponse<object>();
            }
        }

        [HttpPost("SalvarApostas")]
        public async Task<IActionResult> SalvarApostas([FromBody] SalvarApostaRequestDto salvarApostaDto)
        {
            _logger.LogInformation("Iniciando a ação SalvarApostas.");

            if (salvarApostaDto == null || !salvarApostaDto.ApostasJogos.Any())
            {
                NotificarAlerta("Nenhuma aposta foi enviada para salvar.");
                return CustomResponse<bool>();
            }

            try
            {
                var result = await _apostaRodadaService.SalvarApostas(salvarApostaDto);

                if (!result.Success)
                {
                    return CustomResponse(result);
                }

                NotificarSucesso("Apostas salvas com sucesso!");
                return CustomResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar apostas.");
                NotificarErro($"Ocorreu um erro inesperado ao salvar as apostas: {ex.Message}");
                return CustomResponse<bool>();
            }
        }

        [HttpGet("GetPontuacaoTotalDoApostador")]
        public async Task<IActionResult> GetPontuacaoTotalDoApostador([FromQuery] Guid campeonatoId, [FromQuery] Guid apostadorId)
        {
            try
            {
                // Aqui você chamaria o seu serviço de ApostadorCampeonato para obter a pontuação
                var pontuacao = await _apostadorCampeonatoService.ObterPontuacaoTotal(campeonatoId, apostadorId);

                return CustomResponse(pontuacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a pontuação total do apostador.");
                NotificarErro($"Erro ao obter a pontuação total.");
                return CustomResponse<int>(); //(null, "Erro ao obter a pontuação total.");
            }
        }


    }
}
