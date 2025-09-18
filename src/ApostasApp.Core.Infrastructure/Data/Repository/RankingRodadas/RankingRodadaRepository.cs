using ApostasApp.Core.Domain.Interfaces.RankingRodadas;
using ApostasApp.Core.Infrastructure.Data.Context;
using ApostasApp.Core.Domain.Models.RankingRodadas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ApostasApp.Core.Infrastructure.Data.Repository;

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


        public async Task<IEnumerable<RankingRodada>> ObterRankingDaRodada(Guid rodadaId)
        {
            return await DbSet
                .AsNoTracking()
                .Include(r => r.ApostadorCampeonato) // Incluindo o ApostadorCampeonato
                    .ThenInclude(ac => ac.Apostador)     // E o Apostador dentro dele
                        .ThenInclude(ap => ap.Usuario)    // e o usuario 
                .Where(r => r.RodadaId == rodadaId)
                .OrderByDescending(r => r.Pontuacao)
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


        /*
        public async Task<IEnumerable<(Guid ApostadorCampeonatoId, int TotalPontos)>> ObterRankingCampeonatoTotalizadoAsync(Guid campeonatoId)
        {
            var ranking = await DbSet.AsNoTracking()
                              .Where(r => r.Rodada.CampeonatoId == campeonatoId)
                              .GroupBy(r => r.ApostadorCampeonatoId)
                              .Select(g => new
                              {
                                  ApostadorCampeonatoId = g.Key,
                                  TotalPontos = g.Sum(r => r.Pontuacao)
                              })
                              .OrderByDescending(r => r.TotalPontos)
                              .ToListAsync();

            return ranking.Select(r => (r.ApostadorCampeonatoId, r.TotalPontos));
        }
    
        */

    }
}

