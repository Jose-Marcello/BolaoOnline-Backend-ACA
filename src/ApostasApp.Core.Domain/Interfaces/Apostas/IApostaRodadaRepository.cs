using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Interfaces;

namespace ApostasApp.Core.Domain.Interfaces.Apostas
{
    public interface IApostaRodadaRepository : IRepository<ApostaRodada>
    {

        Task<ApostaRodada> ObterApostaRodadaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId);
        // Outros métodos específicos para ApostaRodada

        Task<ApostaRodada> ObterUltimaApostaRodadaDoApostadorNaRodada(Guid apostadorCampeonatoId, Guid rodadaId);
    }

}