using ApostasApp.Core.Domain.Interfaces; // Para IRepository
using ApostasApp.Core.Domain.Models.Jogos; // Para Jogo (se Jogo for uma entidade separada)
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada
using System;
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks;

namespace ApostasApp.Core.Domain.Models.Interfaces.Rodadas
{
    public interface IRodadaRepository : IRepository<Rodada>
    {
        /// <summary>
        /// Obtém a rodada que está atualmente no status 'Em Apostas' para um campeonato específico,
        /// incluindo o campeonato associado.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>A rodada em apostas, ou null se não houver.</returns>
        Task<Rodada> ObterRodadaEmApostasPorCampeonato(Guid campeonatoId);

        /// <summary>
        /// Obtém a rodada que está atualmente no status 'Corrente' para um campeonato específico,
        /// incluindo o campeonato associado.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>A rodada corrente, ou null se não houver.</returns>
        Task<Rodada> ObterRodadaCorrentePorCampeonato(Guid campeonatoId);

        //Obtém a lista de Rodadas Correntes de um campeonato
        Task<IEnumerable<Rodada>> ObterRodadasCorrentePorCampeonato(Guid campeonatoId);

        //Obtém a lista de Rodadas em Aposta de um campeonato
        Task<IEnumerable<Rodada>> ObterRodadasEmApostaPorCampeonato(Guid campeonatoId);

        /// <summary>
        /// Obtém uma rodada específica, incluindo seus jogos e as equipes associadas a esses jogos.
        /// Essencial para exibir a interface de apostas.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>A Rodada com seus Jogos e Equipes, ou null se não encontrada.</returns>
        Task<Rodada> ObterRodadaComJogosEEquipes(Guid rodadaId);

        /// <summary>
        /// Obtém os jogos de uma rodada específica, incluindo os placares reais e as equipes associadas.
        /// Essencial para visualização de resultados e cálculo de pontuação.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma coleção de Jogos da rodada com placares e equipes.</returns>
        Task<IEnumerable<Jogo>> ObterJogosDaRodadaComPlacaresEEquipes(Guid rodadaId); // NOVO MÉTODO

        /// <summary>
        /// Obtém todas as rodadas finalizadas para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma coleção de rodadas finalizadas.</returns>
        Task<IEnumerable<Rodada>> ObterRodadasFinalizadasPorCampeonato(Guid campeonatoId);

        /// <summary>
        /// Obtém rodadas com ranking (corrente ou finalizada) para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma coleção de rodadas com ranking.</returns>
        Task<IEnumerable<Rodada>> ObterRodadasComRankingPorCampeonato(Guid campeonatoId);

        /// <summary>
        /// Obtém todas as rodadas para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma coleção de todas as rodadas do campeonato.</returns>
        Task<IEnumerable<Rodada>> ObterTodasAsRodadasDoCampeonato(Guid campeonatoId);
    }
}
