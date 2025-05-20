using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Validations;

namespace ApostasApp.Core.InfraStructure.Services.Campeonatos
{
    public class ApostadorCampeonatoService : BaseService, IApostadorCampeonatoService
    {
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;


        public ApostadorCampeonatoService(IApostadorCampeonatoRepository apostadorCampeonatoRepository,
                                 INotificador notificador) : base(notificador)
        {
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(ApostadorCampeonato apostadorCampeonato)
        {
            if (!ExecutarValidacao(new ApostadorCampeonatoValidation(), apostadorCampeonato))
                return;

            if (_apostadorCampeonatoRepository.Buscar(ec => ec.CampeonatoId == apostadorCampeonato.CampeonatoId
              && ec.ApostadorId == apostadorCampeonato.ApostadorId).Result.Any())
            {
                Notificar("Este APOSTADOR já foi associado no CAMPEONATO !!");
                return;
            }

            await _apostadorCampeonatoRepository.Adicionar(apostadorCampeonato);
        }

        public async Task RemoverEntity(ApostadorCampeonato apostadorCampeonato)
        {

            await _apostadorCampeonatoRepository.RemoverEntity(apostadorCampeonato);

        }

        public async Task Remover(Guid Id)
        {

            await _apostadorCampeonatoRepository.Remover(Id);

        }


        public void Dispose()
        {
            //_apostadorCampeonatoRepository?.Dispose();
        }
    }
}