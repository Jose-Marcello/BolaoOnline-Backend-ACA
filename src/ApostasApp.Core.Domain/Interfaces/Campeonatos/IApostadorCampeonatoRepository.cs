using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Interfaces;

namespace ApostasApp.Core.Domain.Interfaces.Campeonatos
{
    public interface IApostadorCampeonatoRepository : IRepository<ApostadorCampeonato>
    {
        Task<ApostadorCampeonato> ObterApostadorCampeonato(Guid id);
        Task<ApostadorCampeonato> ObterApostadorDoCampeonato(Guid idCampeonato, Guid idApostador);
        Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresComRanking(Guid campeonatoId);        
        Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresDoCampeonato(Guid id);
        Task<ApostadorCampeonato> ObterApostadorCampeonatoDoApostador(Guid apostadorId);
        Task<ApostadorCampeonato> ObterApostadorCampeonatoPorApostadorECampeonato(string usuarioId, Guid campeonatoId);
        Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresEmOrdemDescrescenteDePontuacao(Guid campeonatoId);
    }
}