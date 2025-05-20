using ApostasApp.Core.Domain.Models;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Services.Apostas;
using ApostasApp.Core.Domain.Validations;
using ApostasApp.Core.Domains.Models.Interfaces.Apostas;
using ApostasApp.Core.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace ApostasApp.Core.InfraStructure.Services.Apostas
{
    public class ApostaService : BaseService, IApostaService
    {
        private readonly IApostaRepository _apostaRepository;
        private readonly IUnitOfWork _uow;

        public ApostaService(IApostaRepository apostaRepository,
                            IUnitOfWork uow,
                                 INotificador notificador) : base(notificador)
        {
            _apostaRepository = apostaRepository;
            _uow = uow;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Aposta aposta)
        {

            try
            {
                if (!ExecutarValidacao(new ApostaValidation(), aposta))
                    return;

                if (_apostaRepository.Buscar(a => a.JogoId == aposta.JogoId
                  && a.ApostadorCampeonatoId == aposta.ApostadorCampeonatoId).Result.Any())
                {
                    Notificar("Já existe uma APOSTA deste APOSTADOR para este JOGO !!");
                    return;
                }

                var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
                await apostaRepository.Adicionar(aposta);

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao adicionar aposta (DbUpdateException): {ex.Message}");
                // Registre a exceção completa para depuração
                Console.WriteLine(ex);
                // Verifique se há erros de validação de dados ou problemas de integridade referencial
                // Notifique o usuário sobre o problema
                _uow.Rollback();
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"Erro ao adicionar aposta (ObjectDisposedException): {ex.Message}");
                // Registre a exceção completa para depuração
                Console.WriteLine(ex);
                // Verifique se o DbContext ou a UnitOfWork foram descartados prematuramente
                // Notifique o usuário sobre o problema
                _uow.Rollback();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar aposta: {ex.Message}");
                // Registre a exceção completa para depuração
                Console.WriteLine(ex);
                // Notifique o usuário sobre o problema
                _uow.Rollback();
            }
        }

        public async Task Atualizar(Aposta aposta)
        {
            if (!ExecutarValidacao(new ApostaValidation(), aposta)) return;

            /* if (_apostaRepository.Buscar(r => r.NumeroAposta == aposta.NumeroAposta
               && r.CampeonatoId == aposta.CampeonatoId).Result.Any())
             { 

                 Notificar("Já existe uma aposta neste campeonato com este NÚMERO informado.");
                 return;
              }*/
            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            await apostaRepository.Atualizar(aposta);
        }

        public async Task Remover(Guid id)
        {
            //aqui tem que verificar se esta RODADA já tem JOGOS associados
            /*var aposta = await _apostaRepository.obterJogosDaAposta(id);

            if (aposta.Any())
            {
                Notificar("O RODADA possui JOGOS associados ! Não pode ser excluída ");
                return;
            }*/

            var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            await apostaRepository.Remover(id);
        }

        public void Dispose()
        {
            //_apostaRepository?.Dispose();
        }
    }
}