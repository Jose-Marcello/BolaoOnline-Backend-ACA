using ApostasApp.Core.Domain.Models.Apostas;


namespace ApostasApp.Core.Domain.Interfaces.Apostas
{
    public interface IPalpiteRepository : IRepository<Palpite>
    {
        // Obtém todos os palpites para uma rodada específica, incluindo o jogo e o apostador
        Task<IEnumerable<Palpite>> ObterPalpitesDaRodada(Guid rodadaId);

        // Novo método para remover todos os palpites associados a uma rodada
        Task<bool> RemoverTodosPalpitesDaRodada(Guid rodadaId);
    }
}
