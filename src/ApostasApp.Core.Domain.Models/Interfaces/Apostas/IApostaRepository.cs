using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Interfaces;

namespace ApostasApp.Core.Domains.Models.Interfaces.Apostas
{
    public interface IApostaRepository : IRepository<Aposta>
    {
        Task<Aposta> ObterAposta(Guid id);

        //Task<IEnumerable<Aposta>> ObterApostasJogoApostador();
        //Task<IEnumerable<Aposta>> ObterApostasJogoApostadorNaRodada(Guid rodadaId);


        Task<IEnumerable<Aposta>> ObterApostasDoJogo(Guid jogoId);

        Task<IEnumerable<Aposta>> ObterApostasDaRodada(Guid rodadaId);

        Task<List<Guid>> ObterApostadoresDaRodada(Guid rodadaId);

        Task<IEnumerable<Aposta>> ObterApostasDoApostador(Guid apostadorId);

        Task<IEnumerable<Aposta>> ObterApostasDoApostadorNaRodada(Guid rodadaId, Guid apostadorId);
        
        Task<Aposta> ObterStatusApostasDoApostadorNaRodada(Guid rodadaId, Guid apostadorId);

        Task<Aposta> ObterApostaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorId);

    }
}