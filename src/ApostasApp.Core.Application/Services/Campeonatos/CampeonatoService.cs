using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces; // Para BaseService
using ApostasApp.Core.Application.Services.Interfaces.Campeonatos; // Para ICampeonatoService
using ApostasApp.Core.Application.Services.Interfaces.Financeiro; // Para IFinanceiroService
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Apostadores; // Para IApostadorRepository
using ApostasApp.Core.Domain.Interfaces.Campeonatos; // Para ICampeonatoRepository, IApostadorCampeonatoRepository
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Campeonatos; // Para Campeonato, ApostadorCampeonato
using ApostasApp.Core.Domain.Models.Financeiro; // Para TipoTransacao
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Application.Campeonatos.Services
{
    /// <summary>
    /// CampeonatoService é responsável por operações de consulta e adesão a campeonatos no ApostasApp.
    /// Ele herda de BaseService, que gerencia o IUnitOfWork e o INotificador.
    /// </summary>
    public class CampeonatoService : BaseService, ICampeonatoService
    {
        private readonly ICampeonatoRepository _campeonatoRepository;
        private readonly IApostadorRepository _apostadorRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly IFinanceiroService _financeiroService;

        public CampeonatoService(
            ICampeonatoRepository campeonatoRepository,
            IApostadorRepository apostadorRepository,
            IApostadorCampeonatoRepository apostadorCampeonatoRepository,
            IFinanceiroService financeiroService,
            IUnitOfWork uow,
            INotificador notificador) : base(notificador, uow)
        {
            _campeonatoRepository = campeonatoRepository;
            _apostadorRepository = apostadorRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _financeiroService = financeiroService;
        }

        /// <summary>
        /// Obtém uma lista de campeonatos disponíveis para exibição.
        /// </summary>
        /// <returns>Uma coleção de DTOs de Campeonatos.</returns>
        public async Task<IEnumerable<CampeonatoListItemDto>> GetAvailableCampeonatosAsync()
        {
            var campeonatos = await _campeonatoRepository.ObterListaDeCampeonatosAtivos();

            return campeonatos.Select(c => new CampeonatoListItemDto
            {
                Id = c.Id,
                Nome = c.Nome,
                DataInic = c.DataInic,
                DataFim = c.DataFim,
                NumRodadas = c.NumRodadas,
                CustoAdesao = c.CustoAdesao,
                Ativo = c.Ativo,
                Tipo = c.Tipo
            }).ToList();
        }

        /// <summary>
        /// Permite que um apostador adira a um campeonato.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <param name="campeonatoId">O ID do campeonato.</param>
        /// <returns>True se a adesão foi bem-sucedida, false caso contrário.</returns>
        public async Task<bool> AdherirCampeonatoAsync(Guid apostadorId, Guid campeonatoId)
        {
            if (apostadorId == Guid.Empty || campeonatoId == Guid.Empty)
            {
                Notificar("Erro", "IDs de apostador ou campeonato inválidos.");
                return false;
            }

            var campeonato = await _campeonatoRepository.ObterPorId(campeonatoId);
            var apostador = await _apostadorRepository.ObterPorId(apostadorId);

            if (campeonato == null)
            {
                Notificar("Alerta", "Campeonato não encontrado.");
                return false;
            }

            if (apostador == null)
            {
                Notificar("Alerta", "Apostador não encontrado.");
                return false;
            }

            if (!campeonato.Ativo)
            {
                Notificar("Alerta", "Campeonato não está ativo para adesão.");
                return false;
            }

            // Verificar se o apostador já aderiu
            var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorDoCampeonato(campeonatoId, apostadorId);

            if (apostadorCampeonato != null)
            {
                Notificar("Alerta", "Apostador já aderiu a este campeonato.");
                return false;
            }

            // Lógica de débito de saldo se houver custo de adesão
            bool debitoSucesso = true;

            if (campeonato.CustoAdesao.HasValue && campeonato.CustoAdesao.Value > 0)
            {
                debitoSucesso = await _financeiroService.DebitarSaldoAsync(
                    apostadorId,
                    campeonato.CustoAdesao.Value,
                    TipoTransacao.AdesaoCampeonato,
                    $"Adesão ao campeonato: {campeonato.Nome}");

                if (!debitoSucesso)
                {
                    return false;
                }
            }

            // Criar e vincular a entidade ApostadorCampeonato
            var novaAdesao = new ApostadorCampeonato(apostadorId, campeonatoId);
            await _apostadorCampeonatoRepository.Adicionar(novaAdesao);

            var saved = await Commit();

            if (saved)
            {
                Notificar("Sucesso", $"Adesão ao campeonato '{campeonato.Nome}' realizada com sucesso!");
                return true;
            }
            else
            {
                Notificar("Erro", "Não foi possível registrar a adesão ao campeonato.");
                return false;
            }
        }

        /// <summary>
        /// Obtém um campeonato pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do campeonato.</param>
        /// <returns>O campeonato encontrado, ou null se não existir.</returns>
        public async Task<Campeonato> ObterPorId(Guid id)
        {
            return await _campeonatoRepository.ObterPorId(id);
        }

        /// <summary>
        /// Obtém todos os campeonatos (método de consulta geral, se necessário para outras partes do app).
        /// </summary>
        /// <returns>Uma coleção de campeonatos.</returns>
        public async Task<IEnumerable<Campeonato>> ObterTodos()
        {
            return await _campeonatoRepository.ObterTodos();
        }
    }
}
