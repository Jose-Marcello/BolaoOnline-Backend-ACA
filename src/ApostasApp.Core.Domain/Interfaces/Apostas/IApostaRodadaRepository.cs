using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;


namespace ApostasApp.Core.Domain.Interfaces.Apostas
{
    public interface IApostaRodadaRepository : IRepository<ApostaRodada>
    {
        Task<ApostaRodada> ObterApostaRodadaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId);
        // Outros métodos específicos para ApostaRodada

        Task<ApostaRodada> ObterUltimaApostaRodadaDoApostadorNaRodada(Guid apostadorCampeonatoId, Guid rodadaId);

        Task<ApostaRodada> ObterStatusApostaRodada(Guid rodadaId, Guid apostadorCampeonatoId);

        //Task<IEnumerable<(Guid ApostadorCampeonatoId, int TotalPontos)>> ObterRankingCampeonatoAsync(Guid campeonatoId);

        //Task<IEnumerable<ApostaRodada>> ObterApostasDoCampeonatoAsync();

        //sk<IEnumerable<RankingDataModel>> ObterDadosRankingCampeonatoAsync(Guid campeonatoId);

        Task<IEnumerable<IRankingResult>> ObterDadosRankingCampeonatoAsync(Guid campeonatoId);

        /// <summary>
        /// Obtém o total de apostas e o valor acumulado de apostas avulsas e mistas de uma rodada.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Um DTO contendo o número total de apostas e o valor acumulado.</returns>
        Task<ApostasTotais> ObterTotaisApostasAvulsas(Guid rodadaId);


        Task<CampeonatoTotais> ObterTotaisCampeonato(Guid campeonatoId);


    }

}