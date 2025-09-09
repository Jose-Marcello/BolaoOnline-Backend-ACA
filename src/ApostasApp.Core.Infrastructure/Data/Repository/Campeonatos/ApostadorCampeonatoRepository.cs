using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Campeonatos
{
    public class ApostadorCampeonatoRepository : Repository<ApostadorCampeonato>, IApostadorCampeonatoRepository
    {
        private readonly ILogger<ApostadorCampeonatoRepository> _logger;

        public ApostadorCampeonatoRepository(MeuDbContext context, ILogger<ApostadorCampeonatoRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<ApostadorCampeonato> ObterApostadorCampeonato(Guid id)
        {
            return await Db.ApostadoresCampeonatos
                           .AsNoTracking()
                           .FirstOrDefaultAsync(ac => ac.Id == id);
        }
        
        public async Task<ApostadorCampeonato> ObterApostadorDoCampeonato(Guid idCampeonato, Guid idApostador)
        {
            return await Db.ApostadoresCampeonatos
                           .AsNoTracking()
                           .FirstOrDefaultAsync(ac => ac.CampeonatoId == idCampeonato && ac.ApostadorId == idApostador);
        }

        public async Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresComRanking(Guid campeonatoId)
        {
            return await Db.ApostadoresCampeonatos
                           .Include(ac => ac.Apostador)
                               .ThenInclude(a => a.Usuario) // Inclui o Usuario do Apostador
                           .Include(ac => ac.Campeonato)
                           .AsNoTracking()
                           .Where(ac => ac.CampeonatoId == campeonatoId)
                           .OrderByDescending(ac => ac.Pontuacao) // Assumindo que PontuacaoAtual existe na entidade
                           .ToListAsync();
        }

        /// <summary>
        /// Obtém uma coleção de associações ApostadorCampeonato para um campeonato,
        /// incluindo as entidades Apostador e Usuario relacionadas.
        /// </summary>
        /// <param name="id">O ID do campeonato.</param>
        /// <returns>Uma coleção de entidades ApostadorCampeonato.</returns>
        public async Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresDoCampeonato(Guid id) // Implementação do método existente
        {
            return await Db.ApostadoresCampeonatos
                           .Include(ac => ac.Apostador)
                               .ThenInclude(a => a.Usuario) // <--- CRUCIAL: Inclui o Usuario do Apostador
                           .Include(ac => ac.Campeonato)
                           .AsNoTracking()
                           .Where(ac => ac.CampeonatoId == id)
                           .ToListAsync();
        }

        public async Task<ApostadorCampeonato> ObterApostadorCampeonatoDoApostador(Guid apostadorId)
        {
            return await Db.ApostadoresCampeonatos
                           .AsNoTracking()
                           .FirstOrDefaultAsync(ac => ac.ApostadorId == apostadorId);
        }

        public async Task<ApostadorCampeonato> ObterApostadorCampeonatoPorApostadorECampeonato(Guid apostadorId, Guid campeonatoId)
        {
            return await Db.ApostadoresCampeonatos
                           .Include(ac => ac.Apostador)
                               .ThenInclude(a => a.Usuario) // Inclui o Usuario do Apostador
                           .Include(ac => ac.Campeonato)
                           .AsNoTracking()
                           //.FirstOrDefaultAsync(ac => ac.Apostador.UsuarioId == usuarioId && ac.CampeonatoId == campeonatoId);
                           .FirstOrDefaultAsync(ac => ac.Apostador.Id == apostadorId && ac.CampeonatoId == campeonatoId);
        }

        public async Task<IEnumerable<ApostadorCampeonato>> ObterAdesoesPorUsuarioIdAsync(string usuarioId)
        {
            return await Db.ApostadoresCampeonatos
                           .Include(ac => ac.Apostador)
                               .ThenInclude(a => a.Usuario) // Inclui o Usuario do Apostador
                           .Include(ac => ac.Campeonato)
                           .AsNoTracking()
                           .Where(ac => ac.Apostador.UsuarioId == usuarioId).ToListAsync();
        }


        public async Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresEmOrdemDescrescenteDePontuacao(Guid campeonatoId)
        {
            return await Db.ApostadoresCampeonatos
                           .Include(ac => ac.Apostador)
                               .ThenInclude(a => a.Usuario) // Inclui o Usuario do Apostador
                           .Include(ac => ac.Campeonato)
                           .AsNoTracking()
                           .Where(ac => ac.CampeonatoId == campeonatoId)
                           .OrderByDescending(ac => ac.Pontuacao) // Assumindo que PontuacaoAtual existe na entidade
                           .ToListAsync();
        }

        public async Task<int> ObterPontuacaoTotal(Guid campeonatoId, Guid apostadorCampeonatoId)
        {
            // Consulta o banco de dados para somar a pontuaÃ§Ã£o total das apostas do apostador
            var pontuacao = await Db.ApostasRodada
                .AsNoTracking()
                .Where(a => a.Rodada.CampeonatoId == campeonatoId && a.ApostadorCampeonatoId == apostadorCampeonatoId)
                .SumAsync(a => a.PontuacaoTotalRodada);

            return pontuacao;
        }


    }
}
