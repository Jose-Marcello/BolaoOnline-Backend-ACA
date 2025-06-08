using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Application.Services.Interfaces.Campeonatos
{
    public interface IEquipeCampeonatoService 
    {
        Task Adicionar(EquipeCampeonato equipeCampeonato);
        //Task Atualizar(EquipeCampeonato equipeCampeonato);
        Task Remover(EquipeCampeonato equipeCampeonato);

    }
}
