using ApostasApp.Core.Domain.Interfaces.RankingRodadas;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.InfraStructure.Data.Repository;
using DApostasApp.Core.Domain.Models.RankingRodadas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Infrastructure.Data.Repository
{
    public class RankingRodadaRepository : Repository<RankingRodada>, IRankingRodadaRepository
    {
        private readonly ILogger<RankingRodadaRepository> _logger;

        public RankingRodadaRepository(MeuDbContext context,
                                       ILogger<RankingRodadaRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<RankingRodada> ObterRankingRodada(Guid id)
        {
            return await Db.RankingRodadas.AsNoTracking()
               .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<RankingRodada>> ObterRankingDaRodada(Guid idRodada)
        {
            return await Db.RankingRodadas.AsNoTracking()
                .Include(r => r.Rodada)
                .Include(r => r.Rodada.Campeonato)
                .Include(r => r.ApostadorCampeonato.Apostador.Usuario)
                .Where(r => r.RodadaId == idRodada)
                .OrderBy(r => r.Posicao) // Ordenar por pontuação
                //.OrderByDescending(r => r.Pontuacao) // Ordenar por pontuação
                .ToListAsync();
        }

        public async Task<RankingRodada> ObterRankingDoApostadorNaRodada(Guid idRodada, Guid idApostador)
        {
            return await Db.RankingRodadas.AsNoTracking()
                .Include(r => r.Rodada)
                .Include(r => r.Rodada.Campeonato)
                .Include(r => r.ApostadorCampeonato.Apostador)
                //.Where(r => r.RodadaId == idRodada && r.ApostadorCampeonatoId == idApostador).ToListAsync();
                .FirstOrDefaultAsync(r => r.Rodada.Id == idRodada && r.ApostadorCampeonato.Id == idApostador);
        }

    }

}