// Localização: ApostasApp.Core.Application.Services.Campeonatos/CampeonatoService.cs

using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.Models; // Para ApiResponse
// using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Verifique se esta interface é realmente necessária ou se é um using antigo - REMOVIDO SE NÃO USADO
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
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
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
                NotificarErro($"Erro interno ao adicionar campeonato: {ex.Message}"); // CORRIGIDO: Usando NotificarErro
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
                NotificarErro($"Erro interno ao atualizar campeonato: {ex.Message}"); // CORRIGIDO: Usando NotificarErro
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
                NotificarErro($"Erro interno ao remover campeonato: {ex.Message}"); // CORRIGIDO: Usando NotificarErro
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
                NotificarErro($"Erro interno ao obter campeonato por ID: {ex.Message}"); // CORRIGIDO: Usando NotificarErro
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
                NotificarErro($"Erro interno ao obter todos os campeonatos: {ex.Message}"); // CORRIGIDO: Usando NotificarErro
                return Enumerable.Empty<CampeonatoDto>();
            }
        }

        public async Task<ApiResponse<IEnumerable<CampeonatoDto>>> GetAvailableCampeonatos(string? userId)
        {
            var apiResponse = new ApiResponse<IEnumerable<CampeonatoDto>>();
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
                    NotificarAlerta("Nenhum campeonato disponível encontrado na base de dados."); // CORRIGIDO: Usando NotificarAlerta
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
                NotificarErro($"Erro interno ao obter campeonatos disponíveis: {ex.Message}"); // CORRIGIDO: Usando NotificarErro
                apiResponse.Success = false; // Em caso de exceção, é um erro
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = Enumerable.Empty<CampeonatoDto>(); // Garante que Data não é nula
                return apiResponse;
            }
        }

        // Método AderirCampeonatoAsync CORRIGIDO
        public async Task<ApiResponse<bool>> AderirCampeonatoAsync(Guid apostadorId, Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                // 1. Validar Campeonato
                var campeonato = await _campeonatoRepository.ObterPorId(campeonatoId);
                if (campeonato == null)
                {
                    NotificarErro("Campeonato não encontrado.");
                    apiResponse.Success = false;
                    apiResponse.Data = false;
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // 2. Validar Adesão Existente
                var adesaoExistente = await _apostadorCampeonatoRepository.ObterApostadorCampeonatoPorApostadorECampeonato(apostadorId, campeonatoId);
                if (adesaoExistente != null)
                {
                    NotificarAlerta("Você já aderiu a este campeonato.");
                    apiResponse.Success = true; // É um sucesso, mas com alerta
                    apiResponse.Data = true;
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // 3. Validar Apostador
                var apostador = await _apostadorRepository.ObterPorId(apostadorId);
                if (apostador == null)
                {
                    NotificarErro("Apostador não encontrado.");
                    apiResponse.Success = false;
                    apiResponse.Data = false;
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // INÍCIO DA TRANSAÇÃO (se aplicável ao seu UoW)
                // Se seu IUnitOfWork suporta transações explícitas, inicie aqui.
                // Ex: _uow.BeginTransaction();

                // 4. Débito de Saldo (se houver custo de adesão)
                if (campeonato.CustoAdesao.HasValue && campeonato.CustoAdesao.Value > 0)
                {
                    var debitoResponse = await _financeiroService.DebitarSaldoAsync(
                        apostadorId,
                        campeonato.CustoAdesao.Value,
                        TipoTransacao.AdesaoCampeonato,
                        $"Adesão ao campeonato: {campeonato.Nome}");

                    if (!debitoResponse.Success)
                    {
                        // Se o débito falhar, propaga as notificações de erro do FinanceiroService
                        // E NÃO CONTINUA A OPERAÇÃO
                        apiResponse.Notifications = debitoResponse.Notifications.ToList();
                        apiResponse.Success = false;
                        apiResponse.Data = false;
                        // Se houver transação, faça o rollback aqui: _uow.RollbackTransaction();
                        return apiResponse;
                    }
                    // Se o débito foi um sucesso, as notificações de sucesso do FinanceiroService
                    // já foram adicionadas ao notificador. Elas serão coletadas no final.
                }

                // 5. Criar o registro de ApostadorCampeonato
                var novaAdesao = new ApostadorCampeonato(apostadorId, campeonato.Id)
                {
                    DataInscricao = DateTime.Now,
                    CustoAdesaoPago = campeonato.CustoAdesao.HasValue && campeonato.CustoAdesao.Value > 0
                };
                _apostadorCampeonatoRepository.Adicionar(novaAdesao);

                // 6. Encontrar a Rodada "Em Apostas" para este campeonato e criar ApostaRodada e Palpites
                var rodadasEmApostas = await _rodadaRepository.ObterRodadasEmApostaPorCampeonato(campeonato.Id);
                var rodadaParaApostaInicial = rodadasEmApostas?.FirstOrDefault();

                if (rodadaParaApostaInicial == null)
                {
                    NotificarAlerta("Nenhuma rodada 'Em Apostas' encontrada para este campeonato. A aposta inicial não será criada.");
                    // A adesão e o débito (se houver) ainda são válidos, então a operação principal continua como sucesso.
                }
                else
                {
                    var apostaRodada = new ApostaRodada(novaAdesao.Id, rodadaParaApostaInicial.Id);
                    _apostaRodadaRepository.Adicionar(apostaRodada);

                    var jogosDaRodada = await _jogoRepository.ObterJogosDaRodada(rodadaParaApostaInicial.Id);

                    if (jogosDaRodada != null && jogosDaRodada.Any())
                    {
                        foreach (var jogo in jogosDaRodada)
                        {
                            var palpite = new Palpite(apostaRodada.Id, jogo.Id)
                            {
                                ApostaRodadaId = apostaRodada.Id,
                                JogoId = jogo.Id,
                                PlacarApostaCasa = null,
                                PlacarApostaVisita = null,
                                Pontos = 0
                            };
                            _palpiteRepository.Adicionar(palpite);
                        }
                    }
                    else
                    {
                        NotificarAlerta("Nenhum jogo encontrado para a rodada 'Em Apostas'. Palpites iniciais não serão criados.");
                    }
                }

                // 7. Commit de todas as operações pendentes
                var saved = await CommitAsync();

                if (saved)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = true;
                    NotificarSucesso("Adesão ao campeonato e preparação para apostas realizada com sucesso!");
                    // Se houver transação, faça o commit aqui: _uow.CommitTransaction();
                }
                else
                {
                    // Se o commit falhar, notifica um erro genérico de persistência
                    NotificarErro("Não foi possível persistir as alterações para vincular o apostador ao campeonato ou preparar as apostas.");
                    apiResponse.Success = false;
                    apiResponse.Data = false;
                    // Se houver transação, faça o rollback aqui: _uow.RollbackTransaction();
                }

                // Coleta as notificações acumuladas (sucesso, alerta ou erro do commit)
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao aderir ao campeonato.");
                NotificarErro($"Erro interno ao aderir ao campeonato: {ex.Message}");
                apiResponse.Success = false;
                apiResponse.Data = false;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                // Se houver transação, faça o rollback aqui: _uow.RollbackTransaction();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<CampeonatoDto?>> GetDetalhesCampeonato(Guid id)
        {
            var apiResponse = new ApiResponse<CampeonatoDto?>();
            try
            {
                var campeonato = await _campeonatoRepository.ObterPorId(id);

                if (campeonato == null)
                {
                    NotificarAlerta($"Campeonato com ID '{id}' não encontrado.");
                    apiResponse.Success = false; // Se o item não foi encontrado, isso é uma falha específica
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
                NotificarErro($"Erro interno ao obter detalhes do campeonato: {ex.Message}");
                apiResponse.Success = false;
                apiResponse.Data = null;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> GetRodadasCorrentes(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>();

            try
            {
                var rodadas = await _rodadaRepository.ObterRodadasCorrentePorCampeonato(campeonatoId);
                if (rodadas == null || !rodadas.Any())
                {
                    NotificarAlerta("Nenhuma rodada 'Corrente' encontrada para este campeonato.");
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
                NotificarErro($"Erro interno ao obter rodada(s) corrente(s): {ex.Message}");
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>(); // Garante que Data não é nula
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> GetRodadasEmApostas(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>();
            try
            {
                var rodadas = await _rodadaRepository.ObterRodadasEmApostaPorCampeonato(campeonatoId);
                if (rodadas == null || !rodadas.Any())
                {
                    NotificarAlerta("Nenhuma rodada 'Em Apostas' encontrada para este campeonato.");
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
                NotificarErro($"Erro interno ao obter rodadas em apostas: {ex.Message}");
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>(); // Garante que Data não é nula
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }
    }
}
