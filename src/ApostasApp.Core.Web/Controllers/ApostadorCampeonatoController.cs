// Localização: ApostasApp.Core.Application.Services.Rodadas/RodadaService.cs

using ApostasApp.Core.Application.DTOs.Apostas; // Para todos os DTOs
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Apostas; // Para IApostaRodadaService
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos; // Para IApostadorCampeonatoService
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Application.Services.Interfaces.Usuarios; // Para IUsuarioService
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Rodadas; // Para StatusRodada
using AutoMapper; // Para IMapper (uso minimizado no Controller)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Para ILogger
using System; // Para Guid
using System.Linq; // Para .Any()

namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApostadorCampeonatoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IApostadorCampeonatoService _apostadorCampeonatoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IRodadaService _rodadaService; // Serviço para operações de Rodada
        private readonly IApostaRodadaService _apostaRodadaService; // Serviço para operações de ApostaRodada
        private readonly ILogger<ApostadorCampeonatoController> _logger;

        public ApostadorCampeonatoController(IMapper mapper,
                                             IApostadorCampeonatoService apostadorCampeonatoService,
                                             IUsuarioService usuarioService,
                                             IRodadaService rodadaService,
                                             IApostaRodadaService apostaRodadaService,
                                             IUnitOfWork uow, // Passando 'uow' para o construtor base
                                             ILogger<ApostadorCampeonatoController> logger,
                                             INotificador notificador) : base(notificador, uow)
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
            // <<-- CORRIGIDO: Obter ApostadorCampeonato como ApiResponse -->>
            var apostadorCampeonatoResponse = await _apostadorCampeonatoService.ObterPorId(apostadorCampeonatoId);

            if (!apostadorCampeonatoResponse.Success || apostadorCampeonatoResponse.Data == null)
            {
                Notificar("Alerta", "Apostador Campeonato não encontrado.");
                return NotFound(new { success = false, errors = apostadorCampeonatoResponse.Notifications.Any() ? apostadorCampeonatoResponse.Notifications : ObterNotificacoesParaResposta() });
            }
            var apostadorCampeonato = apostadorCampeonatoResponse.Data; // Obter a entidade ApostadorCampeonato

            // Chamada corrigida para usar ObterRodadaEmApostasPorCampeonato do IRodadaService
            // <<-- CORRIGIDO: Acessando CampeonatoId da propriedade Data do ApostadorCampeonato -->>
            var rodadaResponse = await _rodadaService.ObterRodadaEmApostasPorCampeonato(apostadorCampeonato.CampeonatoId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                Notificar("Alerta", "No momento NÃO HÁ uma RODADA em APOSTAS para este campeonato, fique atento ao final da RODADA CORRENTE e ao aviso da próxima abertura de APOSTAS !!");
                return NotFound(new { success = false, errors = rodadaResponse.Notifications.Any() ? rodadaResponse.Notifications : ObterNotificacoesParaResposta() });
            }

            var rodada = rodadaResponse.Data; // Obtém a entidade Rodada do ApiResponse

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                Notificar("Erro", "Usuário não logado ou sessão expirada.");
                return Unauthorized(new { success = false, errors = ObterNotificacoesParaResposta() });
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data; // Acessa o DTO real

            return Ok(new
            {
                success = true,
                message = "Rodada 'Em Apostas' encontrada. Prossiga para listar/salvar apostas.",
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
            var listaApostasDto = listaApostasResponse.Data; // Acessa a lista de DTOs real

            if (!listaApostasResponse.Success || listaApostasDto == null || !listaApostasDto.Any())
            {
                Notificar("Alerta", "Rodada não encontrada ou não está mais disponível para apostas, ou não há apostas para edição.");
                return NotFound(new { success = false, errors = listaApostasResponse.Notifications.Any() ? listaApostasResponse.Notifications : ObterNotificacoesParaResposta() });
            }

            return Ok(new { success = true, data = listaApostasDto });
        }


        // =========================================================================================================
        // CENÁRIO 2: RODADA CORRENTE (Onde o usuário vê os placares reais e suas apostas, mas não edita)
        // =========================================================================================================

        [HttpGet("RodadaCorrente/{apostadorCampeonatoId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaCorrente(Guid apostadorCampeonatoId)
        {
            // <<-- CORRIGIDO: Obter ApostadorCampeonato como ApiResponse -->>
            var apostadorCampeonatoResponse = await _apostadorCampeonatoService.ObterPorId(apostadorCampeonatoId);

            if (!apostadorCampeonatoResponse.Success || apostadorCampeonatoResponse.Data == null)
            {
                Notificar("Alerta", "Apostador Campeonato não encontrado.");
                return NotFound(new { success = false, errors = apostadorCampeonatoResponse.Notifications.Any() ? apostadorCampeonatoResponse.Notifications : ObterNotificacoesParaResposta() });
            }
            var apostadorCampeonato = apostadorCampeonatoResponse.Data; // Obter a entidade ApostadorCampeonato

            // Chamada corrigida para usar ObterRodadaCorrentePorCampeonato do IRodadaService
            // <<-- CORRIGIDO: Acessando CampeonatoId da propriedade Data do ApostadorCampeonato -->>
            var rodadaResponse = await _rodadaService.ObterRodadaCorrentePorCampeonato(apostadorCampeonato.CampeonatoId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                Notificar("Alerta", "Ainda não há uma RODADA CORRENTE, com Jogos em Andamento!! no momento para este campeonato.");
                return NotFound(new { success = false, errors = rodadaResponse.Notifications.Any() ? rodadaResponse.Notifications : ObterNotificacoesParaResposta() });
            }

            var rodada = rodadaResponse.Data; // Obtém a entidade Rodada do ApiResponse

            if (rodada.Status != StatusRodada.Corrente)
            {
                Notificar("Alerta", "A rodada encontrada não está no status 'Corrente'.");
                return BadRequest(new { success = false, errors = ObterNotificacoesParaResposta() });
            }

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                Notificar("Erro", "Usuário não logado ou sessão expirada.");
                return Unauthorized(new { success = false, errors = ObterNotificacoesParaResposta() });
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data; // Acessa o DTO real

            return Ok(new
            {
                success = true,
                message = "Rodada 'Corrente' encontrada.",
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
            var listaApostasDto = listaApostasResponse.Data; // Acessa a lista de DTOs real

            if (!listaApostasResponse.Success || listaApostasDto == null || !listaApostasDto.Any())
            {
                Notificar("Alerta", "Rodada corrente não encontrada ou não está mais ativa para visualização, ou não há apostas para visualização.");
                return NotFound(new { success = false, errors = listaApostasResponse.Notifications.Any() ? listaApostasResponse.Notifications : ObterNotificacoesParaResposta() });
            }

            return Ok(new { success = true, data = listaApostasDto });
        }

        // =========================================================================================================
        // CENÁRIO 3: RODADA SELECIONADA (Onde o usuário vê os placares reais e suas apostas, mas não edita)
        // =========================================================================================================

        [HttpGet("RodadaSelecionada/{apostadorCampeonatoId}/{rodadaId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaSelecionada(Guid apostadorCampeonatoId, Guid rodadaId)
        {
            // <<-- CORRIGIDO: Obter ApostadorCampeonato como ApiResponse -->>
            var apostadorCampeonatoResponse = await _apostadorCampeonatoService.ObterPorId(apostadorCampeonatoId);

            if (!apostadorCampeonatoResponse.Success || apostadorCampeonatoResponse.Data == null)
            {
                Notificar("Alerta", "Apostador Campeonato não encontrado.");
                return NotFound(new { success = false, errors = apostadorCampeonatoResponse.Notifications.Any() ? apostadorCampeonatoResponse.Notifications : ObterNotificacoesParaResposta() });
            }
            // var apostadorCampeonato = apostadorCampeonatoResponse.Data; // Não é usado diretamente aqui, mas mantido para consistência

            var rodadaResponse = await _rodadaService.ObterRodadaPorId(rodadaId);

            if (!rodadaResponse.Success || rodadaResponse.Data == null)
            {
                Notificar("Alerta", "Rodada selecionada não encontrada.");
                return NotFound(new { success = false, errors = rodadaResponse.Notifications.Any() ? rodadaResponse.Notifications : ObterNotificacoesParaResposta() });
            }

            var rodada = rodadaResponse.Data; // Obtém a entidade Rodada do ApiResponse

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                Notificar("Erro", "Usuário não logado ou sessão expirada.");
                return Unauthorized(new { success = false, errors = ObterNotificacoesParaResposta() });
            }

            var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeonatoId);
            var apostaRodadaStatus = apostaRodadaStatusResponse.Data; // Acessa o DTO real

            return Ok(new
            {
                success = true,
                message = "Rodada selecionada encontrada.",
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
            var listaApostasDto = listaApostasResponse.Data; // Acessa a lista de DTOs real

            if (!listaApostasResponse.Success || listaApostasDto == null || !listaApostasDto.Any())
            {
                Notificar("Alerta", "Não foram encontradas apostas para a rodada selecionada ou o apostador.");
                return NotFound(new { success = false, errors = listaApostasResponse.Notifications.Any() ? listaApostasResponse.Notifications : ObterNotificacoesParaResposta() });
            }

            return Ok(new { success = true, data = listaApostasDto });
        }

        [HttpGet("StatusApostaDaRodada")]
        public async Task<IActionResult> BuscarStatusEDataHoraApostaDaRodada(
            [FromQuery] Guid apostadorCampeonatoId,
            [FromQuery] Guid rodadaId)
        {
            try
            {
                var apostaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodadaId, apostadorCampeonatoId);
                var apostaStatus = apostaStatusResponse.Data; // Acessa o DTO real

                if (apostaStatus != null)
                {
                    return Ok(new
                    {
                        success = true,
                        aposta = new
                        {
                            enviada = apostaStatus.Enviada,
                            dataHoraAposta = apostaStatus.DataHoraSubmissao?.ToString("o") // Formato ISO 8601 para Angular
                        }
                    });
                }
                else
                {
                    return Ok(new { success = true, aposta = new { enviada = false, dataHoraAposta = (string)null } });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar status e data/hora da aposta.");
                Notificar("Erro", $"Erro ao buscar status e data/hora da aposta: {ex.Message}");
                return StatusCode(500, new { success = false, errors = ObterNotificacoesParaResposta() });
            }
        }

        [HttpPost("SalvarApostas")]
        public async Task<IActionResult> SalvarApostas([FromBody] SalvarApostaRequestDto salvarApostaDto) // Parâmetro corrigido para o DTO de requisição
        {
            _logger.LogInformation("Iniciando a ação SalvarApostas.");

            if (salvarApostaDto == null || !salvarApostaDto.ApostasJogos.Any()) // Validação usando o DTO
            {
                Notificar("Alerta", "Nenhuma aposta foi enviada para salvar.");
                return BadRequest(new { success = false, errors = ObterNotificacoesParaResposta() });
            }

            try
            {
                var result = await _apostaRodadaService.SalvarApostas(salvarApostaDto); // Passando o DTO

                if (!result.Success)
                {
                    // As notificações já virão do serviço, apenas as repassa
                    return BadRequest(new { success = false, errors = result.Notifications });
                }

                return Ok(new { success = true, message = "Apostas salvas com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar apostas.");
                Notificar("Erro", $"Ocorreu um erro inesperado ao salvar as apostas: {ex.Message}");
                return StatusCode(500, new { success = false, errors = ObterNotificacoesParaResposta() });
            }
        }
    }
        

}
