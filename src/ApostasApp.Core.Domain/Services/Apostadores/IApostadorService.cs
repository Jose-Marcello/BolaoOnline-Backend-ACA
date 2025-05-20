using ApostasApp.Core.Domain.Models.Apostadores;

namespace ApostasApp.Core.Domain.Services.Apostadores
{
    public interface IApostadorService : IDisposable
    {
        Task Adicionar(Apostador apostador);
        Task Atualizar(Apostador apostador);
        Task Remover(Guid id);

    }
}
