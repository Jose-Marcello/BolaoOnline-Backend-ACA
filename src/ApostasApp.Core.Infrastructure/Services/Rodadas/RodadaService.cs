
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.Domain.Services.Rodadas;
using ApostasApp.Core.Domain.Validations;
using ApostasApp.Core.Domains.Models.Interfaces.Apostas;
using ApostasApp.Core.InfraStructure;

namespace ApostasApp.Application.Services.Rodadas
{
    public class RodadaService : BaseService, IRodadaService
    {
        private readonly IRodadaRepository _rodadaRepository;
        private readonly IApostaRepository _apostaRepository;


        public RodadaService(IRodadaRepository rodadaRepository,
                             IApostaRepository apostaRepository,
                                 INotificador notificador) : base(notificador)
        {
            _rodadaRepository = rodadaRepository;
            _apostaRepository = apostaRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Rodada rodada)
        {
            if (!ExecutarValidacao(new RodadaValidation(), rodada))
                return;

            if (_rodadaRepository.Buscar(r => r.NumeroRodada == rodada.NumeroRodada
              && r.CampeonatoId == rodada.CampeonatoId).Result.Any())
            {
                Notificar("Já existe uma RODADA neste CAMPEONATO com este NÚMERO infomado.");
                return;
            }

            await _rodadaRepository.Adicionar(rodada);
        }

        public async Task Atualizar(Rodada rodada)
        {
            if (!ExecutarValidacao(new RodadaValidation(), rodada)) return;

            /* if (_rodadaRepository.Buscar(r => r.NumeroRodada == rodada.NumeroRodada
               && r.CampeonatoId == rodada.CampeonatoId).Result.Any())
             { 

                 Notificar("Já existe uma rodada neste campeonato com este NÚMERO informado.");
                 return;
              }*/

            var apostasGeradas = await _apostaRepository.ObterApostasDaRodada(rodada.Id); // Sua implementação

            if (rodada.Status == StatusRodada.EmApostas && apostasGeradas.Count() == 0)
            {
                Notificar("Essa rodada não pode ser colocada EM APOSTAS !! Ainda não existe APOSTAS GERADAS para esta RODADA !!");
                return;
            }      

            await _rodadaRepository.Atualizar(rodada);
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

            await _rodadaRepository.Remover(id);
        }

        public void Dispose()
        {
            // _rodadaRepository?.Dispose();
        }
    }
}