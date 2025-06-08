using ApostasApp.Core.Domain.Models.RankingRodadas;
using ApostasApp.Core.Domain.Models.Rodadas;
using System;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Rodadas
{
    public interface IRodadaService
    {
        // Métodos existentes...
        //Task Atualizar(RankingRodada rankingRodada);

        Task<Rodada> ObterRodadaPorId(Guid rodadaId);
        
        Task<Rodada> ObterRodadaEmApostasPorCampeonato(Guid campeonatoId);

        Task<Rodada> ObterRodadaCorrentePorCampeonato(Guid campeonatoId);
              
    }
}
