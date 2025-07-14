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
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Domain.Models.Rodadas; // Para StatusRodada
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // Necessário para ObterUsuarioIdLogado

namespace ApostasApp.Core.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos os métodos neste controlador exigirão autenticação por padrão, EXCETO os com [AllowAnonymous]
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
                                             // REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor
                                             ILogger<ApostadorCampeonatoController> logger,
                                             INotificador notificador)
            : base(notificador) // Passa apenas o notificador para a BaseController
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

        [HttpGet("RodadaEmApostas/{apostadorCampeonatoId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaEmApostas(Guid apostadorCampeonatoId)
        {
            var apostadorCampeonatoResponse = await _apostadorCampeonatoService.ObterPorId(apostadorCampeonatoId);

            if (!apostadorCampeonatoResponse.Success || apostadorCampeonatoResponse.Data == null)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Apostador Campeonato não encontrado.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }
            var apostadorCampeonato = apostadorCampeonatoResponse.Data;

            var rodadaResponse = await _rodadaService.ObterRodadaEmApostasPorCampeonato(apostadorCampeonato.CampeonatoId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("No momento NÃO HÁ uma RODADA em APOSTAS para este campeonato, fique atento ao final da RODADA CORRENTE e ao aviso da próxima abertura de APOSTAS !!");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }

            var rodada = rodadaResponse.Data;

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro("Usuário não logado ou sessão expirada.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data;

            // Retorna um ApiResponse de sucesso com os dados formatados
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
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Rodada não encontrada ou não está mais disponível para apostas, ou não há apostas para edição.");
                return CustomResponse<IEnumerable<ApostaJogoDto>>(); // Usa CustomResponse do BaseController
            }

            return CustomResponse(listaApostasDto); // Retorna a lista de DTOs encapsulada em ApiResponse de sucesso
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
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Apostador Campeonato não encontrado.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }
            var apostadorCampeonato = apostadorCampeonatoResponse.Data;

            var rodadaResponse = await _rodadaService.ObterRodadaCorrentePorCampeonato(apostadorCampeonato.CampeonatoId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Ainda não há uma RODADA CORRENTE, com Jogos em Andamento!! no momento para este campeonato.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }

            var rodada = rodadaResponse.Data;

            if (rodada.Status != StatusRodada.Corrente)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("A rodada encontrada não está no status 'Corrente'.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro("Usuário não logado ou sessão expirada.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data;

            // Retorna um ApiResponse de sucesso com os dados formatados
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
            var listaApostasResponse = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaVisualizacao(rodadaId, apostadorCampeonatoId);
            var listaApostasDto = listaApostasResponse.Data;

            if (!listaApostasResponse.Success || listaApostasDto == null || !listaApostasDto.Any())
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Rodada corrente não encontrada ou não está mais ativa para visualização, ou não há apostas para visualização.");
                return CustomResponse<IEnumerable<ApostaJogoDto>>(); // Usa CustomResponse do BaseController
            }

            return CustomResponse(listaApostasDto); // Retorna a lista de DTOs encapsulada em ApiResponse de sucesso
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
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Apostador Campeonato não encontrado.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }

            var rodadaResponse = await _rodadaService.ObterRodadaPorId(rodadaId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Rodada selecionada não encontrada.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }

            var rodada = rodadaResponse.Data;

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro("Usuário não logado ou sessão expirada.");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data;

            // Retorna um ApiResponse de sucesso com os dados formatados
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
            var listaApostasResponse = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaVisualizacao(rodadaId, apostadorCampeonatoId);
            var listaApostasDto = listaApostasResponse.Data;

            if (!listaApostasResponse.Success || listaApostasDto == null || !listaApostasDto.Any())
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Não foram encontradas apostas para a rodada selecionada ou o apostador.");
                return CustomResponse<IEnumerable<ApostaJogoDto>>(); // Usa CustomResponse do BaseController
            }

            return CustomResponse(listaApostasDto); // Retorna a lista de DTOs encapsulada em ApiResponse de sucesso
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
                    // Retorna um ApiResponse de sucesso com os dados formatados
                    return CustomResponse(new
                    {
                        enviada = apostaStatus.Enviada,
                        dataHoraAposta = apostaStatus.DataHoraSubmissao?.ToString("o") // Formato ISO 8601 para Angular
                    });
                }
                else
                {
                    // Retorna um ApiResponse de sucesso com dados vazios/default
                    return CustomResponse(new { enviada = false, dataHoraAposta = (string)null });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar status e data/hora da aposta.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Erro ao buscar status e data/hora da aposta: {ex.Message}");
                return CustomResponse<object>(); // Usa CustomResponse do BaseController
            }
        }

        [HttpPost("SalvarApostas")]
        public async Task<IActionResult> SalvarApostas([FromBody] SalvarApostaRequestDto salvarApostaDto)
        {
            _logger.LogInformation("Iniciando a ação SalvarApostas.");

            if (salvarApostaDto == null || !salvarApostaDto.ApostasJogos.Any())
            {
                // CORRIGIDO: Usando NotificarAlerta do BaseController
                NotificarAlerta("Nenhuma aposta foi enviada para salvar.");
                return CustomResponse<bool>(); // Usa CustomResponse do BaseController
            }

            try
            {
                var result = await _apostaRodadaService.SalvarApostas(salvarApostaDto);

                if (!result.Success)
                {
                    // Se o serviço já adicionou notificações, elas serão capturadas pelo CustomResponse
                    return CustomResponse(result); // Retorna a ApiResponse do serviço de forma consistente
                }

                // CORRIGIDO: Usando NotificarSucesso do BaseController
                NotificarSucesso("Apostas salvas com sucesso!");
                return CustomResponse(true); // Retorna ApiResponse de sucesso com true
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar apostas.");
                // CORRIGIDO: Usando NotificarErro do BaseController
                NotificarErro($"Ocorreu um erro inesperado ao salvar as apostas: {ex.Message}");
                return CustomResponse<bool>(); // Usa CustomResponse do BaseController
            }
        }
    }
}
