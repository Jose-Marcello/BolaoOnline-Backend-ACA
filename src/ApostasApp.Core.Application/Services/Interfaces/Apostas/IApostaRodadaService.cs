
using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Apostas;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Rodadas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Apostas
{
  public interface IApostaRodadaService
  {
    Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rodadaId, Guid? apostadorCampeonatoId, Guid apostadorId);
    Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid? apostadorCampeonatoId, Guid apostadorId);
    Task<ApiResponse<ApostaRodadaDto>> SalvarApostas(SalvarApostaRequestDto salvarApostaDto);
    Task<ApiResponse<ApostaRodadaDto>> ExecutarTransacaoApostaAvulsa(CriarApostaAvulsaRequestDto requestDto);
    // Método que estava faltando no contexto
    Task<ApiResponse<ApostaRodadaDto>> GerarApostaRodada(string apostadorCampeonatoId, string apostadorId, string rodadaId, bool ehApostaCampeonato, string identificador, Decimal custo);
    Task<ApostasAvulsasTotaisDto> ObterTotaisApostasAvulsas(Guid rodadaId);
    //Task<ApostasCampeonatoTotaisDto> ObterTotaisCampeonato(Guid campeonatoId);
    Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada apostaRodada);
    Task<ApiResponse<ApostaRodadaResultadosDto>> ObterResultadosDaRodada(Guid rodadaId, Guid apostaRodadaId);

    Task<ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostaRodadaId);

    Task<IEnumerable<JogoPalpiteResultado>> ObterJogosComPalpites(Guid apostaId, Guid rodadaId);  


  }
}
/*
using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.DTOs.Jogos;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Domain.Models.Apostas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Apostas
{
  public interface IApostaRodadaService
  {
    // Consultas
    Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rodadaId, Guid apostadorCampeonatoId);
    Task<IEnumerable<JogoPalpiteResultado>> ObterJogosComPalpites(Guid apostaId, Guid rodadaId);
    Task<ApostasAvulsasTotaisDto> ObterTotaisApostasAvulsas(Guid rodadaId);

    // Ações de Aposta
    Task<ApiResponse<ApostaRodadaDto>> SalvarApostas(SalvarApostaRequestDto salvarApostaDto);
    Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada apostaRodada);

    // Persistência Base
    Task<ApiResponse> Adicionar(ApostaRodada apostaRodada);
    Task<ApiResponse> Atualizar(ApostaRodada apostaRodada);

    Task<ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostaRodadaId);

    Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid apostadorCampeonatoId);
    
    Task<ApiResponse<ApostaRodadaResultadosDto>> ObterResultadosDaRodada(Guid rodadaId, Guid apostaRodadaId);

    Task<ApiResponse<ApostaRodadaDto>> ExecutarTransacaoApostaAvulsa(CriarApostaAvulsaRequestDto requestDto);

  }
}

*/
