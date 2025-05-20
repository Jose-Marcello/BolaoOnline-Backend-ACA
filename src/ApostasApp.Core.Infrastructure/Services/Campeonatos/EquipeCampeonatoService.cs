using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Validations;
using ApostasApp.Core.InfraStructure;

namespace ApostasApp.Core.Infrastructure.Services.Campeonatos
{
    public class EquipeCampeonatoService : BaseService, IEquipeCampeonatoService
    {
        private readonly IEquipeCampeonatoRepository _equipeCampeonatoRepository;


        public EquipeCampeonatoService(IEquipeCampeonatoRepository equipeCampeonatoRepository,
                                 INotificador notificador) : base(notificador)
        {
            _equipeCampeonatoRepository = equipeCampeonatoRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(EquipeCampeonato equipeCampeonato)
        {
            if (!ExecutarValidacao(new EquipeCampeonatoValidation(), equipeCampeonato))
                return;

            if (_equipeCampeonatoRepository.Buscar(ec => ec.CampeonatoId == equipeCampeonato.CampeonatoId
              && ec.EquipeId == equipeCampeonato.EquipeId).Result.Any())
            {
                Notificar("Esta EQUIPE já foi associada no CAMPEONATO !!");
                return;
            }

            await _equipeCampeonatoRepository.Adicionar(equipeCampeonato);
        }

        public async Task RemoverEntity(EquipeCampeonato equipeCampeonato)
        {

            await _equipeCampeonatoRepository.RemoverEntity(equipeCampeonato);

        }

        public void Dispose()
        {
            // _equipeCampeonatoRepository?.Dispose();
        }
    }
}