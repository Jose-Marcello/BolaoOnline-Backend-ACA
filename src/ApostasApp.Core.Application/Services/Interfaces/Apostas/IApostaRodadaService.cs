// ApostasApp.Core.Application.Services/Interfaces/Apostas/IApostaRodadaService.cs

using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Domain.Models.Apostas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Apostas
{
    public interface IApostaRodadaService
    {
        Task<ApiResponse<ApostaRodadaDto>> GerarApostaRodadaInicial(
            string apostadorCampeonatoIdString,
            string rodadaIdString,
            bool ehApostaCampeonato,
            string identificadorAposta = null);

        Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> GerarApostasRodadaParaTodosApostadores(string campeonatoIdString);

        Task<ApiResponse> Adicionar(ApostaRodada apostaRodada);
        Task<ApiResponse> Atualizar(ApostaRodada apostaRodada);
        Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada apostaRodada);

        Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid apostadorCampeonatoId);

        Task<ApiResponse<IEnumerable<ApostaJogoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostadorCampeonatoId);
        Task<ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>> ObterApostasDoApostadorNaRodadaParaVisualizacao(Guid rodadaId, Guid apostadorCampeonatoId);
        Task<ApiResponse> SalvarApostas(SalvarApostaRequestDto salvarApostaDto);
    }
}
