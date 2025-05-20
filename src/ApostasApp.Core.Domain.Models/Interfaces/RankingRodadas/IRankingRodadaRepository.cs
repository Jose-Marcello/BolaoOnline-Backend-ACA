using ApostasApp.Core.Domain.Models.Interfaces;
using DApostasApp.Core.Domain.Models.RankingRodadas;

namespace ApostasApp.Core.Domain.Interfaces.RankingRodadas
{
    public interface IRankingRodadaRepository : IRepository<RankingRodada>
    {
        Task<RankingRodada> ObterRankingRodada(Guid id);

        Task<IEnumerable<RankingRodada>> ObterRankingDaRodada(Guid idRodada);

        Task<RankingRodada> ObterRankingDoApostadorNaRodada(Guid idRodada, Guid idApostador);


    }
}