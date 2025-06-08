using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Interfaces; // Para IRepository
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Domain.Interfaces.Jogos
{
    public interface IJogoRepository : IRepository<Jogo>
    {
        /// <summary>
        /// Obtém um jogo pelo seu ID, incluindo Rodada, Campeonato, Equipes (Casa e Visitante) e Estádio.
        /// Ideal para a tela de detalhes do jogo.
        /// </summary>
        /// <param name="id">O ID do jogo.</param>
        /// <returns>O objeto Jogo completo com as entidades relacionadas, ou null se não encontrado.</returns>
        Task<Jogo> ObterJogoComRodadaCampeonatoEquipesEstadio(Guid id); // Renomeado de ObterJogoRodada(Guid id)

        /// <summary>
        /// Obtém todos os jogos de uma rodada específica, incluindo Equipes (Casa e Visitante) e Estádio,
        /// ordenados por Data e Hora do Jogo.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma coleção de jogos da rodada.</returns>
        Task<IEnumerable<Jogo>> ObterJogosDaRodadaComEquipesEEstadio(Guid rodadaId); // Renomeado/Ajustado

        /// <summary>
        /// Obtém todos os jogos, incluindo Rodada e Campeonato, ordenados por Data e Hora do Jogo.
        /// </summary>
        /// <returns>Uma coleção de todos os jogos.</returns>
        Task<IEnumerable<Jogo>> ObterTodosJogosComRodadaECampeonato(); // Renomeado de ObterJogosRodada()
    }
}
