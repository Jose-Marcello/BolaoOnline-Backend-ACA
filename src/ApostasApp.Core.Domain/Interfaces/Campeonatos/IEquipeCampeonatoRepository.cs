using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Interfaces;

namespace ApostasApp.Core.Domain.Interfaces.Campeonatos
{
    public interface IEquipeCampeonatoRepository : IRepository<EquipeCampeonato>
    {
        Task<EquipeCampeonato> ObterEquipeCampeonato(Guid idCampeonato, Guid idEquipe);
        Task<IEnumerable<EquipeCampeonato>> ObterEquipesDoCampeonato(Guid id);

    }
}