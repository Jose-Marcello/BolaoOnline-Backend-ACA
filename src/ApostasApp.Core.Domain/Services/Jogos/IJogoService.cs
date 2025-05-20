using ApostasApp.Core.Domain.Models.Jogos;

namespace ApostasApp.Core.Domain.Services.Jogos
{
    public interface IJogoService : IDisposable
    {
        Task Adicionar(Jogo jogo);
        Task Atualizar(Jogo jogo);
        Task Remover(Guid id);

    }
}
