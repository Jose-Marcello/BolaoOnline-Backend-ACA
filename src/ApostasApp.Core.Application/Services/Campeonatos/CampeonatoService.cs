// Localização: ApostasApp.Core.Application.Services.Campeonatos/CampeonatoService.cs
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Apostas; // Para IApostaRodadaRepository e IPalpiteRepository
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Jogos; // Para IJogoRepository
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Apostas; // Para ApostaRodada e Palpite
using ApostasApp.Core.Domain.Models.Campeonatos; // Importado para usar o modelo de domínio Campeonato
using ApostasApp.Core.Domain.Models.Financeiro; // Para TipoTransacao
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Verifique se esta interface é realmente necessária ou se é um using antigo
using ApostasApp.Core.Domain.Models.Notificacoes; // Para NotificationDto (DTO de notificação)
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Campeonatos
{
    public class CampeonatoService : BaseService, ICampeonatoService
    {
        private readonly ICampeonatoRepository _campeonatoRepository;
        private readonly IApostadorRepository _apostadorRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly IFinanceiroService _financeiroService;
        private readonly IMapper _mapper;
        private readonly ILogger<CampeonatoService> _logger;

        private readonly IRodadaRepository _rodadaRepository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IPalpiteRepository _palpiteRepository;

        public CampeonatoService(
            ICampeonatoRepository campeonatoRepository,
            IApostadorRepository apostadorRepository,
            IApostadorCampeonatoRepository apostadorCampeonatoRepository,
            IFinanceiroService financeiroService,
            IUnitOfWork uow,
            INotificador notificador,
            IMapper mapper,
            ILogger<CampeonatoService> logger,
            IRodadaRepository rodadaRepository,
            IJogoRepository jogoRepository,
            IApostaRodadaRepository apostaRodadaRepository,
            IPalpiteRepository palpiteRepository)
            : base(notificador, uow)
        {
            _campeonatoRepository = campeonatoRepository;
            _apostadorRepository = apostadorRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _financeiroService = financeiroService;
            _mapper = mapper;
            _logger = logger;

            _rodadaRepository = rodadaRepository;
            _jogoRepository = jogoRepository;
            _apostaRodadaRepository = apostaRodadaRepository;
            _palpiteRepository = palpiteRepository;
        }

        public async Task<bool> Adicionar(CampeonatoDto campeonatoDto)
        {
            try
            {
                var campeonato = _mapper.Map<Campeonato>(campeonatoDto);
                _campeonatoRepository.Adicionar(campeonato);
                return await CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar campeonato.");
                Notificar("Erro", $"Erro interno ao adicionar campeonato: {ex.Message}"); // Notificar com 2 argumentos
                return false;
            }
        }

        public async Task<bool> Atualizar(CampeonatoDto campeonatoDto)
        {
            try
            {
                var campeonato = _mapper.Map<Campeonato>(campeonatoDto);
                _campeonatoRepository.Atualizar(campeonato);
                return await CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar campeonato.");
                Notificar("Erro", $"Erro interno ao atualizar campeonato: {ex.Message}"); // Notificar com 2 argumentos
                return false;
            }
        }

        public async Task<bool> Remover(Campeonato campeonato)
        {
            try
            {
                _campeonatoRepository.Remover(campeonato);
                return await CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover campeonato.");
                Notificar("Erro", $"Erro interno ao remover campeonato: {ex.Message}"); // Notificar com 2 argumentos
                return false;
            }
        }

        public async Task<CampeonatoDto> ObterPorId(Guid id)
        {
            try
            {
                var campeonato = await _campeonatoRepository.ObterPorId(id);
                return _mapper.Map<CampeonatoDto>(campeonato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter campeonato por ID.");
                Notificar("Erro", $"Erro interno ao obter campeonato por ID: {ex.Message}"); // Notificar com 2 argumentos
                return null;
            }
        }

        public async Task<IEnumerable<CampeonatoDto>> ObterTodos()
        {
            try
            {
                var campeonatos = await _campeonatoRepository.ObterTodos();
                return _mapper.Map<IEnumerable<CampeonatoDto>>(campeonatos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os campeonatos.");
                Notificar("Erro", $"Erro interno ao obter todos os campeonatos: {ex.Message}"); // Notificar com 2 argumentos
                return Enumerable.Empty<CampeonatoDto>();
            }
        }

        public async Task<ApiResponse<IEnumerable<CampeonatoDto>>> GetAvailableCampeonatos(string? userId)
        {
            var apiResponse = new ApiResponse<IEnumerable<CampeonatoDto>>(); // Instanciação direta
            try
            {
                HashSet<string> campeonatosAderidosIds = new HashSet<string>();

                if (!string.IsNullOrEmpty(userId))
                {
                    var adesoesDoUsuario = await _apostadorCampeonatoRepository.ObterAdesoesPorUsuarioIdAsync(userId);
                    campeonatosAderidosIds = new HashSet<string>(
                        adesoesDoUsuario.Select(ac => ac.CampeonatoId.ToString())
                    );
                }

                var todosCampeonatos = await _campeonatoRepository.ObterListaDeCampeonatosAtivos();

                var campeonatosDto = todosCampeonatos.Select(c => new CampeonatoDto
                {
                    Id = c.Id.ToString(),
                    Nome = c.Nome,
                    DataInicio = c.DataInic,
                    DataFim = c.DataFim,
                    NumRodadas = c.NumRodadas,
                    Tipo = c.Tipo.ToString(),
                    Ativo = c.Ativo,
                    CustoAdesao = c.CustoAdesao.HasValue ? (decimal)c.CustoAdesao.Value : 0m,
                    AderidoPeloUsuario = !string.IsNullOrEmpty(userId) && campeonatosAderidosIds.Contains(c.Id.ToString())
                }).ToList();

                if (!campeonatosDto.Any())
                {
                    Notificar("Alerta", "Nenhum campeonato disponível encontrado na base de dados."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Pode ser sucesso com dados vazios e alerta
                    apiResponse.Data = new List<CampeonatoDto>();
                }
                else
                {
                    apiResponse.Success = true;
                    apiResponse.Data = campeonatosDto;
                }
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter campeonatos disponíveis.");
                Notificar("Erro", $"Erro interno ao obter campeonatos disponíveis: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Success = false; // Em caso de exceção, é um erro
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = Enumerable.Empty<CampeonatoDto>(); // Garante que Data não é nula
                return apiResponse;
            }
        }

        // Substitua o seu método AderirCampeonatoAsync existente por este:
        public async Task<ApiResponse<bool>> AderirCampeonatoAsync(Guid apostadorId, Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<bool>(); // Instanciação direta
            try
            {
                var campeonato = await _campeonatoRepository.ObterPorId(campeonatoId);
                if (campeonato == null)
                {
                    Notificar("Erro", "Campeonato não encontrado."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false;
                    apiResponse.Data = false;
                    return apiResponse;
                }

                var adesaoExistente = await _apostadorCampeonatoRepository.ObterApostadorCampeonatoPorApostadorECampeonato(apostadorId, campeonatoId);
                if (adesaoExistente != null)
                {
                    Notificar("Alerta", "Você já aderiu a este campeonato."); // Notificar com 2 argumentos
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostador = await _apostadorRepository.ObterPorId(apostadorId); // Este é o ID da entidade Apostador
                if (apostador == null)
                {
                    Notificar("Erro", "Apostador não encontrado."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false;
                    apiResponse.Data = false;
                    return apiResponse;
                }

                if (campeonato.CustoAdesao.HasValue && campeonato.CustoAdesao.Value > 0)
                {
                    var debitoResponse = await _financeiroService.DebitarSaldoAsync(
                        apostadorId, // <<-- Aqui é o ID da entidade Apostador
                        campeonato.CustoAdesao.Value,
                        TipoTransacao.AdesaoCampeonato,
                        $"Adesão ao campeonato: {campeonato.Nome}");

                    if (!debitoResponse.Success)
                    {
                        // Propaga as notificações do FinanceiroService
                        apiResponse.Notifications = debitoResponse.Notifications.ToList();
                        apiResponse.Success = false;
                        apiResponse.Data = false;
                        return apiResponse;
                    }
                    // A limpeza de notificações deve ser feita no Notificador ou no ciclo de vida da requisição,
                    // não diretamente aqui. Removido o acesso direto a _notificador.LimparNotificacoes();
                }

                // 1. Criar o registro de ApostadorCampeonato
                var novaAdesao = new ApostadorCampeonato(apostadorId, campeonato.Id)
                {
                    DataInscricao = DateTime.Now,
                    CustoAdesaoPago = campeonato.CustoAdesao.HasValue && campeonato.CustoAdesao.Value > 0
                };
                _apostadorCampeonatoRepository.Adicionar(novaAdesao);

                // 2. Encontrar a Rodada "Em Apostas" para este campeonato
                var rodadasEmApostas = await _rodadaRepository.ObterRodadasEmApostaPorCampeonato(campeonato.Id);
                var rodadaParaApostaInicial = rodadasEmApostas?.FirstOrDefault(); // Pega a primeira, se houver

                if (rodadaParaApostaInicial == null)
                {
                    Notificar("Alerta", "Nenhuma rodada 'Em Apostas' encontrada para este campeonato. A aposta inicial não será criada."); // Notificar com 2 argumentos
                    // Neste caso, a adesão ainda é válida, mas a aposta inicial não pode ser criada.
                    // O fluxo continua para o commit apenas da adesão e débito.
                }
                else
                {
                    // 3. Criar a ApostaRodada para o apostador e esta rodada
                    // <<-- CORREÇÃO PRINCIPAL AQUI: Usar novaAdesao.Id para ApostadorCampeonatoId -->>
                    var apostaRodada = new ApostaRodada(novaAdesao.Id, rodadaParaApostaInicial.Id);
                    _apostaRodadaRepository.Adicionar(apostaRodada);

                    // 4. Obter todos os jogos dessa rodada
                    var jogosDaRodada = await _jogoRepository.ObterJogosDaRodada(rodadaParaApostaInicial.Id);

                    // 5. Criar Palpites iniciais para cada jogo
                    if (jogosDaRodada != null && jogosDaRodada.Any())
                    {
                        foreach (var jogo in jogosDaRodada)
                        {
                            var palpite = new Palpite(apostaRodada.Id, jogo.Id)
                            {
                                PlacarApostaCasa = null,
                                PlacarApostaVisita = null
                            };
                            _palpiteRepository.Adicionar(palpite);
                        }
                    }
                    else
                    {
                        Notificar("Alerta", "Nenhum jogo encontrado para a rodada 'Em Apostas'. Palpites iniciais não serão criados."); // Notificar com 2 argumentos
                    }
                }

                // CommitAsync() MOVIDO PARA O FINAL DE TODAS AS OPERAÇÕES
                var saved = await CommitAsync();

                if (saved)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    Notificar("Sucesso", "Adesão ao campeonato e preparação para apostas realizada com sucesso!"); // Notificar com 2 argumentos
                }
                else
                {
                    Notificar("Erro", "Não foi possível vincular o apostador ao campeonato ou preparar as apostas."); // Notificar com 2 argumentos
                }
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao aderir ao campeonato.");
                Notificar("Erro", $"Erro interno ao aderir ao campeonato: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Success = false;
                apiResponse.Data = false;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<CampeonatoDto?>> GetDetalhesCampeonato(Guid id)
        {
            var apiResponse = new ApiResponse<CampeonatoDto?>(); // Instanciação direta
            try
            {
                var campeonato = await _campeonatoRepository.ObterPorId(id);

                if (campeonato == null)
                {
                    Notificar("Alerta", $"Campeonato com ID '{id}' não encontrado."); // Notificar com 2 argumentos
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                }
                else
                {
                    apiResponse.Success = true;
                    apiResponse.Data = _mapper.Map<CampeonatoDto>(campeonato);
                }
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes do campeonato.");
                Notificar("Erro", $"Erro interno ao obter detalhes do campeonato: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Success = false;
                apiResponse.Data = null;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> GetRodadasCorrentes(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>(); // Instanciação direta

            try
            {
                var rodadas = await _rodadaRepository.ObterRodadasCorrentePorCampeonato(campeonatoId);
                if (rodadas == null || !rodadas.Any())
                {
                    Notificar("Alerta", "Nenhuma rodada 'Corrente' encontrada para este campeonato."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Pode ser sucesso com dados vazios e alerta
                    apiResponse.Data = Enumerable.Empty<Rodada>();
                }
                else
                {
                    apiResponse.Success = true;
                    apiResponse.Data = rodadas;
                }
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodadas correntes.");
                Notificar("Erro", $"Erro interno ao obter rodada(s) corrente(s): {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>(); // Garante que Data não é nula
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> GetRodadasEmApostas(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>(); // Instanciação direta
            try
            {
                var rodadas = await _rodadaRepository.ObterRodadasEmApostaPorCampeonato(campeonatoId);
                if (rodadas == null || !rodadas.Any())
                {
                    Notificar("Alerta", "Nenhuma rodada 'Em Apostas' encontrada para este campeonato."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Pode ser sucesso com dados vazios e alerta
                    apiResponse.Data = Enumerable.Empty<Rodada>();
                }
                else
                {
                    apiResponse.Success = true;
                    apiResponse.Data = rodadas;
                }
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodadas em apostas.");
                Notificar("Erro", $"Erro interno ao obter rodadas em apostas: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>(); // Garante que Data não é nula
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }
    }
}
