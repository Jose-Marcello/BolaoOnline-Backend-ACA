using ApostasApp.Core.Domain.Models.Estadios;

namespace ApostasApp.Core.Domain.Interfaces.Estadios
{
    public interface IEstadioService : IDisposable
    {
        Task Adicionar(Estadio estadio);
        Task Atualizar(Estadio estadio);
        Task Remover(Guid id);

    }
}
