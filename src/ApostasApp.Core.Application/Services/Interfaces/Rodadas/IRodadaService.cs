// Localização: ApostasApp.Core.Application.Services.Interfaces.Rodadas/IRodadaService.cs

using ApostasApp.Core.Application.DTOs.Conferencia;
using ApostasApp.Core.Application.Models; // <<-- ADICIONADO: Para ApiResponse -->>
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada
using System; // Para Guid
using System.Collections.Generic; // Para IEnumerable, List
using System.Threading.Tasks; // Para Task

namespace ApostasApp.Core.Application.Services.Interfaces.Rodadas
{
    // IRodadaService agora NÃO herda mais de INotifiableService
    public interface IRodadaService
    {
        // Métodos específicos de Rodada (agora retornando ApiResponse<T>)
        Task<ApiResponse<Rodada>> ObterRodadaPorId(Guid rodadaId);

        // Métodos para obter rodadas em status "Em Apostas"
        Task<ApiResponse<Rodada>> ObterRodadaEmApostasPorCampeonato(Guid campeonatoId); // Para obter uma única rodada (se houver)
        Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasEmApostaPorCampeonato(Guid campeonatoId); // Para obter a lista de rodadas

        // Métodos para obter rodadas em status "Corrente"
        Task<ApiResponse<Rodada>> ObterRodadaCorrentePorCampeonato(Guid campeonatoId); // Para obter uma única rodada (se houver)
        Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasCorrentePorCampeonato(Guid campeonatoId); // Para obter a lista de rodadas

        // Métodos específicos para apostadores (seus métodos originais)
        Task<ApiResponse<Rodada>> ObterRodadaEmApostasParaApostador(Guid rodadaId, Guid apostadorCampeonatoId);
        Task<ApiResponse<Rodada>> ObterRodadaCorrenteParaApostador(Guid rodadaId, Guid apostadorCampeonatoId);

        // Outros métodos de Rodada
        Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasFinalizadasPorCampeonato(Guid campeonatoId);
        Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasComRankingPorCampeonato(Guid campeonatoId);
        Task<ApiResponse<IEnumerable<Rodada>>> ObterTodasAsRodadasDoCampeonato(Guid campeonatoId);
        Task<ApiResponse> Atualizar(Rodada Rodada); // Método para atualizar uma Rodada (retornando ApiResponse sem Data)
        Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasEmDestaque();
        //Task<ApiResponse<IEnumerable<ConferenciaPalpiteDto>>> GerarPlanilhaConferencia(Guid rodadaId);
        Task<ApiResponse<IEnumerable<ConferenciaPalpiteDto>>> GerarPlanilhaConferencia(Guid rodadaId);



    }
}
