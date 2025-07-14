using ApostasApp.Core.Application.Services.Interfaces; // Para BaseService
using ApostasApp.Core.Application.Services.Interfaces.RankingRodadas; // Para IRankingRodadaService
using ApostasApp.Core.Application.Validations; // Para RankingRodadaValidation (se for usada aqui)
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Interfaces.RankingRodadas; // Para IRankingRodadaRepository
using ApostasApp.Core.Domain.Models.RankingRodadas; // Para RankingRodada
using System;
using System.Collections.Generic; // Se necessário para métodos de retorno de coleção
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Rodadas
{
    /// <summary>
    /// RankingRodadaService é responsável pela lógica de negócio relacionada a rankings de rodada.
    /// Ele herda de BaseService, que gerencia o IUnitOfWork e o INotificador.
    /// </summary>
    public class RankingRodadaService : BaseService, IRankingRodadaService
    {
        private readonly IRankingRodadaRepository _rankingRodadaRepository;
        // private readonly IUnitOfWork _uow; // REMOVIDO: Gerenciado pela BaseService

        // O construtor agora injeta INotificador e IUnitOfWork e os passa para a BaseService
        public RankingRodadaService(IRankingRodadaRepository rankingRodadaRepository,
                                    IUnitOfWork uow, // UoW injetado
                                    INotificador notificador) : base(notificador, uow) // Passando notificador e uow para o construtor da BaseService
        {
            _rankingRodadaRepository = rankingRodadaRepository;
            // _uow = uow; // REMOVIDO: Não é mais necessário atribuir aqui
        }

        /// <summary>
        /// Adiciona um novo registro de Ranking de Rodada.
        /// </summary>
        /// <param name="rankingRodada">A entidade RankingRodada a ser adicionada.</param>
        public async Task Adicionar(RankingRodada rankingRodada)
        {
            // Assumindo que ExecutarValidacao está na BaseService
            //if (!ExecutarValidacao(new RankingRodadaValidation(), rankingRodada))
            //    return;

            // Lógica de negócio comentada no original, mantida comentada aqui.
            /*if (_rodadaRepository.Buscar(r => r.NumeroRodada == rodada.NumeroRodada
              && r.CampeonatoId == rodada.CampeonatoId).Result.Any())
            {
                Notificar("Alerta", "Já existe uma RODADA neste CAMPEONATO com este NÚMERO infomado.");
                return;
            }*/

            await _rankingRodadaRepository.Adicionar(rankingRodada);
            await CommitAsync(); // Chamar Commit() da BaseService para persistir as alterações
            Notificar("Sucesso", "Ranking de Rodada adicionado com sucesso!");
        }

        /// <summary>
        /// Atualiza um registro de Ranking de Rodada existente.
        /// </summary>
        /// <param name="rankingRodada">A entidade RankingRodada a ser atualizada.</param>
        public async Task Atualizar(RankingRodada rankingRodada)
        {
            // Assumindo que ExecutarValidacao está na BaseService
            //if (!ExecutarValidacao(new RankingRodadaValidation(), rankingRodada)) return;

            // Lógica de negócio comentada no original, mantida comentada aqui.
            /* if (_rodadaRepository.Buscar(r => r.NumeroRodada == rodada.NumeroRodada
               && r.CampeonatoId == rodada.CampeonatoId).Result.Any())
             {
                 Notificar("Alerta", "Já existe uma rodada neste campeonato com este NÚMERO informado.");
                 return;
             }*/

            await _rankingRodadaRepository.Atualizar(rankingRodada);
            await CommitAsync(); // Chamar Commit() da BaseService para persistir as alterações
            Notificar("Sucesso", "Ranking de Rodada atualizado com sucesso!");
        }

        /// <summary>
        /// Remove um registro de Ranking de Rodada pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do registro de ranking a ser removido.</param>
        public async Task Remover(Guid id)
        {
            var rankingRodada = await _rankingRodadaRepository.ObterPorId(id);
            if (rankingRodada == null)
            {
                Notificar("Alerta", "Registro de Ranking de Rodada não encontrado para remoção.");
                return;
            }

            // Lógica de negócio comentada no original, mantida comentada aqui.
            // Aqui você deve verificar se a remoção é permitida com base em regras de negócio.
            // Ex: "aqui tem que verificar se esta RODADA já tem JOGOS associados"
            /*var rodada = await _rodadaRepository.obterJogosDaRodada(id);
            if (rodada.Any())
            {
                Notificar("Alerta", "O RODADA possui JOGOS associados! Não pode ser excluída.");
                return;
            }*/

            await _rankingRodadaRepository.Remover(rankingRodada); // Usar Remover(TEntity entity)
            await CommitAsync(); // Chamar Commit() da BaseService para persistir as alterações
            Notificar("Sucesso", "Registro de Ranking de Rodada removido com sucesso!");
        }

        /// <summary>
        /// Obtém um registro de Ranking de Rodada pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do registro de ranking.</param>
        /// <returns>O registro de Ranking de Rodada encontrado, ou null se não existir.</returns>
        public async Task<RankingRodada> ObterPorId(Guid id)
        {
            return await _rankingRodadaRepository.ObterPorId(id);
        }

        /// <summary>
        /// Obtém todos os registros de Ranking de Rodada.
        /// </summary>
        /// <returns>Uma coleção de registros de Ranking de Rodada.</returns>
        public async Task<IEnumerable<RankingRodada>> ObterTodos()
        {
            return await _rankingRodadaRepository.ObterTodos();
        }

        Task IRankingRodadaService.Remover(Guid id)
        {
            throw new NotImplementedException();
        }

        // Outros métodos de RankingRodadaService...
    }
}
