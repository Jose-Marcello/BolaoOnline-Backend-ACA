using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Domain.Models.Interfaces.Campeonatos
{
    public interface ICampeonatoRepository : IRepository<Campeonato>
    {
        Task<Campeonato> ObterCampeonato(Guid id);
        Task<IEnumerable<Campeonato>> ObterCampeonatosPorTipo();

        //Só pode haver um campeonato ativo (Validar isso)
        Task<Campeonato> ObterCampeonatoAtivo();
        Task<IEnumerable<Campeonato>> ObterListaDeCampeonatosAtivos(); //A princípio, uma lista com apenas 1 (p/grid)

        //Task<Campeonato> ObterCampeonatoRodadas(Guid Id);

    }
}