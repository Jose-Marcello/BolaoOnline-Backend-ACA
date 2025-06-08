using ApostasApp.Core.Domain.Models.Apostadores;

namespace ApostasApp.Core.Application.Services.Interfaces.Apostadores
{
    public interface IApostadorService 
    {
        Task Adicionar(Apostador apostador);
        Task Atualizar(Apostador apostador);
        Task Remover(Guid id);

    }
}
