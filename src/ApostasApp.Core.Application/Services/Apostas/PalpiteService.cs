using ApostasApp.Core.Application.DTOs.Apostas; // Para PalpiteDto, SalvarPalpiteRequestDto
using ApostasApp.Core.Application.Models; // <<-- NOVO: Para ApiResponse -->>
using ApostasApp.Core.Application.Services.Interfaces.Palpites; // Para IPalpiteService (do serviço de aplicação)
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Interfaces.Apostas; // Para IPalpiteRepository (do domínio)
using ApostasApp.Core.Domain.Models.Apostas; // Para Palpite (entidade de domínio)
using ApostasApp.Core.Domain.Models.Notificacoes; // <<-- NOVO: Para Notificacao (entidade de domínio) -->>
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApostasApp.Core.Application.Services.Base; // Para AnyAsync no PalpiteRepository.Buscar
using System; // Para Guid
using System.Collections.Generic; // Para IEnumerable, List
using System.Linq; // Para Linq
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Palpites
{
    /// <summary>
    /// Serviço de aplicação para operações relacionadas a palpites/apostas.
    /// Implementa a interface IPalpiteService e utiliza IPalpiteRepository para acesso a dados.
    /// </summary>
    public class PalpiteService : BaseService, IPalpiteService
    {
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IMapper _mapper;

        public PalpiteService(IPalpiteRepository palpiteRepository,
                                 IMapper mapper,
                                 INotificador notificador,
                                 IUnitOfWork uow) : base(notificador, uow)
        {
            _palpiteRepository = palpiteRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém uma coleção de palpites para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Um ApiResponse contendo uma coleção de DTOs de palpites.</returns>
        public async Task<ApiResponse<IEnumerable<PalpiteDto>>> ObterPalpitesDaRodada(Guid rodadaId)
        {
            var apiResponse = new ApiResponse<IEnumerable<PalpiteDto>>(false, null);
            var palpites = await _palpiteRepository.ObterPalpitesDaRodada(rodadaId);

            if (!palpites.Any())
            {
                Notificar("PALPITES_NAO_ENCONTRADOS", "Alerta", "Nenhum palpite encontrado para a rodada especificada.");
                apiResponse.Success = true; // Considera sucesso, mas sem dados
                apiResponse.Data = new List<PalpiteDto>();
            }
            else
            {
                apiResponse.Success = true;
                apiResponse.Data = _mapper.Map<IEnumerable<PalpiteDto>>(palpites);
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        /// <summary>
        /// Adiciona um novo palpite.
        /// </summary>
        /// <param name="palpiteRequest">O DTO de requisição contendo os dados do palpite.</param>
        /// <returns>Um ApiResponse contendo o DTO do palpite adicionado.</returns>
        public async Task<ApiResponse<PalpiteDto>> AdicionarPalpite(SalvarPalpiteRequestDto palpiteRequest)
        {
            var apiResponse = new ApiResponse<PalpiteDto>(false, null);

            // Mapeia o DTO de requisição para a entidade de domínio
            var palpite = _mapper.Map<Palpite>(palpiteRequest);

            // A pontuação inicial é 0 e é calculada depois que o jogo termina.
            palpite.Pontos = 0;

            await _palpiteRepository.Adicionar(palpite);
            var saved = await CommitAsync();

            if (saved)
            {
                apiResponse.Success = true;
                apiResponse.Data = _mapper.Map<PalpiteDto>(palpite);
                Notificar("PALPITE_ADICIONADO_SUCESSO", "Sucesso", "Palpite adicionado com sucesso!");
            }
            else
            {
                Notificar("PALPITE_ADICIONADO_FALHA", "Erro", "Não foi possível adicionar o palpite.");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        /// <summary>
        /// Atualiza um palpite existente.
        /// </summary>
        /// <param name="palpiteId">O ID do palpite a ser atualizado.</param>
        /// <param name="palpiteRequest">O DTO de requisição contendo os novos dados do palpite.</param>
        /// <returns>Um ApiResponse contendo o DTO do palpite atualizado.</returns>
        public async Task<ApiResponse<PalpiteDto>> AtualizarPalpite(Guid palpiteId, SalvarPalpiteRequestDto palpiteRequest)
        {
            var apiResponse = new ApiResponse<PalpiteDto>(false, null);
            var palpiteExistente = await _palpiteRepository.ObterPorId(palpiteId);

            if (palpiteExistente == null)
            {
                Notificar("PALPITE_NAO_ENCONTRADO_ATUALIZACAO", "Erro", "Palpite não encontrado para atualização.");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            // Atualiza as propriedades da entidade existente com os dados do DTO de requisição
            palpiteExistente.PlacarApostaCasa = palpiteRequest.PlacarApostaCasa;
            palpiteExistente.PlacarApostaVisita = palpiteRequest.PlacarApostaVisita;

            await _palpiteRepository.Atualizar(palpiteExistente);
            var saved = await CommitAsync();

            if (saved)
            {
                apiResponse.Success = true;
                apiResponse.Data = _mapper.Map<PalpiteDto>(palpiteExistente);
                Notificar("PALPITE_ATUALIZADO_SUCESSO", "Sucesso", "Palpite atualizado com sucesso!");
            }
            else
            {
                Notificar("PALPITE_ATUALIZADO_FALHA", "Erro", "Não foi possível atualizar o palpite.");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        /// <summary>
        /// Remove um palpite pelo seu ID.
        /// </summary>
        /// <param name="palpiteId">O ID do palpite a ser removido.</param>
        /// <returns>Um ApiResponse indicando se a remoção foi bem-sucedida.</returns>
        public async Task<ApiResponse<bool>> RemoverPalpite(Guid palpiteId)
        {
            var apiResponse = new ApiResponse<bool>(false, false);
            var palpite = await _palpiteRepository.ObterPorId(palpiteId);

            if (palpite == null)
            {
                Notificar("PALPITE_NAO_ENCONTRADO_REMOCAO", "Erro", "Palpite não encontrado para remoção.");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            await _palpiteRepository.Remover(palpite);
            var saved = await CommitAsync();

            if (saved)
            {
                apiResponse.Success = true;
                apiResponse.Data = true;
                Notificar("PALPITE_REMOVIDO_SUCESSO", "Sucesso", "Palpite removido com sucesso!");
            }
            else
            {
                Notificar("PALPITE_REMOVIDO_FALHA", "Erro", "Não foi possível remover o palpite.");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        /// <summary>
        /// Verifica se existem apostas (palpites) para uma rodada específica.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>Um ApiResponse indicando se existem palpites.</returns>
        public async Task<ApiResponse<bool>> ExistePalpitesParaRodada(Guid rodadaId)
        {
            var apiResponse = new ApiResponse<bool>(false, false);
            var existe = await _palpiteRepository.Buscar(p => p.Jogo.RodadaId == rodadaId).AnyAsync();

            apiResponse.Success = true;
            apiResponse.Data = existe;
            // Se não houver palpites, uma notificação de alerta pode ser adicionada aqui, se desejado.
            if (!existe)
            {
                Notificar("NENHUM_PALPITE_RODADA", "Alerta", "Não existem palpites para esta rodada.");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }
    }
}
