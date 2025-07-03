// Localização: ApostasApp.Core.Application.Services.Rodadas/RodadaService.cs

using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas; // Para IRodadaService
using ApostasApp.Core.Application.Validations; // Para RodadaValidation (se ainda for usada)
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Campeonatos; // Para ICampeonatoRepository, IApostadorCampeonatoRepository
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Para IRodadaRepository
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada
//using ApostasApp.Core.Domain.Models.Enums; // Para StatusRodada (se estiver aqui)
using System; // Para Guid
using System.Collections.Generic; // Para IEnumerable, List
using System.Linq; // Para .Any(), .OrderBy(), .Take(), .ToList(), Enumerable.Empty<T>()
using System.Threading.Tasks; // Para Task
using Microsoft.EntityFrameworkCore; // Para .Include(), .AsNoTracking(), .ToListAsync()

namespace ApostasApp.Core.Application.Services.Rodadas
{
    public class RodadaService : BaseService, IRodadaService
    {
        private readonly IRodadaRepository _rodadaRepository;
        private readonly ICampeonatoRepository _campeonatoRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;

        public RodadaService(IRodadaRepository rodadaRepository,
                             ICampeonatoRepository campeonatoRepository,
                             IApostadorCampeonatoRepository apostadorCampeonatoRepository,
                             INotificador notificador,
                             IUnitOfWork uow) : base(notificador, uow)
        {
            _rodadaRepository = rodadaRepository;
            _campeonatoRepository = campeonatoRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
        }

        public async Task<Rodada> ObterRodadaPorId(Guid rodadaId)
        {
            var rodada = await _rodadaRepository.ObterPorId(rodadaId);
            if (rodada == null)
            {
                Notificar("RODADA_NAO_ENCONTRADA", "Alerta", "Rodada não encontrada.");
            }
            return rodada;
        }

        public async Task<Rodada> ObterRodadaEmApostasPorCampeonato(Guid campeonatoId)
        {
            var rodada = await _rodadaRepository.ObterRodadaEmApostasPorCampeonato(campeonatoId);
            if (rodada == null)
            {
                Notificar("RODADA_EM_APOSTAS_UNICA_NAO_ENCONTRADA", "Alerta", "Não há rodada 'Em Apostas' única para o campeonato informado.");
            }
            return rodada;
        }

        public async Task<IEnumerable<Rodada>> ObterRodadasEmApostaPorCampeonato(Guid campeonatoId)
        {
            var listRodada = await _rodadaRepository.ObterRodadasEmApostaPorCampeonato(campeonatoId);
            if (listRodada == null || !listRodada.Any())
            {
                Notificar("RODADAS_EM_APOSTAS_NAO_ENCONTRADAS", "Alerta", "Não há rodadas 'Em Apostas' para o campeonato informado.");
                return Enumerable.Empty<Rodada>();
            }
            return listRodada;
        }

        public async Task<IEnumerable<Rodada>> ObterRodadasCorrentePorCampeonato(Guid campeonatoId)
        {
            var listRodada = await _rodadaRepository.ObterRodadasCorrentePorCampeonato(campeonatoId);
            if (listRodada == null || !listRodada.Any())
            {
                Notificar("RODADAS_CORRENTE_NAO_ENCONTRADAS", "Alerta", "Não há rodadas 'Corrente' para o campeonato informado.");
                return Enumerable.Empty<Rodada>();
            }
            return listRodada;
        }

        public async Task<Rodada> ObterRodadaCorrentePorCampeonato(Guid campeonatoId)
        {
            var rodada = await _rodadaRepository.ObterRodadaCorrentePorCampeonato(campeonatoId);
            if (rodada == null)
            {
                Notificar("RODADA_CORRENTE_UNICA_NAO_ENCONTRADA", "Alerta", "Não há rodada 'Corrente' única para o campeonato informado.");
            }
            return rodada;
        }

        public async Task<Rodada> ObterRodadaEmApostasParaApostador(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var rodada = await _rodadaRepository.ObterPorId(rodadaId);
            if (rodada == null)
            {
                Notificar("RODADA_NAO_ENCONTRADA_APOSTADOR", "Alerta", "Rodada não encontrada.");
                return null;
            }

            var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorCampeonato(apostadorCampeonatoId);
            if (apostadorCampeonato == null)
            {
                Notificar("APOSTADOR_CAMPEONATO_NAO_ENCONTRADO", "Alerta", "Apostador Campeonato não encontrado.");
                return null;
            }

            if (rodada.Status == StatusRodada.EmApostas && rodada.CampeonatoId == apostadorCampeonato.CampeonatoId)
            {
                return rodada;
            }

            Notificar("RODADA_NAO_DISPONIVEL_APOSTAS", "Alerta", "A rodada não está disponível para apostas ou não pertence ao seu campeonato.");
            return null;
        }

