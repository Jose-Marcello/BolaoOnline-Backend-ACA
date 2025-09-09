// Localização: ApostasApp.Core.Application.Services/Palpites/PalpiteService.cs

using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.Palpites;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Palpites;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Apostas;
using AutoMapper;
using Microsoft.EntityFrameworkCore; // Adicionado para Include e ToListAsync
using Microsoft.Extensions.Logging;


namespace ApostasApp.Core.Application.Services.Palpites
{
    public class PalpiteService : BaseService, IPalpiteService
    {
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PalpiteService> _logger;

        public PalpiteService(
            IPalpiteRepository palpiteRepository,
            IMapper mapper,
            INotificador notificador, // Injetar INotificador
            IUnitOfWork uow, // Injetar IUnitOfWork
            ILogger<PalpiteService> logger)
            : base(notificador, uow) // Chamar construtor da BaseService
        {
            _palpiteRepository = palpiteRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtém uma coleção de palpites para uma rodada específica.
        /// </summary>
        public async Task<ApiResponse<IEnumerable<PalpiteDto>>> ObterPalpitesDaRodada(Guid rodadaId)
        {
            try
            {
                var palpites = await _palpiteRepository.Buscar(p => p.Jogo.RodadaId == rodadaId)
                                                       .Include(p => p.Jogo) // Incluir Jogo para acessar RodadaId
                                                       .ToListAsync();

                if (!palpites.Any())
                {
                    Notificar("Alerta", "Nenhum palpite encontrado para a rodada especificada.");
                    return new ApiResponse<IEnumerable<PalpiteDto>>(true, "Nenhum palpite encontrado.", new List<PalpiteDto>(), ObterNotificacoesParaResposta().ToList());
                }

                var palpitesDto = _mapper.Map<IEnumerable<PalpiteDto>>(palpites);
                return new ApiResponse<IEnumerable<PalpiteDto>>(true, "Palpites obtidos com sucesso.", palpitesDto, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter palpites da rodada.");
                Notificar("Erro", $"Erro ao obter palpites da rodada: {ex.Message}");
                return new ApiResponse<IEnumerable<PalpiteDto>>(false, "Ocorreu um erro ao obter os palpites da rodada.", null, ObterNotificacoesParaResposta().ToList());
            }
        }

        /// <summary>
        /// Adiciona um novo palpite.
        /// </summary>
        public async Task<ApiResponse<PalpiteDto>> AdicionarPalpite(SalvarPalpiteRequestDto palpiteRequest)
        {
            try
            {
                // Mapear DTO para entidade
                var palpite = _mapper.Map<Palpite>(palpiteRequest);

                // Adicionar ao repositório
                await _palpiteRepository.Adicionar(palpite);

                // Salvar mudanças
                if (!await CommitAsync())
                {
                    return new ApiResponse<PalpiteDto>(false, "Falha ao adicionar palpite.", null, ObterNotificacoesParaResposta().ToList());
                }

                var palpiteDto = _mapper.Map<PalpiteDto>(palpite);
                return new ApiResponse<PalpiteDto>(true, "Palpite adicionado com sucesso.", palpiteDto, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar palpite.");
                Notificar("Erro", $"Erro ao adicionar palpite: {ex.Message}");
                return new ApiResponse<PalpiteDto>(false, "Ocorreu um erro ao adicionar o palpite.", null, ObterNotificacoesParaResposta().ToList());
            }
        }

        /// <summary>
        /// Atualiza um palpite existente.
        /// </summary>
        public async Task<ApiResponse<PalpiteDto>> AtualizarPalpite(Guid palpiteId, SalvarPalpiteRequestDto palpiteRequest)
        {
            try
            {
                var palpiteExistente = await _palpiteRepository.ObterPorId(palpiteId);
                if (palpiteExistente == null)
                {
                    Notificar("Alerta", "Palpite não encontrado para atualização.");
                    return new ApiResponse<PalpiteDto>(false, "Palpite não encontrado.", null, ObterNotificacoesParaResposta().ToList());
                }

                // Atualizar propriedades do palpite existente
                _mapper.Map(palpiteRequest, palpiteExistente);

                await _palpiteRepository.Atualizar(palpiteExistente);

                if (!await CommitAsync())
                {
                    return new ApiResponse<PalpiteDto>(false, "Falha ao atualizar palpite.", null, ObterNotificacoesParaResposta().ToList());
                }

                var palpiteDto = _mapper.Map<PalpiteDto>(palpiteExistente);
                return new ApiResponse<PalpiteDto>(true, "Palpite atualizado com sucesso.", palpiteDto, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar palpite.");
                Notificar("Erro", $"Erro ao atualizar palpite: {ex.Message}");
                return new ApiResponse<PalpiteDto>(false, "Ocorreu um erro ao atualizar o palpite.", null, ObterNotificacoesParaResposta().ToList());
            }
        }

        /// <summary>
        /// Remove um palpite pelo seu ID.
        /// </summary>
        public async Task<ApiResponse<bool>> RemoverPalpite(Guid palpiteId)
        {
            try
            {
                var palpiteExistente = await _palpiteRepository.ObterPorId(palpiteId);
                if (palpiteExistente == null)
                {
                    Notificar("Alerta", "Palpite não encontrado para remoção.");
                    return new ApiResponse<bool>(false, "Palpite não encontrado.", false, ObterNotificacoesParaResposta().ToList());
                }

                await _palpiteRepository.Remover(palpiteExistente);

                if (!await CommitAsync())
                {
                    return new ApiResponse<bool>(false, "Falha ao remover palpite.", false, ObterNotificacoesParaResposta().ToList());
                }

                return new ApiResponse<bool>(true, "Palpite removido com sucesso.", true, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover palpite.");
                Notificar("Erro", $"Erro ao remover palpite: {ex.Message}");
                return new ApiResponse<bool>(false, "Ocorreu um erro ao remover o palpite.", false, ObterNotificacoesParaResposta().ToList());
            }
        }

        /// <summary>
        /// Verifica se existem apostas (palpites) para uma rodada específica.
        /// </summary>
        public async Task<ApiResponse<bool>> ExistePalpitesParaRodada(Guid rodadaId)
        {
            try
            {
                var existe = await _palpiteRepository.Buscar(p => p.Jogo.RodadaId == rodadaId).AnyAsync();
                return new ApiResponse<bool>(true, "Verificação de palpites concluída.", existe, ObterNotificacoesParaResposta().ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência de palpites para a rodada.");
                Notificar("Erro", $"Erro ao verificar existência de palpites: {ex.Message}");
                return new ApiResponse<bool>(false, "Ocorreu um erro ao verificar palpites.", false, ObterNotificacoesParaResposta().ToList());
            }
        }
    }
}
