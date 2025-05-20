using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Domain.Interfaces.Campeonatos
{
    public interface IApostadorCampeonatoService : IDisposable
    {
        Task Adicionar(ApostadorCampeonato apostadorCampeonato);

        //task RemoverEntity(ApostadorCampeonato apostadorCampeonato);
        Task Remover(Guid Id);

    }
}
