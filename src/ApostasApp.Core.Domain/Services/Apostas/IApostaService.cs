using ApostasApp.Core.Domain.Models.Apostas;

namespace ApostasApp.Core.Domain.Services.Apostas
{
    public interface IApostaService : IDisposable
    {
        Task Adicionar(Aposta aposta);
        Task Atualizar(Aposta aposta);
        Task Remover(Guid id);

    }
}