        public async Task<Rodada> ObterRodadaCorrenteParaApostador(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var rodada = await _rodadaRepository.ObterPorId(rodadaId);
            if (rodada == null)
            {
                Notificar("RODADA_NAO_ENCONTRADA_CORRENTE_APOSTADOR", "Alerta", "Rodada não encontrada.");
                return null;
            }

            var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorCampeonato(apostadorCampeonatoId);
            if (apostadorCampeonato == null)
            {
                Notificar("APOSTADOR_CAMPEONATO_NAO_ENCONTRADO_CORRENTE", "Alerta", "Apostador Campeonato não encontrado.");
                return null;
            }

            if (rodada.Status == StatusRodada.Corrente && rodada.CampeonatoId == apostadorCampeonato.CampeonatoId)
            {
                return rodada;
            }

            Notificar("RODADA_NAO_CORRENTE_APOSTADOR", "Alerta", "A rodada não está corrente ou não pertence ao seu campeonato.");
            return null;
        }

        public async Task<IEnumerable<Rodada>> ObterRodadasFinalizadasPorCampeonato(Guid campeonatoId)
        {
            var rodadas = await _rodadaRepository.ObterRodadasFinalizadasPorCampeonato(campeonatoId);
            if (rodadas == null || !rodadas.Any())
            {
                Notificar("RODADAS_FINALIZADAS_NAO_ENCONTRADAS", "Alerta", "Nenhuma rodada finalizada encontrada para este campeonato.");
                return Enumerable.Empty<Rodada>();
            }
            return rodadas;
        }

        public async Task<IEnumerable<Rodada>> ObterRodadasComRankingPorCampeonato(Guid campeonatoId)
        {
            var rodadas = await _rodadaRepository.ObterRodadasComRankingPorCampeonato(campeonatoId);
            if (rodadas == null || !rodadas.Any())
            {
                Notificar("RODADAS_RANKING_NAO_ENCONTRADAS", "Alerta", "Nenhuma rodada com ranking encontrada para este campeonato.");
                return Enumerable.Empty<Rodada>();
            }
            return rodadas;
        }

        public async Task<IEnumerable<Rodada>> ObterTodasAsRodadasDoCampeonato(Guid campeonatoId)
        {
            var rodadas = await _rodadaRepository.ObterTodasAsRodadasDoCampeonato(campeonatoId);
            if (rodadas == null || !rodadas.Any())
            {
                Notificar("TODAS_RODADAS_NAO_ENCONTRADAS", "Alerta", "Nenhuma rodada encontrada para este campeonato.");
                return Enumerable.Empty<Rodada>();
            }
            return rodadas;
        }

        public async Task Atualizar(Rodada Rodada)
        {
            // <<-- REMOVIDO ExecutarValidacao. Adicione sua lógica de validação aqui se necessário. -->>
            if (Rodada == null)
            {
                Notificar("RODADA_NULA_ATUALIZAR", "Erro", "A rodada fornecida para atualização é nula.");
                return;
            }

            await _rodadaRepository.Atualizar(Rodada);
            await CommitAsync(); // <<-- CORRIGIDO: Chamando CommitAsync()
            Notificar("ATUALIZACAO_SUCESSO", "Sucesso", "Status da Rodada atualizado com sucesso!");
        }

        public async Task<IEnumerable<Rodada>> ObterRodadasEmDestaque()
        {
            var rodadas = await _rodadaRepository.Buscar(r => r.Status == StatusRodada.Corrente || r.Status == StatusRodada.EmApostas)
                                                 .Include(r => r.Campeonato)
                                                 .OrderBy(r => r.DataInic)
                                                 .Take(5)
                                                 .ToListAsync();

            if (!rodadas.Any())
            {
                Notificar("RODADAS_DESTAQUE_NAO_ENCONTRADAS", "Alerta", "Nenhuma rodada em destaque encontrada na base de dados.");
            }

            return rodadas;
        }
    }
}
