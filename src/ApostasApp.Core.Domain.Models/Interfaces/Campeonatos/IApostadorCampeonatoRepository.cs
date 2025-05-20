using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Domain.Models.Interfaces.Campeonatos
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