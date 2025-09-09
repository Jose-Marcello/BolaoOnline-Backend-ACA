// Localização: ApostasApp.Core.Application.Services.Interfaces/IRankingService.cs

using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.DTOs.Ranking;
using ApostasApp.Core.Application.DTOs.RankingRodadas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces
{
    public interface IRankingService
    {
        // O método de ranking da rodada já estava correto
        Task<ApiResponse<IEnumerable<RankingRodadaDto>>> ObterRankingDaRodada(Guid rodadaId);

        // CORREÇÃO AQUI: O tipo de retorno deve ser um ApiResponse
        Task<ApiResponse<IEnumerable<RankingDto>>> ObterRankingCampeonatoAsync(Guid campeonatoId);
    }
}