using ApostasApp.Core.Domain.Interfaces.RankingRodadas;
using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Services.RankingRodadas;
using ApostasApp.Core.Domain.Validations;
using ApostasApp.Core.InfraStructure;
using ApostasApp.Infrastructure.Data.Repository;
using DApostasApp.Core.Domain.Models.RankingRodadas;

namespace ApostasApp.Application.Services.Rodadas
{
    public class RankingRodadaService : BaseService, IRankingRodadaService
    {
        private readonly IRankingRodadaRepository _rankingRodadaRepository;
        private readonly IUnitOfWork _uow;


        public RankingRodadaService(IRankingRodadaRepository rankingRodadaRepository,
                                    IUnitOfWork uow,
                                    INotificador notificador) : base(notificador)
        {
            _rankingRodadaRepository = rankingRodadaRepository;
            _uow = uow;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(RankingRodada rankingRodada)
        {
            if (!ExecutarValidacao(new RankingRodadaValidation(), rankingRodada))
                return;

            /*if (_rodadaRepository.Buscar(r => r.NumeroRodada == rodada.NumeroRodada 
              && r.CampeonatoId == rodada.CampeonatoId).Result.Any())
            {
                Notificar("Já existe uma RODADA neste CAMPEONATO com este NÚMERO infomado.");
                return;
            }*/

            var rankingRodadaRepository = _uow.GetRepository<RankingRodada>() as RankingRodadaRepository;
            await rankingRodadaRepository.Adicionar(rankingRodada);
        }

        public async Task Atualizar(RankingRodada rankingRodada)
        {
            if (!ExecutarValidacao(new RankingRodadaValidation(), rankingRodada)) return;

            /* if (_rodadaRepository.Buscar(r => r.NumeroRodada == rodada.NumeroRodada
               && r.CampeonatoId == rodada.CampeonatoId).Result.Any())
             { 

                 Notificar("Já existe uma rodada neste campeonato com este NÚMERO informado.");
                 return;
              }*/

            var rankingRodadaRepository = _uow.GetRepository<RankingRodada>() as RankingRodadaRepository;
            await rankingRodadaRepository.Atualizar(rankingRodada);
        }

        public async Task Remover(Guid id)
        {
            //aqui tem que verificar se esta RODADA já tem JOGOS associados
            /*var rodada = await _rodadaRepository.obterJogosDaRodada(id);

            if (rodada.Any())
            {
                Notificar("O RODADA possui JOGOS associados ! Não pode ser excluída ");
                return;
            }*/

            var rankingRodadaRepository = _uow.GetRepository<RankingRodada>() as RankingRodadaRepository;
            await rankingRodadaRepository.Remover(id);
        }

        public void Dispose()
        {
            // _rankingRodadaRepository?.Dispose();
        }
    }
}