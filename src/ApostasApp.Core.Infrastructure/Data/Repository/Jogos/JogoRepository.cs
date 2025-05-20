using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.InfraStructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.Infrastructure.Data.Repository
{
    public class JogoRepository : Repository<Jogo>, IJogoRepository
    {

        private readonly ILogger<JogoRepository> _logger;

        public JogoRepository(MeuDbContext context,
                              ILogger<JogoRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<Jogo> ObterJogo(Guid id)
        {
            return await Db.Jogos.AsNoTracking()
               .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Jogo>> ObterJogosRodada()
        {
            //aqui melhorar ordenando por DATA+HORA
            return await Db.Jogos.AsNoTracking()
                                 .Include(j => j.Rodada)
                                 .Include(j => j.Rodada.Campeonato)
                                 .OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo).ToListAsync();


        }

        public async Task<Jogo> ObterJogoRodada(Guid id)
        {
            return await Db.Jogos.AsNoTracking()
                .Include(j => j.Rodada)
                .Include(j => j.Rodada.Campeonato)
                .Include(j => j.EquipeCasa.Equipe)
                .Include(j => j.EquipeVisitante.Equipe)
                .Include(j => j.Estadio)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<Jogo> ObterJogoEstadioEquipes(Guid id)
        {
            return await Db.Jogos.AsNoTracking()
                //.Include(j => j.Rodada)
                //.Include(j => j.Rodada.Campeonato)
                .Include(j => j.EquipeCasa.Equipe)
                .Include(j => j.EquipeVisitante.Equipe)
                .Include(j => j.Estadio)
                .FirstOrDefaultAsync(j => j.Id == id);
        }


        //Lista Todas as Jogos de um Rodada selecionada
        //aqui melhorar ordenando por DATA+HORA
        public async Task<IEnumerable<Jogo>> ObterJogosDaRodada(Guid rodadaId)
        {
            {
                return await Db.Jogos.AsNoTracking()
                          .Include(j => j.Rodada)
                          .Include(j => j.Estadio)
                          .Include(j => j.EquipeCasa.Equipe)
                          .Include(j => j.EquipeVisitante.Equipe)
                          .Where(j => j.RodadaId == rodadaId)
                          .OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo).ToListAsync();

            }
        }

    }

}