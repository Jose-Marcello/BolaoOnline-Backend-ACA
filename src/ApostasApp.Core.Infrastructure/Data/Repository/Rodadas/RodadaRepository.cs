using ApostasApp.Core.Application.DTOs.Conferencia;
using ApostasApp.Core.Domain.Interfaces.Relatorios;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Para IRodadaRepository
using ApostasApp.Core.Domain.Models.Jogos; // Para Jogo
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada, StatusRodada
using ApostasApp.Core.Infrastructure.Data.Models;
using ApostasApp.Core.Infrastructure.Data.Repository; // PARA HERDAR DE Repository<T>
using ApostasApp.Core.InfraStructure.Data.Context; // Para MeuDbContext
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Rodadas
{
    // RodadaRepository herda de Repository<Rodada> e implementa IRodadaRepository
    // Ele NÃO precisa re-implementar Adicionar, ObterPorId, etc., pois Repository<Rodada> já faz isso.
    public class RodadaRepository : Repository<Rodada>, IRodadaRepository
    {
        private readonly ILogger<RodadaRepository> _logger;
        private readonly IMapper _mapper; // Adicione o IMapper

        public RodadaRepository(MeuDbContext context, ILogger<RodadaRepository> logger,
                                                      IMapper mapper) : base(context)
        {
            _logger = logger;
            _mapper = mapper;
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


        // Localização: ApostasApp.Core.Infrastructure.Data.Repositories.Apostas/ApostaRodadaRepository.cs

        // ...

        // Localização: ApostasApp.Core.Infrastructure.Data.Repositories.Apostas/ApostaRodadaRepository.cs
        // Garanta que o método retorna uma interface, como havíamos discutido.
        public async Task<IEnumerable<IConferenciaPalpite>> ObterDadosPlanilhaConferenciaAsync(Guid rodadaId)
        {
            var dados = await Db.Palpites
                .AsNoTracking()
                .Where(p => p.ApostaRodada.RodadaId == rodadaId)
                .Select(p => new ConferenciaPalpiteDataModel
                {
                    ApelidoApostador = p.ApostaRodada.ApostadorCampeonato.Apostador.Usuario.Apelido,
                    IdentificadorAposta = p.ApostaRodada.IdentificadorAposta,
                    DataHoraEnvio = p.ApostaRodada.DataHoraSubmissao.HasValue ? p.ApostaRodada.DataHoraSubmissao.Value : DateTime.MinValue,
                    NomeEquipeCasa = p.Jogo.EquipeCasa.Equipe.Nome,
                    PlacarPalpiteCasa = p.PlacarApostaCasa ?? 0,
                    PlacarPalpiteVisita = p.PlacarApostaVisita ?? 0,
                    NomeEquipeVisita = p.Jogo.EquipeVisitante.Equipe.Nome,
                    DataJogo = p.Jogo.DataJogo,
                    HoraJogo = p.Jogo.HoraJogo.ToString(),
                    // Adicione as outras propriedades se necessário
                })
                .OrderBy(p => p.ApelidoApostador)
                .ToListAsync();

            return dados;
        }
    }
}
