using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces; // Para BaseService
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Application.Validations;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Campeonatos; // Para ICampeonatoRepository, IApostadorCampeonatoRepository
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador (Namespace correto para a interface)
using ApostasApp.Core.Domain.Interfaces.RankingRodadas;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Para IRodadaRepository
using ApostasApp.Core.Domain.Models.RankingRodadas;
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada, StatusRodada


namespace ApostasApp.Core.Application.Services.Rodadas
{
    /// <summary>
    /// RodadaService é responsável por fornecer dados de rodadas para a camada de aplicação.
    /// Ele delega as operações de persistência e busca para o IRodadaRepository.
    /// Ele herda de BaseService, que gerencia o IUnitOfWork e o INotificador.
    /// </summary>
    public class RodadaService : BaseService, IRodadaService
    {
        private readonly IRodadaRepository _rodadaRepository;
        private readonly ICampeonatoRepository _campeonatoRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository; // Adicionado

        // O construtor agora injeta INotificador, IUnitOfWork e IApostadorCampeonatoRepository
        public RodadaService(IRodadaRepository rodadaRepository,
                             ICampeonatoRepository campeonatoRepository,
                             IApostadorCampeonatoRepository apostadorCampeonatoRepository, // Adicionado
                             INotificador notificador,
                             IUnitOfWork uow) : base(notificador, uow) // Passando notificador e uow para o construtor da BaseService
        {
            _rodadaRepository = rodadaRepository;
            _campeonatoRepository = campeonatoRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository; // Atribuição
        }

        /// <summary>
        /// Obtém uma rodada específica pelo seu ID.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada.</param>
        /// <returns>A rodada encontrada, ou null se não existir.</returns>
        public async Task<Rodada> ObterRodadaPorId(Guid rodadaId)
        {
            return await _rodadaRepository.ObterPorId(rodadaId);
        }

        /// <summary>
        /// Obtém a rodada que está atualmente no status 'Em Apostas' para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>A rodada em apostas, ou null se não houver.</returns>
        public async Task<Rodada> ObterRodadaEmApostasPorCampeonato(Guid campeonatoId)
        {
            // Busca a rodada com status "Em Apostas" que pertence ao campeonato especificado.
            // O repositório já deve incluir o Campeonato.
            var rodada = await _rodadaRepository.ObterRodadaEmApostasPorCampeonato(campeonatoId);

            if (rodada == null)
            {
                Notificar("Alerta", "Não há rodada 'Em Apostas' para o campeonato informado."); // Notificar com dois parâmetros
                return null;
            }

            return rodada;
        }

        /// <summary>
        /// Obtém a rodada que está atualmente no status 'Corrente' para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>A rodada corrente, ou null se não houver.</returns>
        public async Task<Rodada> ObterRodadaCorrentePorCampeonato(Guid campeonatoId)
        {
            // Busca a rodada com status "Corrente" que pertence ao campeonato especificado.
            // O repositório já deve incluir o Campeonato.
            var rodada = await _rodadaRepository.ObterRodadaCorrentePorCampeonato(campeonatoId);

            if (rodada == null)
            {
                Notificar("Alerta", "Não há rodada 'Corrente' para o campeonato informado."); // Notificar com dois parâmetros
                return null;
            }

            return rodada;
        }

        // =========================================================================================================
        // Implementações dos métodos 'ParaApostador' que o Controller ainda espera
        // =========================================================================================================

        /// <summary>
        /// Obtém uma rodada específica que está 'Em Apostas' e pertence ao campeonato do apostador.
        /// Este método valida o ID da rodada fornecido.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada a ser verificada.</param>
        /// <param name="apostadorCampeamentoId">O ID do ApostadorCampeonato para verificar o vínculo.</param>
        /// <returns>A rodada se for válida e estiver 'Em Apostas' para o campeonato, caso contrário null.</returns>
        public async Task<Rodada> ObterRodadaEmApostasParaApostador(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var rodada = await _rodadaRepository.ObterPorId(rodadaId);
            if (rodada == null)
            {
                Notificar("Alerta", "Rodada não encontrada.");
                return null;
            }

            // Usando o método correto do IApostadorCampeonatoRepository para obter pelo ID da entidade
            var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorCampeonato(apostadorCampeonatoId);
            if (apostadorCampeonato == null)
            {
                Notificar("Alerta", "Apostador Campeonato não encontrado.");
                return null;
            }

            if (rodada.Status == StatusRodada.EmApostas && rodada.CampeonatoId == apostadorCampeonato.CampeonatoId)
            {
                return rodada;
            }

            Notificar("Alerta", "A rodada não está disponível para apostas ou não pertence ao seu campeonato.");
            return null;
        }

        /// <summary>
        /// Obtém uma rodada específica que está 'Corrente' e pertence ao campeonato do apostador.
        /// Este método valida o ID da rodada fornecido.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada a ser verificada.</param>
        /// <param name="apostadorCampeamentoId">O ID do ApostadorCampeonato para verificar o vínculo.</param>
        /// <returns>A rodada se for válida e estiver 'Corrente' para o campeonato, caso contrário null.</returns>
        public async Task<Rodada> ObterRodadaCorrenteParaApostador(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var rodada = await _rodadaRepository.ObterPorId(rodadaId);
            if (rodada == null)
            {
                Notificar("Alerta", "Rodada não encontrada.");
                return null;
            }

            // Usando o método correto do IApostadorCampeonatoRepository para obter pelo ID da entidade
            var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorCampeonato(apostadorCampeonatoId);
            if (apostadorCampeonato == null)
            {
                Notificar("Alerta", "Apostador Campeonato não encontrado.");
                return null;
            }

            if (rodada.Status == StatusRodada.Corrente && rodada.CampeonatoId == apostadorCampeonato.CampeonatoId)
            {
                return rodada;
            }

            Notificar("Alerta", "A rodada não está corrente ou não pertence ao seu campeonato.");
            return null;
        }

        // =========================================================================================================
        // Métodos de listagem de rodadas (delegando para o repositório)
        // =========================================================================================================

        /// <summary>
        /// Obtém todas as rodadas finalizadas para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma coleção de rodadas finalizadas.</returns>
        public async Task<IEnumerable<Rodada>> ObterRodadasFinalizadasPorCampeonato(Guid campeonatoId)
        {
            return await _rodadaRepository.ObterRodadasFinalizadasPorCampeonato(campeonatoId);
        }

        /// <summary>
        /// Obtém rodadas com ranking (corrente ou finalizada) para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma coleção de rodadas com ranking.</returns>
        public async Task<IEnumerable<Rodada>> ObterRodadasComRankingPorCampeonato(Guid campeonatoId)
        {
            return await _rodadaRepository.ObterRodadasComRankingPorCampeonato(campeonatoId);
        }

        /// <summary>
        /// Obtém todas as rodadas para um campeonato específico.
        /// </summary>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>Uma coleção de todas as rodadas do campeonato.</returns>
        public async Task<IEnumerable<Rodada>> ObterTodasAsRodadasDoCampeonato(Guid campeonatoId)
        {
            return await _rodadaRepository.ObterTodasAsRodadasDoCampeonato(campeonatoId);
        }

        public async Task Atualizar(Rodada Rodada)
        {
            // Assumindo que ExecutarValidacao está na BaseService
            if (!ExecutarValidacao(new RodadaValidation(), Rodada)) return;
                        
            await _rodadaRepository.Atualizar(Rodada);
            await Commit(); // Chamar Commit() da BaseService para persistir as alterações
            Notificar("Sucesso", "Status da Rodada atualizado com sucesso!");
        }
               
    }
}
