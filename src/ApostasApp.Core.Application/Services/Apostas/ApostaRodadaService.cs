using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces.Apostas; // Para IApostaRodadaService
using ApostasApp.Core.Domain.Interfaces; // Para INotificador, IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Interfaces.Jogos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Apostas; // Para ApostaRodada, Palpite
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Para IRodadaRepository
using ApostasApp.Core.Domain.Models.Rodadas; // Para StatusRodada
using AutoMapper;
using Microsoft.EntityFrameworkCore; // Para FirstOrDefaultAsync, ToListAsync

namespace ApostasApp.Core.Application.Services.Apostas
{
    /// <summary>
    /// ApostaRodadaService é responsável pela lógica de negócio de submissão e consulta de apostas de rodada.
    /// </summary>
    public class ApostaRodadaService : BaseService, IApostaRodadaService
    {
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IRodadaRepository _rodadaRepository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IMapper _mapper;

        public ApostaRodadaService(IApostaRodadaRepository apostaRodadaRepository,
                                   IPalpiteRepository palpiteRepository,
                                   IRodadaRepository rodadaRepository,
                                   IJogoRepository jogoRepository,
                                   IMapper mapper,
                                   INotificador notificador,
                                   IUnitOfWork uow) : base(notificador, uow)
        {
            _apostaRodadaRepository = apostaRodadaRepository;
            _palpiteRepository = palpiteRepository;
            _rodadaRepository = rodadaRepository;
            _jogoRepository = jogoRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Adiciona uma nova ApostaRodada.
        /// </summary>
        /// <param name="apostaRodada">A entidade ApostaRodada a ser adicionada.</param>
        public async Task Adicionar(ApostaRodada apostaRodada)
        {
            await _apostaRodadaRepository.Adicionar(apostaRodada);
            await Commit();
        }

        /// <summary>
        /// Atualiza uma ApostaRodada existente.
        /// </summary>
        /// <param name="apostaRodada">A entidade ApostaRodada a ser atualizada.</param>
        public async Task Atualizar(ApostaRodada apostaRodada)
        {
            await _apostaRodadaRepository.Atualizar(apostaRodada);
            await Commit();
        }

        /// <summary>
        /// Marca uma ApostaRodada como submetida, registrando a data e hora.
        /// </summary>
        /// <param name="apostaRodada">A entidade ApostaRodada a ser marcada.</param>
        public async Task MarcarApostaRodadaComoSubmetida(ApostaRodada apostaRodada)
        {
            apostaRodada.DataHoraSubmissao = DateTime.Now;
            apostaRodada.Enviada = true;
            await _apostaRodadaRepository.Atualizar(apostaRodada);
            await Commit();
        }

        /// <summary>
        /// Obtém o status de envio e a data/hora da aposta de uma rodada para um usuário específico.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <param name="apostadorCampeonatoId">O ID do ApostadorCampeonato.</param>
        /// <returns>Um DTO com o status da aposta, ou null se não houver aposta para a rodada/apostador.</returns>
        public async Task<ApostaRodadaStatusDto> ObterStatusApostaRodadaParaUsuario(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            // O erro estava aqui: _apostaRodadaRepository.Buscar retorna IQueryable<ApostaRodada>
            // que é onde FirstOrDefaultAsync() deve ser chamado.
            // O await deve ser aplicado ao resultado do Buscar() antes de FirstOrDefaultAsync.
            var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == rodadaId && ar.ApostadorCampeonatoId == apostadorCampeonatoId)
                                                            .FirstOrDefaultAsync();

            if (apostaRodada == null)
            {
                return null;
            }

            return _mapper.Map<ApostaRodadaStatusDto>(apostaRodada);
        }

        /// <summary>
        /// Obtém as apostas de um apostador em uma rodada, formatadas para edição.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <param name="apostadorCampeamentoId">O ID do ApostadorCampeonato.</param>
        /// <returns>Uma lista de DTOs com os dados das apostas para edição.</returns>
        public async Task<IEnumerable<ApostaJogoDto>> ObterApostasDoApostadorNaRodadaParaEdicao(Guid rodadaId, Guid apostadorCampeamentoId)
        {
            var rodada = await _rodadaRepository.ObterRodadaComJogosEEquipes(rodadaId);
            if (rodada == null || rodada.Status != StatusRodada.EmApostas)
            {
                Notificar("Alerta", "Rodada não encontrada ou não está mais disponível para apostas.");
                return new List<ApostaJogoDto>();
            }

            var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == rodadaId && ar.ApostadorCampeonatoId == apostadorCampeamentoId)
                                                            .Include(ar => ar.Palpites)
                                                            .FirstOrDefaultAsync();

