using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Domain.Models.Interfaces.Campeonatos
{
    public interface IEquipeCampeonatoRepository : IRepository<EquipeCampeonato>
    {
        Task<EquipeCampeonato> ObterEquipeCampeonato(Guid idCampeonato, Guid idEquipe);
        Task<IEnumerable<EquipeCampeonato>> ObterEquipesDoCampeonato(Guid id);

    }
}