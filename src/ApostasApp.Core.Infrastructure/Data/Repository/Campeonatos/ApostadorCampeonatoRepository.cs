using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces.Campeonatos;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.InfraStructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos
{
    public class ApostadorCampeonatoRepository : Repository<ApostadorCampeonato>,
                                                 IApostadorCampeonatoRepository
    {
        private readonly ILogger<ApostadorCampeonatoRepository> _logger;
        public ApostadorCampeonatoRepository(MeuDbContext context,
                                             ILogger<ApostadorCampeonatoRepository> logger) : base(context)

        {
            _logger = logger;
        }

        public async Task<ApostadorCampeonato> ObterApostadorCampeonato(Guid id)
        {
            return await Db.ApostadoresCampeonatos.AsNoTracking()
                .Include(c => c.Apostador)
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<ApostadorCampeonato> ObterApostadorCampeonatoDoApostador(Guid apostadorId)
        {
            return await Db.ApostadoresCampeonatos.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ApostadorId == apostadorId);
        }

        public async Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresComRanking(Guid campeonatoId)
        {
            return await Db.ApostadoresCampeonatos.AsNoTracking()
                    .Include(ac => ac.Apostador.Usuario)
                    .Where(ac => ac.CampeonatoId == campeonatoId).ToListAsync();                             

        }


        public async Task<ApostadorCampeonato> ObterApostadorDoCampeonato(Guid campeonatoId, Guid apostadorId)
        {

            return await Db.ApostadoresCampeonatos.AsNoTracking()
                               .Where(ec => ec.CampeonatoId == campeonatoId && ec.ApostadorId == apostadorId)
                               .FirstOrDefaultAsync(ec => ec.CampeonatoId == campeonatoId && ec.ApostadorId == apostadorId);
        }

        public async Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresDoCampeonato(Guid campeonatoId)
        {
            {
                return await Db.ApostadoresCampeonatos.AsNoTracking()
                                   .Include(c => c.Campeonato)
                                   .Include(a => a.Apostador)
                                   .Include(a => a.Apostador.Usuario)
                                   .Where(ec => ec.CampeonatoId == campeonatoId)
                                   .OrderBy(ec => ec.Apostador.Usuario.UserName).ToListAsync();
            }
        }

        public async Task<ApostadorCampeonato> ObterApostadorCampeonatoPorApostadorECampeonato(string usuarioId, Guid campeonatoId)
        {
            return await Db.ApostadoresCampeonatos.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Apostador.Usuario.Id == usuarioId && a.CampeonatoId == campeonatoId);
        }

        /*public async Task<ApostadorCampeonato> ObterApostadorCampeonatoPorApostadorERodada(string usuarioId, Guid rodadaId)
        {
            return await Db.ApostadoresCampeonatos.AsNoTracking()
                        //.Where(a => a == campeonatoId && ec.ApostadorId == apostadorId)
                        .FirstOrDefaultAsync(a => a.Apostador.Usuario.Id == usuarioId && a.ApostadorId == apostadorId);


        }*/

        public async Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresEmOrdemDescrescenteDePontuacao(Guid campeonatoId)
        {

            return await Db.ApostadoresCampeonatos.AsNoTracking()
                      .Include(ac => ac.Apostador.Usuario)
                      .Where(ac => ac.CampeonatoId == campeonatoId)
                      .OrderByDescending(ac => ac.Pontuacao)
                      .ToListAsync();

        }

    }

}