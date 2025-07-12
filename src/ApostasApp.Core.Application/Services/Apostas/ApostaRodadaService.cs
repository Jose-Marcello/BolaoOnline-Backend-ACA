// Localização: ApostasApp.Core.Application.Services/Apostas/ApostaRodadaService.cs

using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.ApostasRodada;
using ApostasApp.Core.Application.DTOs.Jogos;
using ApostasApp.Core.Application.DTOs.Palpites;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Models.Jogos;
using ApostasApp.Core.Domain.Models.Notificacoes;
using ApostasApp.Core.Domain.Models.Rodadas;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Apostas
{
    /// <summary>
    /// ApostaRodadaService é responsável pela lógica de negócio de submissão, consulta e geração de apostas de rodada.
    /// </summary>
    public class ApostaRodadaService : BaseService, IApostaRodadaService
    {
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IRodadaRepository _rodadaRepository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApostaRodadaService> _logger;

        public ApostaRodadaService(
            IApostaRodadaRepository apostaRodadaRepository,
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
            _apostaRodadaRepository = apostaRodadaRepository;
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
        public async Task<ApiResponse<ApostaRodadaDto>> GerarApostaRodadaInicial(
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

                var existingApostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoIdGuid &&
                                                                                     ar.RodadaId == rodadaId &&
                                                                                     ar.EhApostaCampeonato == ehApostaCampeonato)
                                                                     .Include(ar => ar.Palpites)
                                                                     .FirstOrDefaultAsync();

                if (existingApostaRodada != null)
                {
                    if (!existingApostaRodada.Enviada)
                    {
                        _logger.LogInformation($"ApostaRodada existente (ID: {existingApostaRodada.Id}) encontrada para o apostador {apostadorCampeonatoIdGuid} na rodada {rodadaId}. Retornando para preenchimento.");
                        var existingDto = _mapper.Map<ApostaRodadaDto>(existingApostaRodada);
                        apiResponse.Success = true;
                        apiResponse.Data = existingDto;
                        apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                        return apiResponse;
                    }
                    else
                    {
                        Notificar("Alerta", "Já existe uma ApostaRodada enviada para este apostador nesta rodada.");
                        apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                        return apiResponse;
                    }
                }

                var jogosDaRodada = await _jogoRepository.Buscar(j => j.RodadaId == rodadaId)
                    .Include(j => j.EquipeCasa)
                        .ThenInclude(ec => ec.Equipe)
                    .Include(j => j.EquipeVisitante)
                        .ThenInclude(ec => ec.Equipe)
                    .Include(j => j.Estadio)
                    .ToListAsync();

                if (!jogosDaRodada.Any())
                {
                    Notificar("Alerta", "Nenhum jogo encontrado para esta rodada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

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
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

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
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostaRodadaDto = _mapper.Map<ApostaRodadaDto>(novaApostaRodada);
                apostaRodadaDto.Palpites = _mapper.Map<IEnumerable<PalpiteDto>>(palpites);

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
                    var result = await GerarApostaRodadaInicial(
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

                Notificar(new NotificationDto { Tipo = "Erro", Mensagem = $"Erro ao obter status da aposta da rodada: {ex.Message}" });

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
        /// </summary>
        public async Task<ApiResponse<IEnumerable<ApostaJogoDto>>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<ApostaJogoDto>>(false, null);
            try
            {
                var rodada = await _rodadaRepository.ObterRodadaComJogosEEquipes(rodadaId);
                if (rodada == null)
                {
                    Notificar("Alerta", "Rodada não encontrada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                if (rodada.Status != StatusRodada.EmApostas)
                {
                    Notificar("Alerta", "Rodada não está mais disponível para apostas.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == rodadaId && ar.ApostadorCampeonatoId == apostadorCampeonatoId)
                                                                 .Include(ar => ar.Palpites)
                                                                 .FirstOrDefaultAsync();

                var apostasParaEdicao = new List<ApostaJogoDto>();

                if (apostaRodada == null || !apostaRodada.Palpites.Any())
                {
                    foreach (var jogo in rodada.JogosRodada.OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo))
                    {
                        apostasParaEdicao.Add(new ApostaJogoDto
                        {
                            Id = Guid.Empty.ToString(),
                            IdJogo = jogo.Id.ToString(),
                            EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                            SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                            EscudoMandante = jogo.EquipeCasa.Equipe.Escudo,
                            PlacarMandante = "",
                            EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                            SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                            EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,
                            PlacarVisitante = "",
                            DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
                            HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm")
                        });
                    }
                    apiResponse.Success = true;
                    apiResponse.Data = apostasParaEdicao;
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                foreach (var jogo in rodada.JogosRodada.OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo))
                {
                    var palpite = apostaRodada.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);

                    apostasParaEdicao.Add(new ApostaJogoDto
                    {
                        Id = palpite?.Id.ToString() ?? Guid.Empty.ToString(),
                        IdJogo = jogo.Id.ToString(),
                        EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                        SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                        EscudoMandante = jogo.EquipeCasa.Equipe.Escudo,
                        PlacarMandante = (palpite?.PlacarApostaCasa ?? 0) == 0 && !apostaRodada.Enviada
                                         ? "" : (palpite?.PlacarApostaCasa ?? 0).ToString(),
                        EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                        SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                        EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,
                        PlacarVisitante = (palpite?.PlacarApostaVisita ?? 0) == 0 && !apostaRodada.Enviada
                                         ? "" : (palpite?.PlacarApostaVisita ?? 0).ToString(),
                        DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
                        HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm")
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
        /// Obtém as apostas de um apostador em uma rodada, formatadas para visualização (com placares reais e pontuação).
        /// </summary>
        public async Task<ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>> ObterApostasDoApostadorNaRodadaParaVisualizacao(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<ApostaJogoVisualizacaoDto>>(false, null);
            try
            {
                var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == rodadaId && ar.ApostadorCampeonatoId == apostadorCampeonatoId)
                                                                 .FirstOrDefaultAsync();

                var jogosDaRodada = await _rodadaRepository.ObterJogosDaRodadaComPlacaresEEquipes(rodadaId);
                if (!jogosDaRodada.Any())
                {
                    Notificar("Alerta", "Não há jogos definidos para esta rodada.");
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                var apostasParaVisualizacao = new List<ApostaJogoVisualizacaoDto>();

                foreach (var jogo in jogosDaRodada.OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo))
                {
                    var palpite = apostaRodada?.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);

                    apostasParaVisualizacao.Add(new ApostaJogoVisualizacaoDto
                    {
                        Id = palpite?.Id.ToString() ?? Guid.Empty.ToString(),
                        IdJogo = jogo.Id.ToString(),
                        EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                        SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                        EscudoMandante = jogo.EquipeCasa.Equipe.Escudo,
                        PlacarRealCasa = jogo.PlacarCasa,
                        PlacarApostaCasa = palpite?.PlacarApostaCasa,
                        EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                        SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                        EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,
                        PlacarRealVisita = jogo.PlacarVisita,
                        PlacarApostaVisita = palpite?.PlacarApostaVisita,
                        DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
                        HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm"),
                        StatusJogo = jogo.Status.ToString(),
                        Enviada = apostaRodada?.Enviada ?? false,
                        Pontuacao = palpite?.Pontos ?? 0
                    });
                }

                apiResponse.Success = true;
                apiResponse.Data = apostasParaVisualizacao;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter apostas do apostador na rodada para visualização.");
                Notificar("Erro", $"Erro interno ao obter apostas para visualização: {ex.Message}");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
        }

        /// <summary>
        /// Salva ou atualiza as apostas de uma rodada para um apostador.
        /// </summary>
        public async Task<ApiResponse> SalvarApostas(SalvarApostaRequestDto salvarApostaDto)
        {
            var apiResponse = new ApiResponse(false, null);
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

                var apostaRodada = await _apostaRodadaRepository.Buscar(ar =>
                        ar.RodadaId == Guid.Parse(salvarApostaDto.RodadaId) &&
                        ar.ApostadorCampeonatoId == Guid.Parse(salvarApostaDto.ApostadorCampeonatoId))
                        .Include(ar => ar.Palpites)
                        .FirstOrDefaultAsync();

                if (apostaRodada == null)
                {
                    apostaRodada = new ApostaRodada
                    {
                        RodadaId = Guid.Parse(salvarApostaDto.RodadaId),
                        ApostadorCampeonatoId = Guid.Parse(salvarApostaDto.ApostadorCampeonatoId),
                        EhApostaCampeonato = salvarApostaDto.EhCampeonato,
                        EhApostaIsolada = false,
                        Enviada = false,
                        PontuacaoTotalRodada = 0,
                        DataCriacao = DateTime.Now
                    };
                    await _apostaRodadaRepository.Adicionar(apostaRodada);
                }

                apostaRodada.DataHoraSubmissao = DateTime.UtcNow;
                apostaRodada.Enviada = true;

                await _apostaRodadaRepository.Atualizar(apostaRodada);

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
                            Pontos = 0
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

                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                apiResponse.Success = true;
                apiResponse.Message = "Apostas salvas com sucesso!";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
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
    }
}
