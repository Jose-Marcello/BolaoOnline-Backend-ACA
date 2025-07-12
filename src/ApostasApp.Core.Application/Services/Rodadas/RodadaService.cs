// Localização: ApostasApp.Core.Application.Services.Rodadas/RodadaService.cs

using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Campeonatos;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Interfaces.Rodadas; // Para IRodadaRepository
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada, StatusRodada
using Microsoft.EntityFrameworkCore; // Para .Include(), .AsNoTracking(), .ToListAsync()
using Microsoft.Extensions.Logging; // Para ILogger
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Rodadas
{
    public class RodadaService : BaseService, IRodadaService
    {
        private readonly IRodadaRepository _rodadaRepository;
        private readonly ICampeonatoRepository _campeonatoRepository;
        private readonly IApostadorCampeonatoRepository _apostadorCampeonatoRepository;
        private readonly ILogger<RodadaService> _logger;

        public RodadaService(IRodadaRepository rodadaRepository,
                                     ICampeonatoRepository campeonatoRepository,
                                     IApostadorCampeonatoRepository apostadorCampeonatoRepository,
                                     INotificador notificador,
                                     IUnitOfWork uow,
                                     ILogger<RodadaService> logger)
            : base(notificador, uow)
        {
            _rodadaRepository = rodadaRepository;
            _campeonatoRepository = campeonatoRepository;
            _apostadorCampeonatoRepository = apostadorCampeonatoRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<Rodada>> ObterRodadaPorId(Guid rodadaId)
        {
            var apiResponse = new ApiResponse<Rodada>(); // Instanciação direta
            try
            {
                var rodada = await _rodadaRepository.ObterPorId(rodadaId);
                if (rodada == null)
                {
                    Notificar("Alerta", "Rodada não encontrada."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false; // Se a rodada não é encontrada, é um erro
                    apiResponse.Data = null;
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = rodada;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodada por ID.");
                Notificar("Erro", $"Erro interno ao obter rodada por ID: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = null;
                return apiResponse;
            }
        }

        public async Task<ApiResponse<Rodada>> ObterRodadaEmApostasPorCampeonato(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<Rodada>(); // Instanciação direta
            try
            {
                var rodada = await _rodadaRepository.ObterRodadaEmApostasPorCampeonato(campeonatoId);
                if (rodada == null)
                {
                    Notificar("Alerta", "Não há rodada 'Em Apostas' única para o campeonato informado."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false; // Se não encontrou, é um erro
                    apiResponse.Data = null;
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = rodada;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodada 'Em Apostas' por campeonato.");
                Notificar("Erro", $"Erro interno ao obter rodada 'Em Apostas': {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = null;
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasEmApostaPorCampeonato(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>(); // Instanciação direta
            try
            {
                var listRodada = await _rodadaRepository.ObterRodadasEmApostaPorCampeonato(campeonatoId);
                if (listRodada == null || !listRodada.Any())
                {
                    Notificar("Alerta", "Não há rodadas 'Em Apostas' para o campeonato informado."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Considera sucesso, mas com lista vazia
                    apiResponse.Data = Enumerable.Empty<Rodada>();
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = listRodada;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter lista de rodadas 'Em Apostas' por campeonato.");
                Notificar("Erro", $"Erro interno ao obter lista de rodadas 'Em Apostas': {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasCorrentePorCampeonato(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>(); // Instanciação direta
            try
            {
                var listRodada = await _rodadaRepository.ObterRodadasCorrentePorCampeonato(campeonatoId);
                if (listRodada == null || !listRodada.Any())
                {
                    Notificar("Alerta", "Não há rodadas 'Corrente' para o campeonato informado."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Considera sucesso, mas com lista vazia
                    apiResponse.Data = Enumerable.Empty<Rodada>();
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = listRodada;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter lista de rodadas 'Corrente' por campeonato.");
                Notificar("Erro", $"Erro interno ao obter lista de rodadas 'Corrente': {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<Rodada>> ObterRodadaCorrentePorCampeonato(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<Rodada>(); // Instanciação direta
            try
            {
                var rodada = await _rodadaRepository.ObterRodadaCorrentePorCampeonato(campeonatoId);
                if (rodada == null)
                {
                    Notificar("Alerta", "Não há rodada 'Corrente' única para o campeonato informado."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false; // Se não encontrou, é um erro
                    apiResponse.Data = null;
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = rodada;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodada 'Corrente' por campeonato.");
                Notificar("Erro", $"Erro interno ao obter rodada 'Corrente': {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = null;
                return apiResponse;
            }
        }

        public async Task<ApiResponse<Rodada>> ObterRodadaEmApostasParaApostador(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var apiResponse = new ApiResponse<Rodada>(); // Instanciação direta
            try
            {
                var rodada = await _rodadaRepository.ObterPorId(rodadaId);
                if (rodada == null)
                {
                    Notificar("Alerta", "Rodada não encontrada."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                    return apiResponse;
                }

                var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorCampeonato(apostadorCampeonatoId);
                if (apostadorCampeonato == null)
                {
                    Notificar("Alerta", "Apostador Campeonato não encontrado."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                    return apiResponse;
                }

                if (rodada.Status == StatusRodada.EmApostas && rodada.CampeonatoId == apostadorCampeonato.CampeonatoId)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = rodada;
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                Notificar("Alerta", "A rodada não está disponível para apostas ou não pertence ao seu campeonato."); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = null;
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodada 'Em Apostas' para apostador.");
                Notificar("Erro", $"Erro interno ao obter rodada 'Em Apostas' para apostador: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = null;
                return apiResponse;
            }
        }

        public async Task<ApiResponse<Rodada>> ObterRodadaCorrenteParaApostador(Guid rodadaId, Guid apostadorCampeonatoId)
        {
            var apiResponse = new ApiResponse<Rodada>(); // Instanciação direta
            try
            {
                var rodada = await _rodadaRepository.ObterPorId(rodadaId);
                if (rodada == null)
                {
                    Notificar("Alerta", "Rodada não encontrada."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                    return apiResponse;
                }

                var apostadorCampeonato = await _apostadorCampeonatoRepository.ObterApostadorCampeonato(apostadorCampeonatoId);
                if (apostadorCampeonato == null)
                {
                    Notificar("Alerta", "Apostador Campeonato não encontrado."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false;
                    apiResponse.Data = null;
                    return apiResponse;
                }

                if (rodada.Status == StatusRodada.Corrente && rodada.CampeonatoId == apostadorCampeonato.CampeonatoId)
                {
                    apiResponse.Success = true;
                    apiResponse.Data = rodada;
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                Notificar("Alerta", "A rodada não está corrente ou não pertence ao seu campeonato."); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = null;
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodada 'Corrente' para apostador.");
                Notificar("Erro", $"Erro interno ao obter rodada 'Corrente' para apostador: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = null;
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasFinalizadasPorCampeonato(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>(); // Instanciação direta
            try
            {
                var rodadas = await _rodadaRepository.ObterRodadasFinalizadasPorCampeonato(campeonatoId);
                if (rodadas == null || !rodadas.Any())
                {
                    Notificar("Alerta", "Nenhuma rodada finalizada encontrada para este campeonato."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Considera sucesso, mas com lista vazia
                    apiResponse.Data = Enumerable.Empty<Rodada>();
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = rodadas;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodadas finalizadas por campeonato.");
                Notificar("Erro", $"Erro interno ao obter rodadas finalizadas: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasComRankingPorCampeonato(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>(); // Instanciação direta
            try
            {
                var rodadas = await _rodadaRepository.ObterRodadasComRankingPorCampeonato(campeonatoId);
                if (rodadas == null || !rodadas.Any())
                {
                    Notificar("Alerta", "Nenhuma rodada com ranking encontrada para este campeonato."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Considera sucesso, mas com lista vazia
                    apiResponse.Data = Enumerable.Empty<Rodada>();
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = rodadas;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodadas com ranking por campeonato.");
                Notificar("Erro", $"Erro interno ao obter rodadas com ranking: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>();
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> ObterTodasAsRodadasDoCampeonato(Guid campeonatoId)
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>(); // Instanciação direta
            try
            {
                var rodadas = await _rodadaRepository.ObterTodasAsRodadasDoCampeonato(campeonatoId);
                if (rodadas == null || !rodadas.Any())
                {
                    Notificar("Alerta", "Nenhuma rodada encontrada para este campeonato."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Considera sucesso, mas com lista vazia
                    apiResponse.Data = Enumerable.Empty<Rodada>();
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }
                apiResponse.Success = true;
                apiResponse.Data = rodadas;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as rodadas do campeonato.");
                Notificar("Erro", $"Erro interno ao obter todas as rodadas: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>();
                return apiResponse;
            }
        }

        public async Task<ApiResponse> Atualizar(Rodada Rodada)
        {
            var apiResponse = new ApiResponse(); // Instanciação direta
            try
            {
                if (Rodada == null)
                {
                    Notificar("Erro", "A rodada fornecida para atualização é nula."); // Notificar com 2 argumentos
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false;
                    return apiResponse;
                }

                await _rodadaRepository.Atualizar(Rodada);
                if (!await CommitAsync())
                {
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    apiResponse.Success = false;
                    return apiResponse;
                }
                apiResponse.Success = true;
                Notificar("Sucesso", "Status da Rodada atualizado com sucesso!"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar rodada.");
                Notificar("Erro", $"Erro interno ao atualizar rodada: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                return apiResponse;
            }
        }

        public async Task<ApiResponse<IEnumerable<Rodada>>> ObterRodadasEmDestaque()
        {
            var apiResponse = new ApiResponse<IEnumerable<Rodada>>(); // Instanciação direta
            try
            {
                var rodadas = await _rodadaRepository.Buscar(r => r.Status == StatusRodada.Corrente || r.Status == StatusRodada.EmApostas)
                                                     .Include(r => r.Campeonato)
                                                     .OrderBy(r => r.DataInic)
                                                     .Take(5)
                                                     .ToListAsync();

                if (!rodadas.Any())
                {
                    Notificar("Alerta", "Nenhuma rodada em destaque encontrada na base de dados."); // Notificar com 2 argumentos
                    apiResponse.Success = true; // Considera sucesso, mas com lista vazia
                    apiResponse.Data = Enumerable.Empty<Rodada>();
                    apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                    return apiResponse;
                }

                apiResponse.Success = true;
                apiResponse.Data = rodadas;
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter rodadas em destaque.");
                Notificar("Erro", $"Erro interno ao obter rodadas em destaque: {ex.Message}"); // Notificar com 2 argumentos
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Success = false;
                apiResponse.Data = Enumerable.Empty<Rodada>();
                return apiResponse;
            }
        }
    }
}
