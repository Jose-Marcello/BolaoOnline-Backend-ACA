using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Models.Interfaces;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Services.Jogos;
using ApostasApp.Core.Domain.Validations;

namespace ApostasApp.Core.InfraStructure.Services.Jogos
{
    public class JogoService : BaseService, IJogoService
    {
        private readonly IJogoRepository _jogoRepository;


        public JogoService(IJogoRepository jogoRepository,
                                 INotificador notificador) : base(notificador)
        {
            _jogoRepository = jogoRepository;
        }

        //tem um problema na APRESENTAÇÃO DA NOTIFICAÇÃO DO ERRO (PESQUISAR)
        public async Task Adicionar(Jogo jogo)
        {
            if (!ExecutarValidacao(new JogoValidation(), jogo))
                return;

            if (_jogoRepository.Buscar(j => j.EquipeCasaId == jogo.EquipeCasaId
              && j.EquipeVisitanteId == jogo.EquipeVisitanteId).Result.Any())
            {
                Notificar("Este JOGO já está cadastrado.");
                return;
            }


            await _jogoRepository.Adicionar(jogo);
        }

        public async Task Atualizar(Jogo jogo)
        {
            if (!ExecutarValidacao(new JogoValidation(), jogo)) return;

            //aqui também tem que ter a validação da inclusão 

            await _jogoRepository.Atualizar(jogo);
        }

        public async Task Remover(Guid id)
        {
            //ver o que entra aqui - Não pode excluir um JOGO que tem APOSTAS ..


            /*var rodada = await _jogoRepository.ObterJogosDaRodada(id);

            if (rodada.Any())
            {
                Notificar("O RODADA possui JOGOS associados ! Não pode ser excluída ");
                return;
            }
*/
            await _jogoRepository.Remover(id);
        }

        public void Dispose()
        {
            //_jogoRepository?.Dispose();
        }
    }
}