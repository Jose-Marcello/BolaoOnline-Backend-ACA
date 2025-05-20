using DApostasApp.Core.Domain.Models.RankingRodadas;

namespace ApostasApp.Core.Domain.Services.RankingRodadas
{
    public interface IRankingRodadaService : IDisposable
    {
        Task Adicionar(RankingRodada rankingRodada);
        Task Atualizar(RankingRodada rankingRodada);
        Task Remover(Guid id);

    }
}
