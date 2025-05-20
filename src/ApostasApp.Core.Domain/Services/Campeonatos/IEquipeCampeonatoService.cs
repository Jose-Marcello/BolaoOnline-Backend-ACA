using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Domain.Interfaces.Campeonatos
{
    public interface IEquipeCampeonatoService : IDisposable
    {
        Task Adicionar(EquipeCampeonato equipeCampeonato);
        //Task Atualizar(EquipeCampeonato equipeCampeonato);
        Task RemoverEntity(EquipeCampeonato equipeCampeonato);

    }
}