            var apostasParaEdicao = new List<ApostaJogoDto>();

            foreach (var jogo in rodada.JogosRodada.OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo))
            {
                var palpite = apostaRodada?.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);

                apostasParaEdicao.Add(new ApostaJogoDto
                {
                    Id = palpite?.Id ?? Guid.Empty,
                    IdJogo = jogo.Id,
                    EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                    SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                    EscudoMandante = jogo.EquipeCasa.Equipe.Escudo,
                    PlacarMandante = (palpite?.PlacarApostaCasa ?? 0) == 0 
                                     && (apostaRodada?.Enviada == false || apostaRodada == null) 
                                     ? "" : (palpite?.PlacarApostaCasa ?? 0).ToString(),
                    EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                    SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                    EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,
                    PlacarVisitante = (palpite?.PlacarApostaVisita ?? 0) == 0 
                                      && (apostaRodada?.Enviada == false || apostaRodada == null) 
                                      ? "" : (palpite?.PlacarApostaVisita ?? 0).ToString(),
                    DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
                    HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm")
                });
            }

            return apostasParaEdicao;
        }

        /// <summary>
        /// Obtém as apostas de um apostador em uma rodada, formatadas para visualização (com placares reais e pontuação).
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <param name="apostadorCampeamentoId">O ID do ApostadorCampeonato.</param>
        /// <returns>Uma lista de DTOs com os dados das apostas para visualização.</returns>
        public async Task<IEnumerable<ApostaJogoVisualizacaoDto>> ObterApostasDoApostadorNaRodadaParaVisualizacao(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == rodadaId 
                                                             && ar.ApostadorCampeonatoId == apostadorCampeonatoId)
                                                            .Include(ar => ar.Palpites)
                                                            .FirstOrDefaultAsync();

            var jogosDaRodada = await _rodadaRepository.ObterJogosDaRodadaComPlacaresEEquipes(rodadaId);
            if (!jogosDaRodada.Any())
            {
                Notificar("Alerta", "Não há jogos definidos para esta rodada.");
                return new List<ApostaJogoVisualizacaoDto>();
            }

            var apostasParaVisualizacao = new List<ApostaJogoVisualizacaoDto>();

            foreach (var jogo in jogosDaRodada.OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo))
            {
                var palpite = apostaRodada?.Palpites.FirstOrDefault(p => p.JogoId == jogo.Id);

                apostasParaVisualizacao.Add(new ApostaJogoVisualizacaoDto
                {
                    Id = palpite?.Id ?? Guid.Empty,
                    IdJogo = jogo.Id,
                    EquipeMandante = jogo.EquipeCasa.Equipe.Nome,
                    SiglaMandante = jogo.EquipeCasa.Equipe.Sigla,
                    EscudoMandante = jogo.EquipeCasa.Equipe.Escudo,
                    PlacarRealCasa = jogo.PlacarCasa,
                    PlacarApostaCasa = palpite?.PlacarApostaCasa ?? 0,
                    EquipeVisitante = jogo.EquipeVisitante.Equipe.Nome,
                    SiglaVisitante = jogo.EquipeVisitante.Equipe.Sigla,
                    EscudoVisitante = jogo.EquipeVisitante.Equipe.Escudo,
                    PlacarRealVisita = jogo.PlacarVisita,
                    PlacarApostaVisita = palpite?.PlacarApostaVisita ?? 0,
                    DataJogo = jogo.DataJogo.ToString("yyyy-MM-dd"),
                    HoraJogo = jogo.HoraJogo.ToString(@"hh\:mm"),
                    StatusJogo = jogo.Status.ToString(),
                    Enviada = apostaRodada?.Enviada ?? false,
                    Pontuacao = palpite?.Pontos ?? 0
                });
            }

            return apostasParaVisualizacao;
        }

        /// <summary>
        /// Salva ou atualiza as apostas de uma rodada para um apostador.
        /// </summary>
        /// <param name="salvarApostaDto">O DTO contendo os dados da aposta a ser salva.</param>
        /// <returns>True se as apostas foram salvas com sucesso, false caso contrário.</returns>
        public async Task<bool> SalvarApostas(SalvarApostaRequestDto salvarApostaDto)
        {
            // 1. Validar a rodada e o status
            var rodada = await _rodadaRepository.ObterPorId(salvarApostaDto.RodadaId);
            if (rodada == null)
            {
                Notificar("Erro", "Rodada não encontrada.");
                return false;
            }
            if (rodada.Status != StatusRodada.EmApostas)
            {
                Notificar("Alerta", "Não é possível salvar apostas para esta rodada. Ela não está mais 'Em Apostas'.");
                return false;
            }

            // 2. Obter ou criar ApostaRodada
            // Corrigido: await no Buscar() antes de FirstOrDefaultAsync()
            var apostaRodada = await _apostaRodadaRepository.Buscar(ar => ar.RodadaId == salvarApostaDto.RodadaId && ar.ApostadorCampeonatoId == salvarApostaDto.ApostadorCampeonatoId)
                                                            .FirstOrDefaultAsync();

            if (apostaRodada == null)
            {
                apostaRodada = new ApostaRodada
                {
                    RodadaId = salvarApostaDto.RodadaId,
                    ApostadorCampeonatoId = salvarApostaDto.ApostadorCampeonatoId,
                    EhApostaCampeonato = salvarApostaDto.EhApostaCampeonato,
                    EhApostaIsolada = salvarApostaDto.EhApostaIsolada,
                    // Outras propriedades padrão para uma nova ApostaRodada
                };
                await _apostaRodadaRepository.Adicionar(apostaRodada); // Usando Adicionar do repositório
            }
            else
            {
                // Se a apostaRodada já existe, atualize-a se necessário (ex: data de submissão)
                // Não é estritamente necessário atualizar aqui se apenas os palpites mudam,
                // mas é uma boa prática para manter o objeto rastreado pelo EF.
                await _apostaRodadaRepository.Atualizar(apostaRodada); // Usando Atualizar do repositório
            }

            // 3. Processar cada palpite
            foreach (var palpiteDto in salvarApostaDto.Palpites)
            {
                var jogo = await _jogoRepository.ObterPorId(palpiteDto.IdJogo);
                if (jogo == null)
                {
                    Notificar("Erro", $"Jogo com ID {palpiteDto.IdJogo} não encontrado.");
                    return false;
                }

                // Verificar se o jogo já começou (horário do jogo + 15 minutos de tolerância)
                DateTime jogoDateTime = jogo.DataJogo.Date + jogo.HoraJogo;
                if (jogoDateTime.AddMinutes(15) < DateTime.Now)
                {
                    Notificar("Alerta", $"Não é possível apostar no jogo entre {jogo.EquipeCasa.Equipe.Nome} e {jogo.EquipeVisitante.Equipe.Nome}, pois ele já começou ou está prestes a começar.");
                    continue; // Pula para o próximo palpite
                }

                var palpiteExistente = await _palpiteRepository.Buscar(p => p.ApostaRodadaId == apostaRodada.Id && p.JogoId == palpiteDto.IdJogo)
                                                               .FirstOrDefaultAsync();

                if (palpiteExistente == null)
                {
                    // Criar novo palpite
                    var novoPalpite = new Palpite
                    {
                        ApostaRodadaId = apostaRodada.Id,
                        JogoId = palpiteDto.IdJogo,
                        PlacarApostaCasa = palpiteDto.PlacarApostaCasa,
                        PlacarApostaVisita = palpiteDto.PlacarApostaVisita
                    };
                    await _palpiteRepository.Adicionar(novoPalpite);
                }
                else
                {
                    // Atualizar palpite existente
                    palpiteExistente.PlacarApostaCasa = palpiteDto.PlacarApostaCasa;
                    palpiteExistente.PlacarApostaVisita = palpiteDto.PlacarApostaVisita;
                    await _palpiteRepository.Atualizar(palpiteExistente);
                }
            }

            // 4. Marcar a ApostaRodada como enviada e registrar a data/hora
            apostaRodada.DataHoraSubmissao = DateTime.Now;
            apostaRodada.Enviada = true;
            await _apostaRodadaRepository.Atualizar(apostaRodada);

            // 5. Salvar as alterações na UnitOfWork
            return await Commit();
        }
    }
}
