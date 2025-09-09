using ApostasApp.Core.Application.Services.Interfaces; // Mantido, mas INotifiableService foi removido da interface específica
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Validations; // Para ApostadorCampeonatoValidation
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApostasApp.Core.Application.Models; // <<-- ADICIONADO: Para ApiResponse -->>
using Microsoft.Extensions.Logging; // <<-- ADICIONADO: Para ILogger -->>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Campeonatos
{
    public class ApostadorCampeonatoService : BaseService, IApostadorCampeonatoService
    {
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApostadorCampeonatoService> _logger; // <<-- ADICIONADO -->>

        public ApostadorCampeonatoService(IApostadorCampeonatoRepository apostadorCampeonatoRepository,
                                          IMapper mapper,
                                          INotificador notificador,
                                          IUnitOfWork uow,
                                          ILogger<ApostadorCampeonatoService> logger) // <<-- ADICIONADO -->>
            : base(notificador, uow)
        {
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _mapper = mapper;
            _logger = logger; // <<-- ATRIBUÍDO -->>
        }

        public async Task<ApiResponse<ApostadorCampeonato>> ObterPorId(Guid apostadorCampeonatoId)
        {
            var apiResponse = new ApiResponse<ApostadorCampeonato>(false, null);
            try
            {
                var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterPorId(apostadorCampeonatoId);
                if (apostadorCampeonato == null)
                {
                    Notificar("ALERTA", "Associação Apostador-Campeonato não encontrada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = apostadorCampeonato;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter associação Apostador-Campeonato por ID.");
                Notificar("ERRO", $"Erro interno ao obter associação: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApiResponse> Adicionar(ApostadorCampeonato apostadorCampeonato)
        {
            var apiResponse = new ApiResponse(false, null);
            try
            {
                //if (!ExecutarValidacao(new ApostadorCampeonatoValidation(), apostadorCampeonato))
                //    return; // Se a validação interna do domínio for usada, as notificações já estarão lá

                if (await _apostadorCampeonatoRepository.Buscar(ec => ec.CampeonatoId == apostadorCampeonato.CampeonatoId
                                                                   && ec.ApostadorId == apostadorCampeonato.ApostadorId).AnyAsync())
                {
                    Notificar("ALERTA", "Este APOSTADOR já foi associado a este CAMPEONATO!");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                await _apostadorCampeonatoRepository.Adicionar(apostadorCampeonato);

                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                Notificar("SUCESSO", "Associação Apostador-Campeonato adicionada com sucesso!");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar associação Apostador-Campeonato.");
                Notificar("ERRO", $"Erro interno ao adicionar associação: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApiResponse> Remover(ApostadorCampeonato apostadorCampeonato)
        {
            var apiResponse = new ApiResponse(false, null);
            try
            {
                if (apostadorCampeonato == null)
                {
                    Notificar("ALERTA", "Associação Apostador-Campeonato não encontrada para remoção.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                await _apostadorCampeonatoRepository.Remover(apostadorCampeonato);
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                Notificar("SUCESSO", "Associação Apostador-Campeonato removida com sucesso!");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover associação Apostador-Campeonato.");
                Notificar("ERRO", $"Erro interno ao remover associação: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<ApostadorCampeonato>> ObterApostadorCampeonatoPorApostadorECampeonato(Guid apostadorId, Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<ApostadorCampeonato>(false, null);
            try
            {
                var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorCampeonatoPorApostadorECampeonato(apostadorId, campeonatoId);
                if (apostadorCampeonato == null)
                {
                    Notificar("ALERTA", "Associação Apostador-Campeonato não encontrada para o usuário e campeonato informados.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = apostadorCampeonato;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter associação Apostador-Campeonato por Apostador e Campeonato.");
                Notificar("ERRO", $"Erro interno ao obter associação: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Obtém uma lista de apostadores associados a um campeonato específico,
        /// incluindo informações do apostador e do usuário.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma coleção de DTOs de ApostadorCampeonato.</returns>
        public async Task<ApiResponse<IEnumerable<ApostadorCampeonatoDto>>> ObterApostadoresDoCampeonato(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<ApostadorCampeonatoDto>>(false, Enumerable.Empty<ApostadorCampeonatoDto>());
            try
            {
                // Chamando o método existente do repositório, que agora inclui Apostador e Usuario
                var apostadoresCampeonato = await _apostadorCampeonatoRepository.ObterApostadoresDoCampeonato(campeonatoId);

                if (apostadoresCampeonato == null || !apostadoresCampeonato.Any())
                {
                    Notificar("ALERTA", "Nenhum apostador encontrado para este campeonato.");
                    apiResponse.Success = true; // Considera sucesso, mas com lista vazia
                    apiResponse.Data = Enumerable.Empty<ApostadorCampeonatoDto>();
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // Mapeia as entidades de domínio para os DTOs
                apiResponse.Success = true;
                apiResponse.Data = _mapper.Map<IEnumerable<ApostadorCampeonatoDto>>(apostadoresCampeonato);
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter apostadores do campeonato.");
                Notificar("ERRO", $"Erro interno ao obter apostadores do campeonato: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<int> ObterPontuacaoTotal(Guid campeonatoId, Guid apostadorCampeonatoId)
        {
            // Chama o repositÃ³rio para obter a pontuaÃ§Ã£o
            var pontuacao = await _apostadorCampeonatoRepository.ObterPontuacaoTotal(campeonatoId, apostadorCampeonatoId);
            return pontuacao;
        }



    }
}
