using ApostasApp.Core.Application.DTOs.Apostas; // Para todos os DTOs
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos; // Para IApostadorCampeonatoService
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Application.Services.Interfaces.Apostas; // Para IApostaRodadaService
using ApostasApp.Core.Domain.Models.Rodadas; // Para StatusRodada
using AutoMapper; // Para IMapper (uso minimizado no Controller)
using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces.Usuarios;
namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApostadorCampeonatoController : BaseController
    {
        private readonly IMapper _mapper; // Mantido, mas seu uso no Controller será minimizado.
                                          // Mapeamento de Entidade para DTO deve ser feito no serviço.
        private readonly IApostadorCampeonatoService _apostadorCampeamentoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IRodadaService _rodadaService; // Serviço para operações de Rodada
        private readonly IApostaRodadaService _apostaRodadaService; // Serviço para operações de ApostaRodada
        // private readonly IUnitOfWork _uow; // REMOVIDO: Controllers não devem ter acesso direto ao UnitOfWork.
        private readonly ILogger<ApostadorCampeonatoController> _logger;

        public ApostadorCampeonatoController(IMapper mapper,
                                             IApostadorCampeonatoService apostadorCampeamentoService,
                                             IUsuarioService usuarioService,
                                             IRodadaService rodadaService,
                                             IApostaRodadaService apostaRodadaService,
                                             // IUnitOfWork uow, // Removido do construtor
                                             ILogger<ApostadorCampeonatoController> logger,
                                             INotificador notificador) : base(notificador)
        {
            _mapper = mapper;
            _apostadorCampeamentoService = apostadorCampeamentoService;
            _usuarioService = usuarioService;
            _rodadaService = rodadaService;
            _apostaRodadaService = apostaRodadaService;
            // _uow = uow; // Removido da atribuição
            _logger = logger;
        }

        // =========================================================================================================
        // CENÁRIO 1: RODADA EM APOSTAS (Onde o usuário pode fazer/editar as apostas)
        // =========================================================================================================

        [HttpGet("RodadaEmApostas/{apostadorCampeamentoId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaEmApostas(Guid apostadorCampeamentoId)
        {
            var apostadorCampeonato = await _apostadorCampeamentoService.ObterPorId(apostadorCampeamentoId);
            if (apostadorCampeonato == null)
            {
                Notificar("Alerta", "Apostador Campeonato não encontrado.");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            // Chamada corrigida para usar ObterRodadaEmApostasPorCampeonato do IRodadaService
            var rodada = await _rodadaService.ObterRodadaEmApostasPorCampeonato(apostadorCampeamentoId);
            if (rodada == null)
            {
                Notificar("Alerta", "No momento NÃO HÁ uma RODADA em APOSTAS para este campeonato, fique atento ao final da RODADA CORRENTE e ao aviso da próxima abertura de APOSTAS !!");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                Notificar("Erro", "Usuário não logado ou sessão expirada.");
                return Unauthorized(new { success = false, errors = ObterTodasNotificacoes() });
            }

            var apostaRodadaStatus = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeamentoId);

            return Ok(new
            {
                success = true,
                message = "Rodada 'Em Apostas' encontrada. Prossiga para listar/salvar apostas.",
                apostadorCampeamentoId = apostadorCampeamentoId,
                rodadaId = rodada.Id,
                apostadorApelido = usuario.Apelido,
                campeonatoNome = rodada.Campeonato?.Nome ?? "N/A",
                numeroRodada = rodada.NumeroRodada,
                statusEnvioAposta = apostaRodadaStatus?.Enviada == true ? "ENVIADA" : "NÃO ENVIADA",
                dataAposta = apostaRodadaStatus?.DataHoraAposta?.ToShortDateString(),
                horaAposta = apostaRodadaStatus?.DataHoraAposta?.ToShortTimeString(),
                apostaRodadaId = apostaRodadaStatus?.ApostaRodadaId
            });
        }

        [HttpGet("BuscarApostasParaEdicao/{apostadorCampeamentoId}/{rodadaId}")]
        public async Task<IActionResult> BuscarApostasParaEdicao(Guid apostadorCampeamentoId, Guid rodadaId)
        {
            // O serviço agora retorna o DTO de edição diretamente, sem Select no Controller
            var listaApostasDto = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaEdicao(rodadaId, apostadorCampeamentoId);

            if (listaApostasDto == null || !listaApostasDto.Any())
            {
                Notificar("Alerta", "Rodada não encontrada ou não está mais disponível para apostas, ou não há apostas para edição.");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            return Ok(new { success = true, data = listaApostasDto });
        }


        // =========================================================================================================
        // CENÁRIO 2: RODADA CORRENTE (Onde o usuário vê os placares reais e suas apostas, mas não edita)
        // =========================================================================================================

        [HttpGet("RodadaCorrente/{apostadorCampeamentoId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaCorrente(Guid apostadorCampeamentoId)
        {
            var apostadorCampeonato = await _apostadorCampeamentoService.ObterPorId(apostadorCampeamentoId);
            if (apostadorCampeonato == null)
            {
                Notificar("Alerta", "Apostador Campeonato não encontrado.");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            // Chamada corrigida para usar ObterRodadaCorrentePorCampeonato do IRodadaService
            var rodada = await _rodadaService.ObterRodadaCorrentePorCampeonato(apostadorCampeamentoId);
            if (rodada == null)
            {
                Notificar("Alerta", "Ainda não há uma RODADA CORRENTE, com Jogos em Andamento!! no momento para este campeonato.");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            if (rodada.Status != StatusRodada.Corrente)
            {
                Notificar("Alerta", "A rodada encontrada não está no status 'Corrente'.");
                return BadRequest(new { success = false, errors = ObterTodasNotificacoes() });
            }

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                Notificar("Erro", "Usuário não logado ou sessão expirada.");
                return Unauthorized(new { success = false, errors = ObterTodasNotificacoes() });
            }

            var apostaRodadaStatus = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeamentoId);

            return Ok(new
            {
                success = true,
                message = "Rodada 'Corrente' encontrada.",
                apostadorCampeamentoId = apostadorCampeamentoId,
                rodadaId = rodada.Id,
                apostadorApelido = usuario.Apelido,
                campeonatoNome = rodada.Campeonato?.Nome ?? "N/A",
                numeroRodada = rodada.NumeroRodada,
                statusEnvioAposta = apostaRodadaStatus?.Enviada == true ? "ENVIADA" : "NÃO ENVIADA",
                dataAposta = apostaRodadaStatus?.DataHoraAposta?.ToShortDateString(),
                horaAposta = apostaRodadaStatus?.DataHoraAposta?.ToShortTimeString()
            });
        }

        [HttpGet("BuscarApostasParaVisualizacaoCorrente/{apostadorCampeamentoId}/{rodadaId}")]
        public async Task<IActionResult> BuscarApostasParaVisualizacaoCorrente(Guid apostadorCampeamentoId, Guid rodadaId)
        {
            // O serviço agora retorna o DTO de visualização diretamente, sem Select no Controller
            var listaApostasDto = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaVisualizacao(rodadaId, apostadorCampeamentoId);

            if (listaApostasDto == null || !listaApostasDto.Any())
            {
                Notificar("Alerta", "Rodada corrente não encontrada ou não está mais ativa para visualização, ou não há apostas para visualização.");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            return Ok(new { success = true, data = listaApostasDto });
        }

        // =========================================================================================================
        // CENÁRIO 3: RODADA SELECIONADA (Onde o usuário vê os placares reais e suas apostas, mas não edita)
        // =========================================================================================================

        [HttpGet("RodadaSelecionada/{apostadorCampeamentoId}/{rodadaId}")]
        public async Task<IActionResult> ExibirInterfaceDaRodadaSelecionada(Guid apostadorCampeamentoId, Guid rodadaId)
        {
            var apostadorCampeonato = await _apostadorCampeamentoService.ObterPorId(apostadorCampeamentoId);
            if (apostadorCampeonato == null)
            {
                Notificar("Alerta", "Apostador Campeonato não encontrado.");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            var rodada = await _rodadaService.ObterRodadaPorId(rodadaId);
            if (rodada == null)
            {
                Notificar("Alerta", "Rodada selecionada não encontrada.");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            var usuario = await _usuarioService.GetLoggedInUser();
            if (usuario == null)
            {
                Notificar("Erro", "Usuário não logado ou sessão expirada.");
                return Unauthorized(new { success = false, errors = ObterTodasNotificacoes() });
            }

            var apostaRodadaStatus = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodada.Id, apostadorCampeamentoId);

            return Ok(new
            {
                success = true,
                message = "Rodada selecionada encontrada.",
                apostadorCampeamentoId = apostadorCampeamentoId,
                rodadaId = rodada.Id,
                apostadorApelido = usuario.Apelido,
                campeonatoNome = rodada.Campeonato?.Nome ?? "N/A",
                numeroRodada = rodada.NumeroRodada,
                statusEnvioAposta = apostaRodadaStatus?.Enviada == true ? "ENVIADA" : "NÃO ENVIADA",
                dataAposta = apostaRodadaStatus?.DataHoraAposta?.ToShortDateString(),
                horaAposta = apostaRodadaStatus?.DataHoraAposta?.ToShortTimeString()
            });
        }

        [HttpGet("BuscarApostasParaVisualizacaoSelecionada/{apostadorCampeamentoId}/{rodadaId}")]
        public async Task<IActionResult> BuscarApostasParaVisualizacaoSelecionada(Guid apostadorCampeamentoId, Guid rodadaId)
        {
            // O serviço agora retorna o DTO de visualização diretamente, sem Select no Controller
            var listaApostasDto = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaVisualizacao(rodadaId, apostadorCampeamentoId);

            if (listaApostasDto == null || !listaApostasDto.Any())
            {
                Notificar("Alerta", "Não foram encontradas apostas para a rodada selecionada ou o apostador.");
                return NotFound(new { success = false, errors = ObterTodasNotificacoes() });
            }

            return Ok(new { success = true, data = listaApostasDto });
        }

        [HttpGet("StatusApostaDaRodada")]
        public async Task<IActionResult> BuscarStatusEDataHoraApostaDaRodada(
            [FromQuery] Guid apostadorCampeamentoId,
            [FromQuery] Guid rodadaId)
        {
            try
            {
                var apostaStatus = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodadaId, apostadorCampeamentoId);

                if (apostaStatus != null)
                {
                    return Ok(new
                    {
                        success = true,
                        aposta = new
                        {
                            enviada = apostaStatus.Enviada,
                            dataHoraAposta = apostaStatus.DataHoraAposta?.ToString("o") // Formato ISO 8601 para Angular
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
                return StatusCode(500, new { success = false, errors = ObterTodasNotificacoes() });
            }
        }

        [HttpPost("SalvarApostas")]
        public async Task<IActionResult> SalvarApostas([FromBody] SalvarApostaRequestDto salvarApostaDto) // Parâmetro corrigido para o DTO de requisição
        {
            _logger.LogInformation("Iniciando a ação SalvarApostas.");

            if (salvarApostaDto == null || !salvarApostaDto.Palpites.Any()) // Validação usando o DTO
            {
                Notificar("Alerta", "Nenhuma aposta foi enviada para salvar.");
                return BadRequest(new { success = false, errors = ObterTodasNotificacoes() });
            }

            try
            {
                var result = await _apostaRodadaService.SalvarApostas(salvarApostaDto); // Passando o DTO

                if (!result)
                {
                    return BadRequest(new { success = false, errors = ObterTodasNotificacoes() });
                }

                return Ok(new { success = true, message = "Apostas salvas com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar apostas.");
                Notificar("Erro", $"Ocorreu um erro inesperado ao salvar as apostas: {ex.Message}");
                return StatusCode(500, new { success = false, errors = ObterTodasNotificacoes() });
            }
        }
    }
}


