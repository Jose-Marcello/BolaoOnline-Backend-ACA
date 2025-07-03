using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Para IRodadaRepository
using ApostasApp.Core.Domain.Models.Jogos; // Para Jogo
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada, StatusRodada
using ApostasApp.Core.InfraStructure.Data.Context; // Para MeuDbContext
using ApostasApp.Core.Infrastructure.Data.Repository; // PARA HERDAR DE Repository<T>
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Rodadas
{
    // RodadaRepository herda de Repository<Rodada> e implementa IRodadaRepository
    // Ele NÃO precisa re-implementar Adicionar, ObterPorId, etc., pois Repository<Rodada> já faz isso.
    public class RodadaRepository : Repository<Rodada>, IRodadaRepository
    {
        private readonly ILogger<RodadaRepository> _logger;

        public RodadaRepository(MeuDbContext context, ILogger<RodadaRepository> logger) : base(context)
        {
            _logger = logger;
        }

        // Métodos ESPECÍFICOS de IRodadaRepository (além dos que já vêm de IRepository<T>)
        public async Task<Rodada> ObterRodadaEmApostasPorCampeonato(Guid campeonatoId)
        {
            return await Db.Rodadas
                           .Include(r => r.Campeonato)
                           .AsNoTracking()
                           .FirstOrDefaultAsync(r => r.CampeonatoId == campeonatoId && r.Status == StatusRodada.EmApostas);
        }

        
        public async Task<IEnumerable<Rodada>> ObterRodadasEmApostaPorCampeonato(Guid campeonatoId)
        {
            return await Db.Rodadas
                           .Include(r => r.Campeonato)
                           .AsNoTracking()
                           .Where(r => r.CampeonatoId == campeonatoId && r.Status == StatusRodada.EmApostas).ToListAsync();
        }

        public async Task<IEnumerable<Rodada>> ObterRodadasCorrentePorCampeonato(Guid campeonatoId)
        {
            return await Db.Rodadas
                           .Include(r => r.Campeonato)
                           .AsNoTracking()
                           .Where(r => r.CampeonatoId == campeonatoId && r.Status == StatusRodada.Corrente).ToListAsync();
        }

        public async Task<Rodada> ObterRodadaCorrentePorCampeonato(Guid campeonatoId)
        {
            return await Db.Rodadas
                           .Include(r => r.Campeonato)
                           .AsNoTracking()
                           .FirstOrDefaultAsync(r => r.CampeonatoId == campeonatoId && r.Status == StatusRodada.Corrente);
        }

        public async Task<Rodada> ObterRodadaComJogosEEquipes(Guid rodadaId)
        {
            return await Db.Rodadas
                           .Include(r => r.JogosRodada)
                               .ThenInclude(jr => jr.EquipeCasa)
                                   .ThenInclude(ec => ec.Equipe)
                           .Include(r => r.JogosRodada)
                               .ThenInclude(jr => jr.EquipeVisitante)
                                   .ThenInclude(ev => ev.Equipe)
                           .AsNoTracking()
                           .FirstOrDefaultAsync(r => r.Id == rodadaId);
        }

        public async Task<IEnumerable<Jogo>> ObterJogosDaRodadaComPlacaresEEquipes(Guid rodadaId)
        {
            return await Db.Jogos
                           .Include(j => j.EquipeCasa)
                               .ThenInclude(ec => ec.Equipe)
                           .Include(j => j.EquipeVisitante)
                               .ThenInclude(ev => ev.Equipe)
                           .AsNoTracking()
                           .Where(j => j.RodadaId == rodadaId)
                           .ToListAsync();
        }

        public async Task<IEnumerable<Rodada>> ObterRodadasFinalizadasPorCampeonato(Guid campeonatoId)
        {
            return await Db.Rodadas
                           .Include(r => r.Campeonato)
                           .AsNoTracking()
                           .Where(r => r.CampeonatoId == campeonatoId && r.Status == StatusRodada.Finalizada)
                           .ToListAsync();
        }

        public async Task<IEnumerable<Rodada>> ObterRodadasComRankingPorCampeonato(Guid campeonatoId)
        {
            return await Db.Rodadas
                           .Include(r => r.Campeonato)
                           .Where(r => r.CampeonatoId == campeonatoId
                            && (r.Status == StatusRodada.Corrente || r.Status == StatusRodada.Finalizada))
                           .AsNoTracking()
                           .ToListAsync();
        }

        public async Task<IEnumerable<Rodada>> ObterTodasAsRodadasDoCampeonato(Guid campeonatoId)
        {
            return await Db.Rodadas
                           .Include(r => r.Campeonato)
                           .Where(r => r.CampeonatoId == campeonatoId)
                           .AsNoTracking()
                           .ToListAsync();
        }
    }
}
