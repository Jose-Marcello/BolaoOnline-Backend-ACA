using ApostasApp.Core.Domain.Models.Rodadas;

namespace ApostasApp.Core.Domain.Models.Interfaces.Rodadas
{
    public interface IRodadaRepository : IRepository<Rodada>
    {
        Task<Rodada> ObterRodada(Guid id);

        //Task<Rodada> ObterRodadaAtiva();

        Task<Rodada> ObterRodadaEmApostas();

        Task<IEnumerable<Rodada>> ObterListaComRodadaCorrente();
        Task<Rodada> ObterRodadaEmConstrucao();

        Task<IEnumerable<Rodada>> ObterRodadasCampeonato();

        Task<Rodada> ObterRodadaCampeonato(Guid campeonatoId);

        Task<IEnumerable<Rodada>> ObterRodadasDoCampeonato(Guid campeonatoId);
        Task<IEnumerable<Rodada>> ObterRodadasComRanking(Guid campeonatoId);
                

    }
}