// Localização: ApostasApp.Core.Application.Services.Financeiro/FinanceiroService.cs

using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Models.Notificacoes; // Para NotificationDto
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Financeiro
{
    public class FinanceiroService : BaseService, IFinanceiroService
    {
        private readonly ISaldoRepository _saldoRepository;
        private readonly ITransacaoFinanceiraRepository _transacaoFinanceiraRepository;
        private readonly IApostadorRepository _apostadorRepository;
        private readonly IMapper _mapper;

        public FinanceiroService(
            ISaldoRepository saldoRepository,
            ITransacaoFinanceiraRepository transacaoFinanceiraRepository,
            IApostadorRepository apostadorRepository,
            IUnitOfWork uow,
            INotificador notificador,
            IMapper mapper) : base(notificador, uow)
        {
            _saldoRepository = saldoRepository;
            _transacaoFinanceiraRepository = transacaoFinanceiraRepository;
            _apostadorRepository = apostadorRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém o saldo atual de um apostador.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <returns>Um ApiResponse com o DTO do saldo atual.</returns>
        public async Task<ApiResponse<SaldoDto>> ObterSaldoAtualAsync(Guid apostadorId)
        {
            var apiResponse = new ApiResponse<SaldoDto>(); // Instancia o ApiResponse<T>
            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                Notificar("Alerta", "Apostador não possui saldo registrado. Saldo inicializado como zero.");
                apiResponse.Success = false;
                apiResponse.Message = "Apostador não possui saldo registrado. Saldo inicializado como zero.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = default(SaldoDto); // Define o valor padrão para SaldoDto
            }
            else
            {
                apiResponse.Success = true;
                apiResponse.Message = "Saldo obtido com sucesso.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = _mapper.Map<SaldoDto>(saldo);
            }
            return apiResponse;
        }

        /// <summary>
        /// Debita um valor do saldo de um apostador e registra a transação.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <param name="valor">O valor a ser debitado.</param>
        /// <param name="tipoTransacao">O tipo da transação (ex: AdesaoCampeonato).</param>
        /// <param name="descricao">A descrição da transação.</param>
        /// <returns>Um ApiResponse indicando o sucesso do débito.</returns>
        public async Task<ApiResponse<bool>> DebitarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao)
        {
            var apiResponse = new ApiResponse<bool>(); // Instancia o ApiResponse<bool>

            if (valor <= 0)
            {
                Notificar("Erro", "O valor do débito deve ser maior que zero.");
                apiResponse.Success = false;
                apiResponse.Message = "O valor do débito deve ser maior que zero.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = false;
                return apiResponse;
            }

            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                Notificar("Erro", "Apostador não possui saldo para debitar. Por favor, deposite um valor antes.");
                apiResponse.Success = false;
                apiResponse.Message = "Apostador não possui saldo para debitar.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = false;
                return apiResponse;
            }

            var debitoEfetuado = saldo.Debitar(valor);

            if (!debitoEfetuado)
            {
                Notificar("Alerta", "Saldo insuficiente para realizar o débito.");
                apiResponse.Success = false;
                apiResponse.Message = "Saldo insuficiente para realizar o débito.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = false;
                return apiResponse;
            }

            _saldoRepository.Atualizar(saldo);
            var transacao = new TransacaoFinanceira(
                apostadorId,
                tipoTransacao,
                -valor,
                descricao
            );
            transacao.SaldoId = saldo.Id;
            _transacaoFinanceiraRepository.Adicionar(transacao);

            Notificar("Sucesso", "Débito e transação preparados para persistência.");
            apiResponse.Success = true;
            apiResponse.Message = "Débito e transação preparados para persistência.";
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            apiResponse.Data = true;
            return apiResponse;
        }

        /// <summary>
        /// Credita um valor ao saldo de um apostador e registra a transação.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <param name="valor">O valor a ser creditado.</param>
        /// <param name="tipoTransacao">O tipo da transação (ex: Deposito).</param>
        /// <param name="descricao">A descrição da transação.</param>
        /// <returns>Um ApiResponse indicando o sucesso do crédito.</returns>
        public async Task<ApiResponse<bool>> CreditarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao)
        {
            var apiResponse = new ApiResponse<bool>(); // Instancia o ApiResponse<bool>

            if (valor <= 0)
            {
                Notificar("Erro", "O valor do crédito deve ser maior que zero.");
                apiResponse.Success = false;
                apiResponse.Message = "O valor do crédito deve ser maior que zero.";
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                apiResponse.Data = false;
                return apiResponse;
            }

            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                saldo = new Saldo(apostadorId, 0);
                _saldoRepository.Adicionar(saldo);
            }

            saldo.Adicionar(valor);
            _saldoRepository.Atualizar(saldo);

            var transacao = new TransacaoFinanceira(
                apostadorId,
                tipoTransacao,
                valor,
                descricao
            );
            transacao.SaldoId = saldo.Id;

            _transacaoFinanceiraRepository.Adicionar(transacao);

            Notificar("Sucesso", "Crédito e transação preparados para persistência.");
            apiResponse.Success = true;
            apiResponse.Message = "Crédito e transação preparados para persistência.";
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            apiResponse.Data = true;
            return apiResponse;
        }
    }
}
