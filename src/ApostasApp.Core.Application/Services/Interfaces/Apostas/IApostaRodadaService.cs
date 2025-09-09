// Localização: ApostasApp.Core.Application/Services/Interfaces/Apostas/IApostaRodadaService.cs

using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Domain.Models.Apostas; // Para o modelo de domínio ApostaRodada
using ApostasApp.Core.Domain.Models.Campeonatos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Apostas
{
    public interface IApostaRodadaService
    {
        Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid apostadorCampeonatoId);
        Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rodadaId, Guid? apostadorCampeonatoId);
        Task<ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostaRodadaId);
        Task<ApiResponse<ApostaRodadaResultadosDto>> ObterResultadosDaRodada(Guid rodadaId, Guid apostaRodadaId);
        // Tipo de retorno corrigido para ApiResponse (sem DTO específico no Data)
        //Task<ApiResponse> SalvarApostas(SalvarApostaRequestDto request);
        Task<ApiResponse<ApostaRodadaDto>> SalvarApostas(SalvarApostaRequestDto salvarApostaDto);
        // Se você tiver o método GerarApostasRodadaParaTodosApostadores, mantenha-o aqui
        Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> GerarApostasRodadaParaTodosApostadores(string campeonatoIdString);
        // Se você tiver os métodos Adicionar, Atualizar, MarcarApostaRodadaComoSubmetida, mantenha-os aqui
        Task<ApiResponse> Adicionar(ApostaRodada apostaRodada); // Assumindo que ApostaRodada é o modelo de domínio
        Task<ApiResponse> Atualizar(ApostaRodada apostaRodada); // Assumindo que ApostaRodada é o modelo de domínio
        Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada apostaRodada); // Assumindo que ApostaRodada é o modelo de domínio
        Task<ApiResponse<ApostaRodadaDto>> ExecutarTransacaoApostaAvulsa(CriarApostaAvulsaRequestDto requestDto);
        Task<ApostasAvulsasTotaisDto> ObterTotaisApostasAvulsas(Guid rodadaId);
        Task<ApostasCampeonatoTotaisDto> ObterTotaisCampeonato(Guid campeonatoId);

    }
}
