using ApostasApp.Core.Domain.Models.Estadios;

namespace ApostasApp.Core.Domain.Models.Interfaces.Estadios
{
    public interface IEstadioRepository : IRepository<Estadio>
    {
        Task<Estadio> ObterEstadio(Guid id);

        Task<IEnumerable<Estadio>> ObterEstadiosUf();
        Task<IEnumerable<Estadio>> ObterEstadiosEmOrdemAlfabetica();



    }
}