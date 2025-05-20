using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Models.Rodadas;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.InfraStructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.Infrastructure.Data.Repository
{
    public class RodadaRepository : Repository<Rodada>, IRodadaRepository
    {
        private readonly ILogger<RodadaRepository> _logger;
        public RodadaRepository(MeuDbContext context,
                                ILogger<RodadaRepository> logger) : base(context)
        {

            _logger = logger;
        }

        public async Task<Rodada> ObterRodada(Guid id)
        {
            return await Db.Rodadas.AsNoTracking()
                .Include(c => c.Campeonato)
               .FirstOrDefaultAsync(r => r.Id == id);
        }

       /* public async Task<Rodada> ObterRodadaAtiva()

        {
            //Provisório - deste modo, só permitiria manter os jogos de uma rodada ATIVA
            //substituir por controle MASTERDETAIL

            return await Db.Rodadas.AsNoTracking()
                         .Include(r => r.Campeonato)
                         .FirstOrDefaultAsync(r => (r.Status == StatusRodada.Corrente || r.Status == StatusRodada.EmApostas) && r.Ativa);

        }*/

        public async Task<Rodada> ObterRodadaEmApostas()
        {
            return await Db.Rodadas.AsNoTracking()
                         .Include(r => r.Campeonato)
                         .FirstOrDefaultAsync(r => r.Status == StatusRodada.EmApostas);

        }
        public async Task<Rodada> ObterRodadaEmConstrucao()
        {
            //Provisório - deste modo, só permitiria manter os jogos de uma rodada ATIVA
            //substituir por controle MASTERDETAIL

            return await Db.Rodadas.AsNoTracking()
                         .Include(r => r.Campeonato)
                         //.Where(r => r.Ativa)
                         .Where(r => r.Status == StatusRodada.EmConstrucao)
                         .FirstOrDefaultAsync(r => r.Status == StatusRodada.EmConstrucao);
            //.ToListAsync();
        }

        public async Task<Rodada> ObterRodadaCorrente()
        {
            return await Db.Rodadas.AsNoTracking()
                         .Include(r => r.Campeonato)
                         .FirstOrDefaultAsync(r => r.Status == StatusRodada.Corrente);
        }

        public async Task<IEnumerable<Rodada>> ObterListaComRodadaCorrente()

        {
            //Provisório - deste modo, só permitiria manter os jogos de uma rodada ATIVA
            //substituir por controle MASTERDETAIL

            return await Db.Rodadas.AsNoTracking()
                         .Include(r => r.Campeonato)
                         //.Where(r => r.Ativa == true)
                         .Where(r => r.Status == StatusRodada.Corrente)
                         //.FirstOrDefaultAsync(r => r.Ativa == true);
                         .ToListAsync();

        }

        public async Task<IEnumerable<Rodada>> ObterRodadasComRanking(Guid idCampeonato)
        {          

            return await Db.Rodadas.AsNoTracking()
                         .Include(r => r.Campeonato)                         
                         .Where(r => (r.Status == StatusRodada.Corrente || r.Status == StatusRodada.Finalizada)
                                && r.CampeonatoId == idCampeonato )
                         .OrderByDescending(r => r.Status == StatusRodada.Corrente)
                         .ToListAsync();

        }

        public async Task<IEnumerable<Rodada>> ObterRodadasCampeonato()
        {

            return await Db.Rodadas.AsNoTracking().Include(c => c.Campeonato)
                                   .OrderBy(r => r.NumeroRodada).ToListAsync();


        }

        public async Task<Rodada> ObterRodadaCampeonato(Guid id)
        {
            return await Db.Rodadas.AsNoTracking().Include(c => c.Campeonato)
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        //Lista Todas as Rodadas de um Campeonato selecionado
        public async Task<IEnumerable<Rodada>> ObterRodadasDoCampeonato(Guid campeonatoId)
        {
            {
                return await Db.Rodadas.AsNoTracking().Include(c => c.Campeonato)
                                   .Where(r => r.CampeonatoId == campeonatoId)
                                   .OrderBy(r => r.NumeroRodada).ToListAsync();

            }
        }

    }

}