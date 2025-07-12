using ApostasApp.Core.Application.DTOs.Jogos; // Para JogoDetalheDto, JogoViewModel

namespace ApostasApp.Core.Application.Services.Interfaces.Jogos
{
    /// <summary>
    /// Define o contrato para o serviço de aplicação de jogos.
    /// Este serviço é responsável por orquestrar operações de consulta de jogos.
    /// </summary>
    public interface IJogoService
    {
        /// <summary>
        /// Obtém os detalhes de um jogo específico, incluindo informações de rodada e equipes.
        /// </summary>
        /// <param name="id">O ID do jogo.</param>
        /// <returns>Um DTO com os detalhes do jogo, ou null se não encontrado.</returns>
        Task<JogoDetalheDto> ObterDetalhesJogo(Guid id);

        /// <summary>
        /// Obtém uma coleção de jogos para uma rodada específica, formatados para exibição.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma coleção de DTOs de jogos.</returns>
        Task<IEnumerable<JogoDto>> ObterJogosDaRodada(Guid rodadaId);
    }
}
