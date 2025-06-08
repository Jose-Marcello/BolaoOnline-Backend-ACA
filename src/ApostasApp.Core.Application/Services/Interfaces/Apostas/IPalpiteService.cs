using ApostasApp.Core.Application.DTOs.Apostas; // Para PalpiteDto, SalvarPalpiteRequestDto


namespace ApostasApp.Core.Application.Services.Interfaces.Palpites
{
    /// <summary>
    /// Define o contrato para o serviço de aplicação de palpites/apostas.
    /// </summary>
    public interface IPalpiteService
    {
        /// <summary>
        /// Obtém uma coleção de palpites para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Uma coleção de DTOs de palpites.</returns>
        Task<IEnumerable<PalpiteDto>> ObterPalpitesDaRodada(Guid rodadaId);

        /// <summary>
        /// Adiciona um novo palpite.
        /// </summary>
        /// <param name="palpiteRequest">O DTO de requisição contendo os dados do palpite.</param>
        /// <returns>O DTO do palpite adicionado, ou null se falhar.</returns>
        Task<PalpiteDto> AdicionarPalpite(SalvarPalpiteRequestDto palpiteRequest); // NOVO MÉTODO

        /// <summary>
        /// Atualiza um palpite existente.
        /// </summary>
        /// <param name="palpiteId">O ID do palpite a ser atualizado.</param>
        /// <param name="palpiteRequest">O DTO de requisição contendo os novos dados do palpite.</param>
        /// <returns>O DTO do palpite atualizado, ou null se falhar.</returns>
        Task<PalpiteDto> AtualizarPalpite(Guid palpiteId, SalvarPalpiteRequestDto palpiteRequest); // NOVO MÉTODO

        /// <summary>
        /// Remove um palpite pelo seu ID.
        /// </summary>
        /// <param name="palpiteId">O ID do palpite a ser removido.</param>
        /// <returns>True se a remoção foi bem-sucedida, false caso contrário.</returns>
        Task<bool> RemoverPalpite(Guid palpiteId);

        /// <summary>
        /// Verifica se existem apostas (palpites) para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>True se existirem palpites, false caso contrário.</returns>
        Task<bool> ExistePalpitesParaRodada(Guid rodadaId);
    }
}
