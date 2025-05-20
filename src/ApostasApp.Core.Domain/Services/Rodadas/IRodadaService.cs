using ApostasApp.Core.Domain.Models.Rodadas;

namespace ApostasApp.Core.Domain.Services.Rodadas
{
   public interface IRodadaService : IDisposable
    {
        Task Adicionar(Rodada rodada);
        Task Atualizar(Rodada rodada);
        Task Remover(Guid id);

    }
}
