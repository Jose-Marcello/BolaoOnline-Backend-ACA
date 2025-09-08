// Localização: ApostasApp.Core.Application.Services/Apostas/ApostaRodadaService.cs

using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.DTOs.Jogos; // Mantenha se necessário para outros métodos
using ApostasApp.Core.Application.DTOs.Palpites; // Mantenha se necessário para outros métodos
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Apostas;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Apostas; // Modelos de domínio (ex: ApostaRodada, Palpite)
using ApostasApp.Core.Domain.Models.Base;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Mantenha se necessário
using ApostasApp.Core.Domain.Models.Jogos; // Modelos de domínio (ex: Jogo, Equipe)
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Domain.Models.Rodadas; // Modelos de domínio (ex: Rodada, StatusRodada)
using AutoMapper;
using Microsoft.EntityFrameworkCore; // Para .Include()
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApostasApp.Core.Application.Services.Apostas
{
    /// <summary>
    /// ApostaRodadaService é responsável pela lógica de negócio de submissão, consulta e geração de apostas de rodada.
    /// </summary>
    public class ApostaRodadaService : BaseService, IApostaRodadaService
    {
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly ICampeonatoRepository _campeonatoRepository;
        private readonly IApostadorRepository _apostadorRepository;
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IRodadaRepository _rodadaRepository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApostaRodadaService> _logger;
        private readonly IFinanceiroService _financeiroService;


        public ApostaRodadaService(
            IFinanceiroService financeiroService,
            IApostaRodadaRepository apostaRodadaRepository,
            ICampeonatoRepository campeonatoRepository,
            IApostadorRepository apostadorRepository,
            IPalpiteRepository palpiteRepository,
            IRodadaRepository rodadaRepository,
            IJogoRepository jogoRepository,
            IApostadorCampeonatoRepository apostadorCampeonatoRepository,
            IMapper mapper,
            INotificador notificador,
            IUnitOfWork uow,
            ILogger<ApostaRodadaService> logger)
                             : base(notificador, uow)
        {
            _financeiroService = financeiroService;
            _apostaRodadaRepository = apostaRodadaRepository;
            _campeonatoRepository = campeonatoRepository;
            _apostadorRepository = apostadorRepository;
            _palpiteRepository = palpiteRepository;
            _rodadaRepository = rodadaRepository;
            _jogoRepository = jogoRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gera uma ApostaRodada inicial com palpites vazios para todos os jogos de uma rodada específica
        /// para um dado apostador.
        /// </summary>
        /// <param name="apostadorCampeonatoIdString">ID do ApostadorCampeonato.</param>
        /// <param name="rodadaIdString">ID da Rodada.</param>
        /// <param name="ehApostaCampeonato">Indica se esta aposta conta para o campeonato.</param>
        /// <param name="identificadorAposta">Um nome opcional para a aposta (ex: "Minha Aposta Principal").</param>
        /// <returns>ApiResponse com a ApostaRodadaDto gerada ou erros.</returns>
        // Localização: ApostasApp.Core.Application.Services/Apostas/ApostaRodadaService.cs

       
       // Localização: ApostasApp.Core.Application.Services/Apostas/ApostaRodadaService.cs

      public async Task<ApiResponse<ApostaRodadaDto>> GerarApostaRodada(
                                                      string apostadorCampeonatoIdString,
                                                      string rodadaIdString,
                                                      bool ehApostaCampeonato,
                                                      string identificadorAposta = null)

        {
            var apiResponse = new ApiResponse<ApostaRodadaDto>(false, null);
            try
            {
                if (!Guid.TryParse(apostadorCampeonatoIdString, out Guid apostadorCampeonatoIdGuid) ||
                    !Guid.TryParse(rodadaIdString, out Guid rodadaId))
                {
                    Notificar("Erro", "IDs de apostador ou rodada inválidos.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var rodada = await _rodadaRepository.ObterPorId(rodadaId);
                if (rodada == null)
                {
                    Notificar("Erro", "Rodada não encontrada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                if (!ehApostaCampeonato && rodada.Status != StatusRodada.EmApostas)
                {
                    Notificar("Alerta", "Esta rodada não está aberta para apostas avulsas.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterPorId(apostadorCampeonatoIdGuid);
                if (apostadorCampeonato == null)
                {
                    Notificar("Erro", "Associação Apostador-Campeonato não encontrada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }


                // <<-- NOVO TRECHO: LÓGICA PARA NUMERAÇÃO SEQUENCIAL -->>
                string novoIdentificador = identificadorAposta;
                if (!ehApostaCampeonato)
                {
                    var count = await _apostaRodadaRepository.Buscar(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoIdGuid && ar.RodadaId == rodadaId && ar.EhApostaIsolada == true).CountAsync();
                    novoIdentificador = $"Aposta Avulsa #{count + 1}";
                }
                // <<-- FIM DO NOVO TRECHO -->>

                if (ehApostaCampeonato)
                {
                    novoIdentificador = $"Aposta Única"; // Ou apenas "Única"
                }

                // Busca os jogos da rodada
                var jogosDaRodada = await _jogoRepository.ObterJogosDaRodadaComPlacaresEEquipes(rodadaId);
               

                if (!jogosDaRodada.Any())
                {
                    Notificar("Alerta", "Nenhum jogo encontrado para esta rodada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // Cria a nova aposta e a adiciona ao contexto
                var novaApostaRodada = new ApostaRodada
                {
                    ApostadorCampeonatoId = apostadorCampeonatoIdGuid,
                    RodadaId = rodadaId,
                    IdentificadorAposta = identificadorAposta ?? (ehApostaCampeonato ? $"Aposta Campeonato - Rodada {rodada.NumeroRodada}" : $"Aposta Avulsa - Rodada {rodada.NumeroRodada}"),
                    EhApostaCampeonato = ehApostaCampeonato,
                    EhApostaIsolada = !ehApostaCampeonato,
                    CustoPagoApostaRodada = ehApostaCampeonato ? 0 : rodada.CustoApostaRodada,
                    PontuacaoTotalRodada = 0,
                    Enviada = false,
                    DataHoraSubmissao = null,
                    DataCriacao = DateTime.Now
                };

                await _apostaRodadaRepository.Adicionar(novaApostaRodada);

                // Cria os palpites e os adiciona ao contexto
                var palpites = new List<Palpite>();
                foreach (var jogo in jogosDaRodada)
                {
                    palpites.Add(new Palpite
                    {
                        ApostaRodadaId = novaApostaRodada.Id,
                        JogoId = jogo.Id,
                        PlacarApostaCasa = null,
                        PlacarApostaVisita = null,
                        Pontos = 0
                    });
                }
                await _palpiteRepository.AdicionarRange(palpites);

                // <<-- CORREÇÃO AQUI: Não há necessidade de consultar o banco de dados. -->>
                // A aposta e os palpites já estão na memória. Basta associá-los para o mapeamento.
                novaApostaRodada.Palpites = palpites;

                // Mapeia a aposta recém-criada (em memória) para o DTO de retorno
                var apostaRodadaDto = _mapper.Map<ApostaRodadaDto>(novaApostaRodada);

                apiResponse.Success = true;
                apiResponse.Data = apostaRodadaDto;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar ApostaRodada inicial.");
                Notificar("Erro", $"Erro interno ao gerar ApostaRodada inicial: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Gera uma ApostaRodada inicial para todos os apostadores de um campeonato
        /// para a rodada atualmente "Em Apostas". (Função de Admin)
        /// </summary>
        public async Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> GerarApostasRodadaParaTodosApostadores(string campeonatoIdString)
        {
            var apiResponse = new ApiResponse<IEnumerable<ApostaRodadaDto>>(false, null);
            try
            {
                if (!Guid.TryParse(campeonatoIdString, out Guid campeonatoId))
                {
                    Notificar("Erro", "ID do campeonato inválido.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var rodadaEmApostas = await _rodadaRepository.Buscar(r => r.CampeonatoId == campeonatoId && r.Status == StatusRodada.EmApostas)
                                                                       .FirstOrDefaultAsync();

                if (rodadaEmApostas == null)
                {
                    Notificar("Alerta", "Nenhuma rodada 'Em Apostas' encontrada para este campeonato.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostadoresNoCampeonato = await _apostadorCampeonatoRepository.Buscar(ac => ac.CampeonatoId == campeonatoId)
                                                                                         .ToListAsync();

                if (!apostadoresNoCampeonato.Any())
                {
                    Notificar("Alerta", "Nenhum apostador encontrado neste campeonato.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var generatedApostas = new List<ApostaRodadaDto>();
                foreach (var apostadorCampeonato in apostadoresNoCampeonato)
                {
                    var result = await GerarApostaRodada(
                        apostadorCampeonatoIdString: apostadorCampeonato.Id.ToString(),
                        rodadaIdString: rodadaEmApostas.Id.ToString(),
                        ehApostaCampeonato: true,
                        identificadorAposta: $"Aposta Campeonato - Rodada {rodadaEmApostas.NumeroRodada}"
                    );

                    if (result.Success && result.Data != null)
                    {
                        generatedApostas.Add(result.Data);
                    }
                    else
                    {
                        _logger.LogWarning($"Falha ao gerar ApostaRodada para apostador {apostadorCampeonato.Id}: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");
                        Notificar("Alerta", $"Falha ao gerar aposta para um apostador: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");
                    }
                }

                apiResponse.Success = true;
                apiResponse.Data = generatedApostas;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar ApostasRodada para todos os apostadores.");
                Notificar("Erro", $"Erro interno ao gerar ApostasRodada para todos os apostadores: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Adiciona uma nova ApostaRodada.
        /// </summary>
        public async Task<ApiResponse> Adicionar(ApostaRodada apostaRodada)
        {
            var apiResponse = new ApiResponse(false, null);
            try
            {
                await _apostaRodadaRepository.Adicionar(apostaRodada);
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar ApostaRodada.");
                Notificar("Erro", $"Erro interno ao adicionar ApostaRodada: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Atualiza uma ApostaRodada existente.
        /// </summary>
        public async Task<ApiResponse> Atualizar(ApostaRodada apostaRodada)
        {
            var apiResponse = new ApiResponse(false, null);
            try
            {
                await _apostaRodadaRepository.Atualizar(apostaRodada);
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar ApostaRodada.");
                Notificar("Erro", $"Erro interno ao atualizar ApostaRodada: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Marca uma ApostaRodada como submetida, registrando a data e hora.
        /// </summary>
        public async Task<ApiResponse> MarcarApostaRodadaComoSubmetida(ApostaRodada apostaRodada)
        {
            var apiResponse = new ApiResponse(false, null);
            try
            {
                apostaRodada.DataHoraSubmissao = DateTime.UtcNow;
                apostaRodada.Enviada = true;
                await _apostaRodadaRepository.Atualizar(apostaRodada);
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar ApostaRodada como submetida.");
                Notificar("Erro", $"Erro interno ao marcar ApostaRodada como submetida: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Obtém o status de envio e a data/hora da aposta de uma rodada para um usuário específico.
        /// </summary>
        public async Task<ApiResponse<ApostaRodadaStatusDto>> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            try
            {
                var apostaRodada = await _apostaRodadaRepository.ObterStatusApostaRodada(rodadaId, apostadorCampeonatoId);

                ApostaRodadaStatusDto statusDto;

                if (apostaRodada != null)
                {
                    statusDto = _mapper.Map<ApostaRodadaStatusDto>(apostaRodada);
                    statusDto.StatusAposta = 1;
                }
                else
                {
                    statusDto = new ApostaRodadaStatusDto
                    {
                        RodadaId = rodadaId.ToString(),
                        ApostadorCampeonatoId = apostadorCampeonatoId.ToString(),
                        StatusAposta = 0,
                        DataAposta = null
                    };
                }

                return new ApiResponse<ApostaRodadaStatusDto>
                {
                    Success = true,
                    Message = "Status da aposta da rodada obtido com sucesso.",
                    Data = statusDto,
                    Notifications = new List<NotificationDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter status da aposta da rodada.");

                Notificar("Erro", $"Erro ao obter status da aposta da rodada: {ex.Message}");

                return new ApiResponse<ApostaRodadaStatusDto>
                {
                    Success = false,
                    Message = "Ocorreu um erro interno ao obter o status da aposta da rodada.",
                    Data = null,
                    Notifications = ObterNotificacoesParaResposta().ToList()
                };
            }
        }

        /// <summary>
        /// Obtém as apostas de um apostador em uma rodada, formatadas para edição.
        /// Este método agora retorna uma lista de ApostaJogoVisualizacaoDto, com dados completos do jogo e palpites.
        /// </summary>
        public async Task<ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostaRodadaId)
        {
            var apiResponse = new ApiResponse<IEnumerable<ApostaJogoEdicaoDto>>(false, null);
            try
            {
                // 1. Busca a aposta específica do apostador para a rodada
                var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == rodadaId && ar.Id == apostaRodadaId)
                                                                .Include(ar => ar.Palpites)
                                                                .FirstOrDefaultAsync();

                // 2. Busca todos os jogos da rodada
                var jogosDaRodada = await _jogoRepository.ObterJogosDaRodadaComPlacaresEEquipes(rodadaId);

                if (!jogosDaRodada.Any())
                {
                    Notificar("Alerta", "Não há jogos definidos para esta rodada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostasParaEdicao = new List<ApostaJogoEdicaoDto>();

                foreach (var jogo in jogosDaRodada.OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo))
                {
                    var palpite = apostaRodada?.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);

                    apostasParaEdicao.Add(new ApostaJogoEdicaoDto
                    {
                        Id = palpite?.Id.ToString() ?? Guid.NewGuid().ToString(),
                        IdJogo = jogo.Id.ToString(),
                        EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                        SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                        EscudoMandante = jogo.EquipeCasa.Equipe.Escudo,
                        EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                        SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                        EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,
                        EstadioNome = jogo.Estadio?.Nome,
                        DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
                        HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm"),
                        StatusJogo = jogo.Status.ToString(),
                        PlacarApostaCasa = palpite?.PlacarApostaCasa,
                        PlacarApostaVisita = palpite?.PlacarApostaVisita,
                        Enviada = apostaRodada?.Enviada ?? false,
                    });
                }

                apiResponse.Success = true;
                apiResponse.Data = apostasParaEdicao;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter apostas do apostador na rodada para edição.");
                Notificar("Erro", $"Erro interno ao obter apostas para edição: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Obtém as apostas de um apostador em uma rodada, formatadas para edição.
        /// Este método agora retorna uma lista de ApostaJogoVisualizacaoDto, com dados completos do jogo e palpites.
        /// </summary>
        public async Task<ApiResponse<ApostaRodadaResultadosDto>> ObterResultadosDaRodada(Guid rodadaId, Guid apostaRodadaId)
        {
            var apiResponse = new ApiResponse<ApostaRodadaResultadosDto>(false, null);
            try
            {
                var jogosDaRodada = await _jogoRepository.Buscar(j => j.RodadaId == rodadaId)
                    .Include(j => j.EquipeCasa).ThenInclude(ec => ec.Equipe)
                    .Include(j => j.EquipeVisitante).ThenInclude(ev => ev.Equipe)
                    .Include(j => j.Estadio)
                    .ToListAsync();

                var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.Id == apostaRodadaId)
                    .Include(ar => ar.Palpites)
                    .FirstOrDefaultAsync();

                if (apostaRodada == null)
                {
                    Notificar("Alerta", "Aposta de rodada não encontrada.");
                    return new ApiResponse<ApostaRodadaResultadosDto> { Success = false, Notifications = ObterNotificacoesParaResposta().ToList() };
                }

                var resultados = new ApostaRodadaResultadosDto
                {
                    ApostaRodadaId = apostaRodada.Id.ToString(),
                    PontuacaoTotalRodada = apostaRodada.PontuacaoTotalRodada,
                    JogosComResultados = jogosDaRodada.Select(jogo =>
                    {
                        var palpite = apostaRodada.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);
                        return new ApostaJogoResultadosDto
                        {
                            Id = palpite?.Id.ToString(),
                            IdJogo = jogo.Id.ToString(),
                            EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                            SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                            EscudoMandante = jogo.EquipeCasa.Equipe.Escudo,
                            EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                            SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                            EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,
                            EstadioNome = jogo.Estadio?.Nome,
                            DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
                            HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm"),
                            StatusJogo = jogo.Status.ToString(),
                            PlacarRealCasa = jogo.PlacarCasa,
                            PlacarRealVisita = jogo.PlacarVisita,
                            PlacarApostaCasa = palpite?.PlacarApostaCasa,
                            PlacarApostaVisita = palpite?.PlacarApostaVisita,
                            Pontuacao = palpite?.Pontos ?? 0
                        };
                    }).ToList()
                };

                apiResponse.Success = true;
                apiResponse.Data = resultados;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter resultados da rodada.");
                Notificar("Erro", $"Erro interno ao obter resultados da rodada: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }
        /// <summary>
        /// Obtém as apostas de um apostador em uma rodada.
        /// Este método é para listar as "ApostasRodada" de um apostador para uma rodada específica.
        /// </summary>
        public async Task<ApiResponse<IEnumerable<ApostaRodadaDto>>> ObterApostasRodadaPorApostador(Guid rodadaId, Guid? apostadorCampeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<ApostaRodadaDto>>(false, null);
            try
            {
                var apostasRodada = await _apostaRodadaRepository.Buscar(ar =>
                                                                           ar.RodadaId == rodadaId &&
                                                                           ar.ApostadorCampeonatoId == apostadorCampeonatoId)
                                                               .Include(ar => ar.Palpites)
                                                                   .ThenInclude(p => p.Jogo) // Inclui o Jogo dentro de cada Palpite
                                                                       .ThenInclude(j => j.EquipeCasa) // Inclui EquipeCasa do Jogo
                                                                           .ThenInclude(ec => ec.Equipe) // Inclui a entidade Equipe da EquipeCasa
                                                               .Include(ar => ar.Palpites)
                                                                   .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                                       .ThenInclude(j => j.EquipeVisitante) // Inclui EquipeVisitante do Jogo
                                                                           .ThenInclude(ev => ev.Equipe) // Inclui a entidade Equipe da EquipeVisitante
                                                               .Include(ar => ar.Palpites)
                                                                   .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                                       .ThenInclude(j => j.Estadio) // Inclui o Estádio do Jogo
                                                               .ToListAsync();

                if (!apostasRodada.Any())
                {
                    Notificar("Alerta", "Nenhuma aposta de rodada encontrada para este apostador nesta rodada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostasRodadaDto = _mapper.Map<IEnumerable<ApostaRodadaDto>>(apostasRodada);

                apiResponse.Success = true;
                apiResponse.Data = apostasRodadaDto;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter apostas de rodada por apostador.");
                Notificar("Erro", $"Erro interno ao obter apostas de rodada por apostador: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Salva ou atualiza as apostas de uma rodada para um apostador.
        /// </summary>
        public async Task<ApiResponse<ApostaRodadaDto>> SalvarApostas(SalvarApostaRequestDto salvarApostaDto)
        {
            // O tipo de retorno da ApiResponse foi alterado para ApostaRodadaDto
            var apiResponse = new ApiResponse<ApostaRodadaDto>(false, null);
            try
            {
                _logger.LogInformation("Iniciando SalvarApostas.");

                if (salvarApostaDto == null || !salvarApostaDto.ApostasJogos.Any())
                {
                    Notificar("Alerta", "Nenhuma aposta foi enviada para salvar.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var rodada = await _rodadaRepository.ObterPorId(Guid.Parse(salvarApostaDto.RodadaId));
                if (rodada == null)
                {
                    Notificar("Erro", "Rodada não encontrada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                if (rodada.Status != StatusRodada.EmApostas)
                {
                    Notificar("Alerta", "Não é possível salvar apostas para esta rodada. Ela não está mais 'Em Apostas'.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // --- VALIDAÇÃO DA REGRA DE NEGÓCIO: Ao menos 3 empates/vitória dos visitantes ---
                int empates = 0;
                int vitoriasVisitante = 0;

                foreach (var apostaJogoDto in salvarApostaDto.ApostasJogos)
                {
                    // Certifique-se de que os placares não são nulos para a validação
                    if (apostaJogoDto.PlacarCasa.HasValue && apostaJogoDto.PlacarVisitante.HasValue)
                    {
                        if (apostaJogoDto.PlacarCasa.Value == apostaJogoDto.PlacarVisitante.Value)
                        {
                            empates++;
                        }
                        else if (apostaJogoDto.PlacarVisitante.Value > apostaJogoDto.PlacarCasa.Value)
                        {
                            vitoriasVisitante++;
                        }
                    }
                }

                if ((empates + vitoriasVisitante) < 3)
                {
                    Notificar("Alerta", "Sua aposta deve conter no mínimo 3 resultados de empate ou vitória do time visitante.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse; // Retorna com erro de validação
                }
                // --- FIM DA VALIDAÇÃO ---

                // Busca a apostaRodada existente ou cria uma nova
                // Inclui os palpites existentes para poder atualizá-los
                var apostaRodada = await _apostaRodadaRepository.Buscar(ar =>
                                                                           ar.RodadaId == Guid.Parse(salvarApostaDto.RodadaId) &&
                                                                           ar.ApostadorCampeonatoId == Guid.Parse(salvarApostaDto.ApostadorCampeonatoId))
                                                               .Include(ar => ar.Palpites) // Inclui palpites para poder iterar e atualizar
                                                               .FirstOrDefaultAsync();

                if (apostaRodada == null)
                {
                    apostaRodada = new ApostaRodada
                    {
                        RodadaId = Guid.Parse(salvarApostaDto.RodadaId),
                        ApostadorCampeonatoId = Guid.Parse(salvarApostaDto.ApostadorCampeonatoId),
                        EhApostaCampeonato = salvarApostaDto.EhCampeonato,
                        EhApostaIsolada = false, // Assumindo que apostas salvas aqui não são isoladas por padrão
                        Enviada = false, // Será marcado como true mais abaixo
                        PontuacaoTotalRodada = 0,
                        DataCriacao = DateTime.Now,
                        IdentificadorAposta = salvarApostaDto.IdentificadorAposta
                    };
                    await _apostaRodadaRepository.Adicionar(apostaRodada);
                }

                apostaRodada.DataHoraSubmissao = DateTime.UtcNow; // Define a data de submissão
                apostaRodada.Enviada = true; // Marca como enviada ao salvar

                // Atualiza a apostaRodada principal (para salvar DataHoraSubmissao e Enviada)
                await _apostaRodadaRepository.Atualizar(apostaRodada);

                // Itera sobre os palpites enviados e atualiza/cria
                foreach (var apostaJogoDto in salvarApostaDto.ApostasJogos)
                {
                    var jogoId = Guid.Parse(apostaJogoDto.JogoId);
                    var palpite = apostaRodada.Palpites.FirstOrDefault(p => p.JogoId == jogoId);

                    if (palpite == null)
                    {
                        palpite = new Palpite
                        {
                            ApostaRodadaId = apostaRodada.Id,
                            JogoId = jogoId,
                            PlacarApostaCasa = apostaJogoDto.PlacarCasa,
                            PlacarApostaVisita = apostaJogoDto.PlacarVisitante,
                            Pontos = 0 // Pontos iniciais
                        };
                        await _palpiteRepository.Adicionar(palpite);
                    }
                    else
                    {
                        palpite.PlacarApostaCasa = apostaJogoDto.PlacarCasa;
                        palpite.PlacarApostaVisita = apostaJogoDto.PlacarVisitante;
                        await _palpiteRepository.Atualizar(palpite);
                    }
                }

                // Salva todas as alterações no banco de dados
                if (!await CommitAsync())
                {
                    Notificar("Erro", "Falha ao persistir dados da aposta.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // --- RETORNA A APOSTA ATUALIZADA COM PALPITES E JOGOS ---
                // Rebusca a apostaRodada com todos os includes necessários para o DTO de retorno
                var apostaRodadaCompleta = await _apostaRodadaRepository.Buscar(ar => ar.Id == apostaRodada.Id)
                                                                           .Include(ar => ar.Palpites)
                                                                               .ThenInclude(p => p.Jogo) // Inclui o Jogo dentro de cada Palpite
                                                                                   .ThenInclude(j => j.EquipeCasa) // Inclui EquipeCasa do Jogo
                                                                                       .ThenInclude(ec => ec.Equipe) // Inclui a entidade Equipe da EquipeCasa
                                                                           .Include(ar => ar.Palpites)
                                                                               .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                                                   .ThenInclude(j => j.EquipeVisitante) // Inclui EquipeVisitante do Jogo
                                                                                       .ThenInclude(ev => ev.Equipe) // Inclui a entidade Equipe da EquipeVisitante
                                                                           .Include(ar => ar.Palpites)
                                                                               .ThenInclude(p => p.Jogo) // Re-inclui Jogo para a próxima ThenInclude
                                                                                   .ThenInclude(j => j.Estadio) // Inclui o Estádio do Jogo
                                                                           .FirstOrDefaultAsync();

                if (apostaRodadaCompleta == null)
                {
                    Notificar("Erro", "Aposta salva, mas não foi possível recuperá-la para retorno.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // Mapeia a entidade completa para o DTO de retorno
                apiResponse.Data = _mapper.Map<ApostaRodadaDto>(apostaRodadaCompleta);
                apiResponse.Success = true;
                apiResponse.Message = "Apostas salvas com sucesso!";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList(); // Inclui quaisquer notificações que possam ter sido adicionadas
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar apostas.");
                Notificar("Erro", $"Erro interno ao salvar apostas: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }


        /// <summary>
        /// Executa a transação completa para criar uma aposta avulsa no backend.
        /// </summary>
        public async Task<ApiResponse<ApostaRodadaDto>> ExecutarTransacaoApostaAvulsa(CriarApostaAvulsaRequestDto requestDto)
        {
            var apiResponse = new ApiResponse<ApostaRodadaDto>(false, null);
            try
            {
                _logger.LogInformation("Iniciando transação para criar aposta avulsa. ApostadorId: {ApostadorId}, CampeonatoId: {CampeonatoId}", requestDto.ApostadorId, requestDto.CampeonatoId);

                // 1. Validar e obter a rodada, o apostador e o campeonato
                var rodada = await _rodadaRepository.ObterPorId(Guid.Parse(requestDto.RodadaId));
                var apostador = await _apostadorRepository.ObterPorIdComSaldo(Guid.Parse(requestDto.ApostadorId));
                var campeonato = await _campeonatoRepository.ObterPorId(Guid.Parse(requestDto.CampeonatoId));

                if (rodada == null || apostador == null || campeonato == null)
                {
                    Notificar("Erro", "Dados essenciais para a transação não encontrados.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // 2. Verificar saldo do apostador
                if (apostador.Saldo.Valor < requestDto.CustoAposta)
                {
                    Notificar("Erro", "Saldo insuficiente para a aposta.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // 3. Criar ou obter a associação ApostadorCampeonato
                var apostadorCampeonato = await _apostadorCampeonatoRepository.Buscar(ac => ac.CampeonatoId == campeonato.Id && ac.ApostadorId == apostador.Id)
                                                                            .FirstOrDefaultAsync();

                if (apostadorCampeonato == null)
                {
                    apostadorCampeonato = new ApostadorCampeonato
                    {
                        ApostadorId = apostador.Id,
                        CampeonatoId = campeonato.Id,
                        DataInscricao = DateTime.Now
                    };
                    await _apostadorCampeonatoRepository.Adicionar(apostadorCampeonato);
                    _logger.LogInformation($"Nova associação ApostadorCampeonato criada para ApostadorId: {requestDto.ApostadorId} e CampeonatoId: {requestDto.CampeonatoId}");
                }

                // <<-- CORREÇÃO DA LÓGICA: Validar apenas se for uma aposta de campeonato (não é o caso aqui) -->>
                // A lógica do front-end neste ponto é criar uma aposta avulsa, então não precisamos desta validação
                // para este fluxo específico. No entanto, se o método fosse usado para criar uma aposta de campeonato,
                // a validação otimizada seria a seguinte:
                /*
                bool apostaCampeonatoJaExiste = await _apostaRodadaRepository.Buscar(ar => 
                                                                                    ar.ApostadorCampeonatoId == apostadorCampeonato.Id && 
                                                                                    ar.RodadaId == rodada.Id && 
                                                                                    ar.EhApostaCampeonato == true)
                                                                            .AnyAsync();
                if (apostaCampeonatoJaExiste)
                {
                    Notificar("Alerta", "Já existe uma aposta de campeonato para este apostador nesta rodada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                */

                // 4. Debitar saldo e registrar transação financeira
                var debitoResponse = await _financeiroService.DebitarSaldoAsync(
                    apostador.Id,
                    requestDto.CustoAposta,
                    TipoTransacao.ApostaRodada,
                    $"Aposta avulsa na rodada {rodada.NumeroRodada} do campeonato {campeonato.Nome}"
                );
                if (!debitoResponse.Success)
                {
                    apiResponse.Success = false;
                    apiResponse.Notifications = debitoResponse.Notifications;
                    return apiResponse;
                }

                // 5. Gerar a aposta avulsa e os palpites iniciais
                var apostaRodadaResponse = await GerarApostaRodada(
                    apostadorCampeonatoIdString: apostadorCampeonato.Id.ToString(),
                    rodadaIdString: rodada.Id.ToString(),
                    ehApostaCampeonato: false,
                    identificadorAposta: $"Aposta Avulsa - Rodada {rodada.NumeroRodada}"
                );

                if (!apostaRodadaResponse.Success || apostaRodadaResponse.Data == null)
                {
                    Notificar("Erro", "Falha ao gerar aposta avulsa inicial.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                // 6. Commit de todas as operações pendentes
                if (!await CommitAsync())
                {
                    Notificar("Erro", "Falha ao persistir dados da aposta.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                apiResponse.Success = true;
                apiResponse.Data = apostaRodadaResponse.Data;
                Notificar("Sucesso", "Transação de aposta avulsa concluída com sucesso!");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                _logger.LogInformation("Transação de aposta avulsa concluída com sucesso.");
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na transação de aposta avulsa.");
                Notificar("Erro", $"Erro interno na transação de aposta avulsa: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        public async Task<ApostasAvulsasTotaisDto> ObterTotaisApostasAvulsas(Guid rodadaId)
        {
            // 1. O serviço chama o repositório e recebe um objeto do domínio.
            var totaisDominio = await _apostaRodadaRepository.ObterTotaisApostasAvulsas(rodadaId);

            // 2. Mapeia o objeto do domínio para o DTO de aplicação.
            var totaisDto = new ApostasAvulsasTotaisDto
            {
                NumeroDeApostas = totaisDominio.NumeroDeApostas,
                ValorTotal = totaisDominio.ValorTotal
            };

            return totaisDto;
        }

        public async Task<ApostasCampeonatoTotaisDto> ObterTotaisCampeonato(Guid campeonatoId)
        {
            // 1. O serviço chama o repositório e recebe um objeto do domínio.
            var totaisDominio = await _apostaRodadaRepository.ObterTotaisCampeonato(campeonatoId);

            // 2. Mapeia o objeto do domínio para o DTO de aplicação.
            var totaisDto = new ApostasCampeonatoTotaisDto
            {
                NumeroDeApostadores = totaisDominio.NumeroDeApostadores,
                ValorTotalArrecadado = totaisDominio.ValorTotalArrecadado
            };

            return totaisDto;
        }

        
    }

}


