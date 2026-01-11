// Localização: ApostasApp.Core.Web/Controllers/ApostaRodadaController.cs

using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Apostas;
using ApostasApp.Core.Application.Services.Interfaces.Usuarios;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Campeonatos; // Necessário para FromForm
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Web.Controllers; // Para BaseController
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Web.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ApostaRodadaController : BaseController
  {
    private readonly IApostaRodadaService _apostaRodadaService;
    private readonly ILogger<ApostaRodadaController> _logger;
    private readonly IUsuarioService _usuarioService; // <--- ADICIONADO

    public ApostaRodadaController(INotificador notificador,
                                   IApostaRodadaService apostaRodadaService,
                                   IUsuarioService usuarioService, // <--- INJETADO
                                   ILogger<ApostaRodadaController> logger)
            : base(notificador)
    {
      _apostaRodadaService = apostaRodadaService;
      _usuarioService = usuarioService; // <--- ATRIBUÍDO
      _logger = logger;
    }
    /// <summary>
    /// Busca o status da aposta de uma rodada para um apostador.
    /// </summary>
    /// <param name="rodadaId">ID da rodada.</param>
    /// <param name="apostadorCampeonatoId">ID do apostador no campeonato.</param>
    /// <returns>ApiResponse contendo o status da aposta da rodada.</returns>
    ///
    [AllowAnonymous]
    [HttpGet("StatusAposta")]
    public async Task<IActionResult> StatusAposta([FromQuery] Guid rodadaId, [FromQuery] Guid? apostadorCampeonatoId, Guid apostadorId)
    {
      try
      {
        var statusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(rodadaId, apostadorCampeonatoId, apostadorId);

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


    /// <summary>
    /// Busca as apostas de uma rodada específica para edição por um apostador.
    /// Este endpoint é usado para carregar os palpites existentes para que o usuário possa editá-los.
    /// </summary>
    /// <param name="rodadaId">ID da rodada.</param>
    /// <param name="apostadorCampeonatoId">ID do relacionamento Apostador-Campeonato.</param>
    /// <returns>ApiResponse contendo a ApostaRodadaDto (ou uma lista, dependendo da sua lógica de serviço).</returns>
    [HttpGet("ParaEdicao")] // <<-- NOVO ENDPOINT ADICIONADO AQUI -->>

    public async Task<IActionResult> ParaEdicao([FromQuery] Guid rodadaId, [FromQuery] Guid apostaRodadaId)
    {
      try
      {
        var result = await _apostaRodadaService.ObterApostasDoApostadorNaRodadaParaEdicao(rodadaId, apostaRodadaId);
        return CustomResponse(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao obter apostas para edição.");
        NotificarErro($"Erro interno ao obter apostas para edição: {ex.Message}");
        return CustomResponse<ApostaJogoEdicaoDto[]>();
      }
    }

    [AllowAnonymous]
    [HttpGet("Resultados")]
    public async Task<IActionResult> Resultados([FromQuery] Guid rodadaId, [FromQuery] Guid apostaRodadaId)
    {
      try
      {
        _logger.LogInformation("Chamando Resultados para RodadaId: {RodadaId} e ApostaRodadaId: {ApostaRodadaId}", rodadaId, apostaRodadaId);

        var result = await _apostaRodadaService.ObterResultadosDaRodada(rodadaId, apostaRodadaId);
        return CustomResponse(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao obter resultados da rodada.");
        NotificarErro($"Erro interno ao obter resultados da rodada: {ex.Message}");
        return CustomResponse<ApostaRodadaResultadosDto>();
      }
    }

    // <<-- NOVO ENDPOINT PARA CRIAR APOSTA AVULSA -->>
    /// <summary>
    /// Cria uma nova aposta avulsa para uma rodada e um apostador.
    /// Se o apostador ainda não tiver um relacionamento com o campeonato, ele será criado.
    /// </summary>
    /// <param name="request">O DTO de requisição com os dados da aposta.</param>
    /// <returns>ApiResponse com a ApostaRodadaDto criada.</returns>
    [HttpPost("CriarApostaAvulsa")]
    public async Task<IActionResult> CriarApostaAvulsa([FromBody] CriarApostaAvulsaRequestDto request)
    {
      try
      {
        // Converte as strings para Guid antes de passar para o serviço
        if (!Guid.TryParse(request.CampeonatoId, out Guid campeonatoId) ||
            !Guid.TryParse(request.RodadaId, out Guid rodadaId) ||
            !Guid.TryParse(request.ApostadorId, out Guid apostadorId))
        {
          NotificarErro("IDs de campeonato, rodada ou apostador inválidos.");
          return CustomResponse();
        }

        _logger.LogInformation("Chamando CriarApostaAvulsa para CampeonatoId: {CampeonatoId}, RodadaId: {RodadaId}, ApostadorId: {ApostadorId}", campeonatoId, rodadaId, apostadorId);

        var result = await _apostaRodadaService.ExecutarTransacaoApostaAvulsa(request);

        return CustomResponse(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao criar a aposta avulsa.");
        NotificarErro($"Erro interno ao criar a aposta avulsa: {ex.Message}");
        return CustomResponse<ApostaRodadaDto>();
      }
    }


    [HttpGet("ListarPorRodadaEApostadorCampeonato")]
    public async Task<IActionResult> ListarPorRodadaEApostadorCampeonato([FromQuery] Guid rodadaId, [FromQuery] Guid? apostadorCampeonatoId)
    {
      try
      {
        // 1. Obtemos o usuário logado com a navegação do Apostador incluída
        var usuario = await _usuarioService.GetLoggedInUser();

        if (usuario?.Apostador == null)
        {
          NotificarErro("Não foi possível identificar o apostador logado.");
          return CustomResponse();
        }
                
        var apostadorId = usuario.Apostador.Id;

        _logger.LogInformation("Buscando apostas para Rodada: {RodadaId}, Adesão: {ApostadorCampeonatoId}, Apostador: {ApostadorId}",
                                rodadaId, apostadorCampeonatoId, apostadorId);

        // 2. Chamada ao Service passando os 3 IDs necessários para a lógica de "Dono da Aposta"
        var result = await _apostaRodadaService.ObterApostasRodadaPorApostador(rodadaId, apostadorCampeonatoId, apostadorId);

        return CustomResponse(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao obter a lista de apostas do apostador.");
        NotificarErro($"Erro interno ao obter a lista de apostas: {ex.Message}");
        return CustomResponse<ApostaRodadaDto[]>();
      }
    }

    [AllowAnonymous]
    [HttpGet("ObterJogosComPalpites/{apostaId}/{rodadaId}")]
    public async Task<IActionResult> ObterJogosComPalpites(Guid apostaId, Guid rodadaId)
    {
      // Chamamos o AppService que, por sua vez, usa o Repository para buscar os dados
      var result = await _apostaRodadaService.ObterJogosComPalpites(apostaId, rodadaId);

      // CustomResponse garante que o Angular receba o objeto no padrão { success: true, data: [...] }
      return CustomResponse(result);
    }


    // --- NOVO ENDPOINT PARA OBTER TOTAIS DE APOSTAS AVULSAS ---
    /// <summary>
    /// Obtém o total de apostas e o valor acumulado de apostas avulsas e mistas de uma rodada.
    /// </summary>
    /// <param name="rodadaId">ID da rodada.</param>
    /// <returns>ApiResponse com o total de apostas e valor acumulado.</returns>
    /// 
    [AllowAnonymous]
    [HttpGet("totais-apostas-avulsas")]
    public async Task<IActionResult> ObterTotaisApostasAvulsas([FromQuery] Guid rodadaId)
    {
      try
      {
        var totais = await _apostaRodadaService.ObterTotaisApostasAvulsas(rodadaId);

        if (totais == null)
        {
          NotificarErro("Não foi possível encontrar os totais de apostas para a rodada especificada.");
          return CustomResponse();
        }

        return CustomResponse(new ApiResponse<ApostasAvulsasTotaisDto>(true, "Totais de apostas obtidos com sucesso.", totais));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao obter os totais de apostas avulsas.");
        NotificarErro($"Erro interno ao obter totais de apostas: {ex.Message}");
        return CustomResponse<ApostasAvulsasTotaisDto>();
      }
    }

    /*
    [AllowAnonymous]
    [HttpGet("totais-campeonato/{campeonatoId}")]
    public async Task<ActionResult<ApiResponse<ApostasCampeonatoTotaisDto>>> ObterTotaisCampeonato(Guid campeonatoId)
    {
      try
      {
        var totais = await _apostaRodadaService.ObterTotaisCampeonato(campeonatoId);

        if (totais == null)
        {
          NotificarErro("Não foi possível encontrar os totais do campeonato especificado.");
          return CustomResponse<ApostasCampeonatoTotaisDto>(); // Retorna um tipo vazio, mas com a mensagem de erro
        }

        return CustomResponse(new ApiResponse<ApostasCampeonatoTotaisDto>(true, "Totais de campeonato obtidos com sucesso.", totais));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao obter os totais do campeonato.");
        NotificarErro($"Erro interno ao obter totais do campeonato: {ex.Message}");
        return CustomResponse<ApostasCampeonatoTotaisDto>();
      }
    }
    */


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
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao salvar apostas.");
        NotificarErro($"Ocorreu um erro inesperado ao salvar as apostas: {ex.Message}");
        return CustomResponse<bool>();
      }

      return CustomResponse<bool>();

    }

  }

}
