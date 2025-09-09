using ApostasApp.Core.Domain.Models.Estadios;
using ApostasApp.Core.Domain.Interfaces;

namespace ApostasApp.Core.Domain.Interfaces.Estadios
{
    public interface IEstadioRepository : IRepository<Estadio>
    {
        Task<Estadio> ObterEstadio(Guid id);

        Task<IEnumerable<Estadio>> ObterEstadiosUf();
        Task<IEnumerable<Estadio>> ObterEstadiosEmOrdemAlfabetica();



    }
}