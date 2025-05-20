using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domains.Models.Interfaces.Apostas;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.InfraStructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.Infrastructure.Data.Repository
{
    public class ApostaRepository : Repository<Aposta>, IApostaRepository
    {
        private readonly ILogger<ApostaRepository> _logger;

        public ApostaRepository(MeuDbContext context,
                                ILogger<ApostaRepository> logger
                                             ) : base(context)
        {
            _logger = logger;
        }

        public ApostaRepository(MeuDbContext db) : base(db)
        {
        }

        public async Task<Aposta> ObterAposta(Guid id)
        {
            return await Db.Apostas.AsNoTracking()
               .FirstOrDefaultAsync(r => r.Id == id);
        }
        /*
                public async Task<IEnumerable<Aposta>> ObterApostasJogoNaRodada(Guid rodadaId)
                {
                    return await Db.Apostas.AsNoTracking()
                               .Include(a => a.Jogo)
                               .Include(a => a.ApostadorCampeonato)
                               .Include(a => a.Jogo.Estadio)                   
                               .Where(a => a.Jogo.RodadaId == rodadaId)
                               .OrderBy(a => a.ApostadorCampeonatoId).ToListAsync();
                }

        */
        //Lista Todas as APOSTAS de um jOGO
        public async Task<IEnumerable<Aposta>> ObterApostasDoJogo(Guid jogoId)
        {
            {
                return await Db.Apostas.AsNoTracking()
                                   .Include(a => a.Jogo)
                                   .Include(a => a.ApostadorCampeonato)
                                   .Where(a => a.JogoId == jogoId)
                                   .OrderBy(a => a.DataHoraAposta).ToListAsync();

            }
        }

        public async Task<IEnumerable<Aposta>> ObterApostasDaRodada(Guid rodadaId)
        {
            {
                return await Db.Apostas.AsNoTracking()
                                   .Include(a => a.Jogo)
                                   //.Include(a => a.ApostadorCampeonato)
                                   .Include(a => a.ApostadorCampeonato.Apostador)
                                   //.Include(a => a.Jogo.EquipeCasa)
                                   .Include(a => a.Jogo.EquipeCasa.Equipe)
                                   //.Include(a => a.Jogo.EquipeVisitante)
                                   .Include(a => a.Jogo.EquipeVisitante.Equipe)
                                   .Where(a => a.Jogo.RodadaId == rodadaId)
                                   //.OrderBy(a => a.ApostadorCampeonato.Apostador.Usuario.UserName).ToListAsync();
                                   .OrderBy(a => a.Jogo.DataJogo).ThenBy(a => a.Jogo.HoraJogo).ToListAsync();

            }
        }

        public async Task<List<Guid>> ObterApostadoresDaRodada(Guid rodadaId)
        {
            _logger.LogInformation($"Iniciando ObterApostadoresDaRodada para rodada {rodadaId}");

            try
            {
                var apostadores = await Db.Apostas
                    .AsNoTracking()
                    .Where(a => a.Jogo.RodadaId == rodadaId)
                    .Select(a => a.ApostadorCampeonatoId)
                    .Distinct()
                    .ToListAsync();

                _logger.LogInformation($"ObterApostadoresDaRodada concluído com sucesso para rodada {rodadaId}, {apostadores.Count} apostadores encontrados.");

                return apostadores;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Erro InvalidOperationException ao obter apostadores da rodada {rodadaId}: {ex.Message}");
                return new List<Guid>(); // Ou lance a exceção novamente, se necessário
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter apostadores da rodada {rodadaId}: {ex.Message}");
                return new List<Guid>(); // Ou lance a exceção novamente, se necessário
            }
        }


        //Lista Todas as APOSTAS de um APOSTADOR
        public async Task<IEnumerable<Aposta>> ObterApostasDoApostador(Guid apostadorId)
        {
            {
                return await Db.Apostas.AsNoTracking()
                                   .Include(a => a.Jogo)
                                   .Include(a => a.ApostadorCampeonato)
                                   .Include(a => a.ApostadorCampeonato.Apostador.Usuario)
                                   .Where(a => a.ApostadorCampeonatoId == apostadorId)                                   
                                   .OrderBy(a => a.Jogo.DataJogo).ThenBy(a => a.Jogo.HoraJogo).ToListAsync();

            }
        }

        public async Task<IEnumerable<Aposta>> ObterApostasDoApostadorNaRodada(Guid rodadaId, Guid apostadorId)
        {

            _logger.LogInformation($"Iniciando ObterApostasDoApostadorNaRodada para rodada : {rodadaId} e apostador : {apostadorId} ");

            try
            {

                return await Db.Apostas.AsNoTracking()
                                 .Include(a => a.Jogo)
                                 //.Include(a => a.ApostadorCampeonato)
                                 .Include(a => a.ApostadorCampeonato.Apostador.Usuario)
                                 //.Include(a => a.Jogo.Rodada)
                                 .Include(a => a.Jogo.EquipeCasa.Equipe)
                                 .Include(a => a.Jogo.EquipeVisitante.Equipe)
                                 //.Include(a => a.ApostadorCampeonato)
                                 .Include(a => a.Jogo.Estadio)
                                 .Where(a => a.Jogo.RodadaId == rodadaId && a.ApostadorCampeonatoId == apostadorId)
                                 .OrderBy(a => a.Jogo.DataJogo).ThenBy(a => a.Jogo.HoraJogo).ToListAsync();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Erro InvalidOperationException ao obter apostadores da rodada {rodadaId} e apostador : {apostadorId} : {ex.Message}");
                return new List<Aposta>(); // Ou lance a exceção novamente, se necessário
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter apostadores da rodada {rodadaId} e apostador : {apostadorId} : {ex.Message}");
                return new List<Aposta>(); // Ou lance a exceção novamente, se necessário
            }
        }

        public async Task<Aposta> ObterStatusApostasDoApostadorNaRodada(Guid rodadaId, Guid apostadorId)
        {
          
                return await Db.Apostas.AsNoTracking()                                
                                 .Where(a => a.Jogo.RodadaId == rodadaId && a.ApostadorCampeonatoId == apostadorId)
                                 .FirstOrDefaultAsync();
           
            
        }

        public async Task<Aposta> ObterApostaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorId)
        {
            return await Db.Apostas.AsNoTracking()
                        .AsNoTracking()
                        .Where(a => a.ApostadorCampeonato.Id == apostadorId && a.Jogo.Rodada.Id == rodadaId)
                        .FirstOrDefaultAsync();

        }


    }

}