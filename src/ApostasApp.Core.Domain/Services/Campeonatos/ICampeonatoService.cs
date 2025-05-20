using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Domain.Interfaces.Campeonatos
{
    public interface ICampeonatoService : IDisposable
    {
        Task Adicionar(Campeonato campeonato);
        Task Atualizar(Campeonato campeonato);
        Task Remover(Guid id);

    }
}
