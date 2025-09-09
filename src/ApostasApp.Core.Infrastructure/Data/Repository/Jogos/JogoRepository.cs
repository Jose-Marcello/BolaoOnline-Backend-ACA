using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.InfraStructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace ApostasApp.Core.Infrastructure.Data.Repository.Jogos // Namespace ajustado
{
    public class JogoRepository : Repository<Jogo>, IJogoRepository
    {
        private readonly ILogger<JogoRepository> _logger;

        public JogoRepository(MeuDbContext context,
                               ILogger<JogoRepository> logger) : base(context)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtém um jogo pelo seu ID, incluindo Rodada, Campeonato, Equipes (Casa e Visitante) e Estádio.
        /// Ideal para a tela de detalhes do jogo.
        /// </summary>
        /// <param name="id">O ID do jogo.</param>
        /// <returns>O objeto Jogo completo com as entidades relacionadas, ou null se não encontrado.</returns>
        public async Task<Jogo> ObterJogoComRodadaCampeonatoEquipesEstadio(Guid id)
        {
            return await Db.Jogos.AsNoTracking()
                .Include(j => j.Rodada)
                    .ThenInclude(r => r.Campeonato) // Inclui Campeonato da Rodada
                .Include(j => j.EquipeCasa)
                    .ThenInclude(ec => ec.Equipe)
                .Include(j => j.EquipeVisitante)
                    .ThenInclude(ev => ev.Equipe)
                .Include(j => j.Estadio)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        /// <summary>
        /// Obtém todos os jogos de uma rodada específica, incluindo Equipes (Casa e Visitante) e Estádio,
        /// ordenados por Data e Hora do Jogo.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma coleção de jogos da rodada.</returns>
        public async Task<IEnumerable<Jogo>> ObterJogosDaRodadaComEquipesEEstadio(Guid rodadaId)
        {
            return await Db.Jogos.AsNoTracking()
                             .Include(j => j.EquipeCasa)
                                 .ThenInclude(ec => ec.Equipe)
                             .Include(j => j.EquipeVisitante)
                                 .ThenInclude(ev => ev.Equipe)
                             .Include(j => j.Estadio)
                             .Where(j => j.RodadaId == rodadaId)
                             .OrderBy(j => j.DataJogo)
                             .ThenBy(j => j.HoraJogo)
                             .ToListAsync();
        }

        public async Task<IEnumerable<Jogo>> ObterJogosDaRodada(Guid rodadaId)
        {
            return await Db.Jogos.AsNoTracking()
                            // .Include(j => j.EquipeCasa)
                            //     .ThenInclude(ec => ec.Equipe)
                            // .Include(j => j.EquipeVisitante)
                            //     .ThenInclude(ev => ev.Equipe)
                            // .Include(j => j.Estadio)
                             .Where(j => j.RodadaId == rodadaId)
                             .OrderBy(j => j.DataJogo)
                             .ThenBy(j => j.HoraJogo)
                             .ToListAsync();
        }

        /// <summary>
        /// Obtém todos os jogos, incluindo Rodada e Campeonato, ordenados por Data e Hora do Jogo.
        /// </summary>
        /// <returns>Uma coleção de todos os jogos.</returns>
        public async Task<IEnumerable<Jogo>> ObterTodosJogosComRodadaECampeonato()
        {
            return await Db.Jogos.AsNoTracking()
                                 .Include(j => j.Rodada)
                                     .ThenInclude(r => r.Campeonato)
                                 .OrderBy(j => j.DataJogo)
                                 .ThenBy(j => j.HoraJogo)
                                 .ToListAsync();
        }

        // Os métodos ObterJogo, ObterJogosRodada, ObterJogoEstadioEquipes
        // foram substituídos/consolidados pelos novos métodos mais claros.
        // Se você ainda precisar deles com os nomes antigos, pode mantê-los,
        // mas o ideal é usar os novos.


        public async Task<IEnumerable<Jogo>> ObterJogosDaRodadaComPlacaresEEquipes(Guid rodadaId)
        {
            return await Db.Jogos // Seu DbSet para Jogos
                                 .AsNoTracking()
                                 .Include(j => j.EquipeCasa)
                                     .ThenInclude(ec => ec.Equipe)
                                 .Include(j => j.EquipeVisitante)
                                     .ThenInclude(ev => ev.Equipe)
                                 .Include(j => j.Estadio)
                                 .Where(j => j.RodadaId == rodadaId)
                                 .ToListAsync();
        }


    }


}
