using ApostasApp.Core.Domain.Models.RankingRodadas;
using ApostasApp.Core.Domain.Interfaces;

namespace ApostasApp.Core.Domain.Interfaces.RankingRodadas
{
    public interface IRankingRodadaRepository : IRepository<RankingRodada>
    {
        Task<RankingRodada> ObterRankingRodada(Guid id);

        Task<IEnumerable<RankingRodada>> ObterRankingDaRodada(Guid idRodada);

        Task<RankingRodada> ObterRankingDoApostadorNaRodada(Guid idRodada, Guid idApostador);


    }
}