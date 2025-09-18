using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.Infrastructure.Data.Context;
using ApostasApp.Core.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.Infrastructure.Data.Repository.Campeonatos
{
    public class EquipeCampeonatoRepository : Repository<EquipeCampeonato>, IEquipeCampeonatoRepository
    {
        private readonly ILogger<EquipeCampeonatoRepository> _logger;
        public EquipeCampeonatoRepository(MeuDbContext context,
                                          ILogger<EquipeCampeonatoRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<EquipeCampeonato> ObterEquipeCampeonato(Guid campeonatoId, Guid equipeId)
        {

            return await Db.EquipesCampeonatos.AsNoTracking()
                               .Where(ec => ec.CampeonatoId == campeonatoId && ec.EquipeId == equipeId)
                               .FirstOrDefaultAsync(ec => ec.CampeonatoId == campeonatoId && ec.EquipeId == equipeId);
        }

        public async Task<IEnumerable<EquipeCampeonato>> ObterEquipesDoCampeonato(Guid campeonatoId)
        {
            {
                return await Db.EquipesCampeonatos.AsNoTracking()
                                   .Include(c => c.Campeonato)
                                   .Include(e => e.Equipe)
                                   .Include(u => u.Equipe.Uf)
                                   .Where(ec => ec.CampeonatoId == campeonatoId).ToListAsync();
                                   //.OrderBy(ec => ec.Equipe.Nome).ToListAsync();

            }


        }




    }

}