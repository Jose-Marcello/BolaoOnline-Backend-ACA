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

    [AllowAnonymous]
    [HttpGet("{apostadorCampeonatoId}/RodadasEmApostas")]
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
        NotificarAlerta("No momento NÃO HÁ uma RODADA em APOSTAS para este campeonato...");
        return CustomResponse<object>();
      }

      var rodada = rodadaResponse.Data;

      var usuario = await _usuarioService.GetLoggedInUser();
      if (usuario == null)
      {
        NotificarErro("Usuário não logado ou sessão expirada.");
        return CustomResponse<object>();
      }

      // AJUSTE: Passando o ID do apostador logado (extraído do usuário)
      // Aqui assumimos que o seu DTO de usuário ou objeto de domínio tem o ApostadorId
      var apostaRodadaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(
          rodada.Id,
          apostadorCampeonatoId,
          usuario.Apostador.Id); // <--- OBRIGATÓRIO AGORA

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

    // AJUSTE NO MÉTODO DE STATUS (Muito importante para o Jeff_Bolinha)
    [AllowAnonymous]
    [HttpGet("StatusApostaDaRodada")]
    public async Task<IActionResult> BuscarStatusEDataHoraApostaDaRodada(
        [FromQuery] Guid? apostadorCampeonatoId, // Tornado opcional
        [FromQuery] Guid rodadaId)
    {
      try
      {
        var usuario = await _usuarioService.GetLoggedInUser();
        if (usuario == null) return Unauthorized();

        // Passamos acId (opcional) e o apId (obrigatório)
        var apostaStatusResponse = await _apostaRodadaService.ObterStatusApostaRodadaParaUsuario(
            rodadaId,
            apostadorCampeonatoId,
            usuario.Apostador.Id);

        var apostaStatus = apostaStatusResponse.Data;

        if (apostaStatus != null)
        {
          return CustomResponse(new
          {
            enviada = apostaStatus.Enviada,
            dataHoraAposta = apostaStatus.DataHoraSubmissao?.ToString("o")
          });
        }
        return CustomResponse(new { enviada = false, dataHoraAposta = (string)null });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Erro ao buscar status.");
        return CustomResponse<object>();
      }
    }

    // NOTA: Os métodos de "BuscarApostasParaEdicao" e "BuscarApostasParaVisualizacao" 
    // precisam que você decida se passará o ApostadorId via Rota ou extrairá do Token.
    // Se o Jeff for usar esses métodos, a Rota precisa aceitar o ApostadorId.
  }
}
