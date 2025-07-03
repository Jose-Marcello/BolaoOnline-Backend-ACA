// Localização: ApostasApp.Core.Application.Services.Interfaces.Rodadas/IRodadaService.cs

using System; // Para Guid
using System.Collections.Generic; // Para IEnumerable, List
using System.Threading.Tasks; // Para Task
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada
using ApostasApp.Core.Application.Services.Interfaces; // Para INotifiableService

namespace ApostasApp.Core.Application.Services.Interfaces.Rodadas
{
    // IRodadaService agora herda de INotifiableService
    public interface IRodadaService : INotifiableService
    {
        // Métodos específicos de Rodada (garantidos para estarem completos aqui)
        Task<Rodada> ObterRodadaPorId(Guid rodadaId);

        // Métodos para obter rodadas em status "Em Apostas"
        Task<Rodada> ObterRodadaEmApostasPorCampeonato(Guid campeonatoId); // Para obter uma única rodada (se houver)
        Task<IEnumerable<Rodada>> ObterRodadasEmApostaPorCampeonato(Guid campeonatoId); // Para obter a lista de rodadas

        // Métodos para obter rodadas em status "Corrente"
        Task<Rodada> ObterRodadaCorrentePorCampeonato(Guid campeonatoId); // Para obter uma única rodada (se houver)
        Task<IEnumerable<Rodada>> ObterRodadasCorrentePorCampeonato(Guid campeonatoId); // Para obter a lista de rodadas

        // Métodos específicos para apostadores (seus métodos originais)
        Task<Rodada> ObterRodadaEmApostasParaApostador(Guid rodadaId, Guid apostadorCampeonatoId);
        Task<Rodada> ObterRodadaCorrenteParaApostador(Guid rodadaId, Guid apostadorCampeonatoId);

        // Outros métodos de Rodada
        Task<IEnumerable<Rodada>> ObterRodadasFinalizadasPorCampeonato(Guid campeonatoId);
        Task<IEnumerable<Rodada>> ObterRodadasComRankingPorCampeonato(Guid campeonatoId);
        Task<IEnumerable<Rodada>> ObterTodasAsRodadasDoCampeonato(Guid campeonatoId);
        Task Atualizar(Rodada Rodada); // Método para atualizar uma Rodada
        Task<IEnumerable<Rodada>> ObterRodadasEmDestaque();
    }
}
