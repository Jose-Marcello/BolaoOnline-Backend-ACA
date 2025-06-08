using ApostasApp.Core.Domain.Models.RankingRodadas;

namespace ApostasApp.Core.Application.Services.Interfaces.RankingRodadas
{
    public interface IRankingRodadaService
    {
        Task Adicionar(RankingRodada rankingRodada);
        Task Atualizar(RankingRodada rankingRodada);
        Task Remover(Guid id);

    }
}
