using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Interfaces.Campeonatos;
using ApostasApp.Core.InfraStructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos
{
    public class CampeonatoRepository : Repository<Campeonato>, ICampeonatoRepository
    {
        private readonly ILogger<CampeonatoRepository> _logger;
        public CampeonatoRepository(MeuDbContext context,
                                    ILogger<CampeonatoRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<Campeonato> ObterCampeonato(Guid id)
        {
            return await Db.Campeonatos.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Campeonato> ObterCampeonatoAtivo()

        {
            var query = Db.Campeonatos.AsNoTracking().Where(c => c.Ativo == true);
            var sql = query.ToQueryString(); // Obtém a consulta SQL gerada
            Console.WriteLine(sql); // Imprime a consulta SQL no console
            return await query.FirstOrDefaultAsync();



        }

        public async Task<IEnumerable<Campeonato>> ObterListaDeCampeonatosAtivos()

        {
            //throw new NotImplementedException();

            return await Db.Campeonatos.AsNoTracking()
                         .Where(c => c.Ativo == true)
                         .ToListAsync();

        }

        public async Task<IEnumerable<Campeonato>> ObterCampeonatosPorTipo()
        {
            return await Db.Campeonatos.AsNoTracking()
                .OrderBy(c => c.Tipo).ToListAsync();
        }

        /*
        public async Task<Campeonato> ObterCampeonatoRodadas(Guid id)
        {
            return await Db.Campeonatos.AsNoTracking()
                 .Include(c => c.Rodadas)
                 .FirstOrDefaultAsync(c => c.Id == id);
        }
        */

    }

}