using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Validations;
using ApostasApp.Core.InfraStructure;

namespace ApostasApp.Core.Application.Services.Campeonatos
{
    public class CampeonatoService : BaseService, ICampeonatoService
    {
        private readonly ICampeonatoRepository _campeonatoRepository;

        public CampeonatoService(ICampeonatoRepository campeonatoRepository,
                                 INotificador notificador) : base(notificador)
        {
            _campeonatoRepository = campeonatoRepository;
        }

        public async Task Adicionar(Campeonato campeonato)
        {
            if (!ExecutarValidacao(new CampeonatoValidation(), campeonato))
                return;

            if (_campeonatoRepository.Buscar(f => f.Nome == campeonato.Nome).Result.Any())
            {
                Notificar("Já existe um campeonato com este NOME infomado.");
                return;
            }

            await _campeonatoRepository.Adicionar(campeonato);
        }

        public async Task Atualizar(Campeonato campeonato)
        {
            if (!ExecutarValidacao(new CampeonatoValidation(), campeonato)) return;

            if (_campeonatoRepository.Buscar(f => f.Nome == campeonato.Nome && f.Id != campeonato.Id).Result.Any())
            {
                Notificar("Já existe um campeonato com este NOME informado.");
                return;
            }

            await _campeonatoRepository.Atualizar(campeonato);
        }

        public async Task Remover(Guid id)
        {
            //var campeonato = await _campeonatoRepository.ObterRodadasDoCampeonato(id);

            //if (campeonato.Rodadas.Any())
            //{
            //    Notificar("O campeonato possui RODADAS associadas !");
            //    return;
            //}

            await _campeonatoRepository.Remover(id);
        }

        public void Dispose()
        {
            //_campeonatoRepository?.Dispose();
        }
    }
}