using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Jogos;

namespace ApostasApp.Core.Domain.Interfaces.Jogos
{
    public interface IJogoRepository : IRepository<Jogo>
    {
        Task<Jogo> ObterJogo(Guid id);

        Task<IEnumerable<Jogo>> ObterJogosRodada();

        Task<Jogo> ObterJogoRodada(Guid rodadaId);

        Task<IEnumerable<Jogo>> ObterJogosDaRodada(Guid rodadaId);

        Task<Jogo> ObterJogoEstadioEquipes(Guid id);

    }
}