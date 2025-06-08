using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces; // Para BaseService
using ApostasApp.Core.Application.Services.Interfaces.Financeiro; // Para IFinanceiroService
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork
using ApostasApp.Core.Domain.Interfaces.Financeiro; // Para ISaldoRepository, ITransacaoFinanceiraRepository
using ApostasApp.Core.Domain.Interfaces.Notificacoes; // Para INotificador
using ApostasApp.Core.Domain.Models.Financeiro; // Para Saldo, TransacaoFinanceira, TipoTransacao
using System;
using System.Threading.Tasks;

namespace ApostasApp.Application.Financeiro.Services.Financeiro // Verifique este namespace, ele pode ser ApostasApp.Core.Application.Services.Financeiro
{
    // Agora herda de BaseService
    public class FinanceiroService : BaseService, IFinanceiroService
    {
        private readonly ISaldoRepository _saldoRepository;
        private readonly ITransacaoFinanceiraRepository _transacaoFinanceiraRepository;

        // Construtor ajustado para passar notificador e uow para a BaseService
        public FinanceiroService(
            ISaldoRepository saldoRepository,
            ITransacaoFinanceiraRepository transacaoFinanceiraRepository,
            IUnitOfWork uow, // UoW injetado
            INotificador notificador) : base(notificador, uow) // Passando para a BaseService
        {
            _saldoRepository = saldoRepository;
            _transacaoFinanceiraRepository = transacaoFinanceiraRepository;
        }

        /// <summary>
        /// Obtém o saldo atual de um apostador.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <returns>Um DTO com o saldo atual.</returns>
        public async Task<SaldoDto> ObterSaldoAtualAsync(Guid apostadorId)
        {
            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                Notificar("Alerta", "Apostador não possui saldo registrado. Saldo inicializado como zero.");
                return new SaldoDto { ApostadorId = apostadorId, Valor = 0, DataUltimaAtualizacao = DateTime.Now };
            }

            return new SaldoDto
            {
                ApostadorId = saldo.ApostadorId,
                Valor = saldo.Valor,
                DataUltimaAtualizacao = saldo.DataUltimaAtualizacao
            };
        }

        /// <summary>
        /// Debita um valor do saldo de um apostador e registra a transação.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <param name="valor">O valor a ser debitado.</param>
        /// <param name="tipoTransacao">O tipo da transação (ex: AdesaoCampeonato).</param>
        /// <param name="descricao">A descrição da transação.</param>
        /// <returns>True se o débito foi bem-sucedido, false caso contrário.</returns>
        public async Task<bool> DebitarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao)
        {
            if (valor <= 0)
            {
                Notificar("Erro", "O valor do débito deve ser maior que zero.");
                return false;
            }

            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                Notificar("Erro", "Apostador não possui saldo para debitar.");
                return false;
            }

            var debitoEfetuado = saldo.Debitar(valor);

            if (!debitoEfetuado)
            {
                Notificar("Alerta", "Saldo insuficiente para realizar o débito.");
                return false;
            }

            _saldoRepository.Atualizar(saldo);

            var transacao = new TransacaoFinanceira(
                saldo.Id,
                tipoTransacao,
                -valor, // Valor negativo para representar o débito
                descricao);

            await _transacaoFinanceiraRepository.Adicionar(transacao);

            // CORRIGIDO: Usar o Commit() da BaseService para salvar todas as alterações
            var saved = await Commit();

            if (saved)
            {
                Notificar("Sucesso", $"Débito de {valor:C} realizado com sucesso. Saldo atual: {saldo.Valor:C}");
            }
            else
            {
                Notificar("Erro", "Não foi possível registrar o débito. Tente novamente.");
            }

            return saved;
        }

        /// <summary>
        /// Credita um valor ao saldo de um apostador e registra a transação.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <param name="valor">O valor a ser creditado.</param>
        /// <param name="tipoTransacao">O tipo da transação (ex: Deposito).</param>
        /// <param name="descricao">A descrição da transação.</param>
        /// <returns>True se o crédito foi bem-sucedido, false caso contrário.</returns>
        public async Task<bool> CreditarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao)
        {
            if (valor <= 0)
            {
                Notificar("Erro", "O valor do crédito deve ser maior que zero.");
                return false;
            }

            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                // Se não houver saldo, cria um novo
                saldo = new Saldo(apostadorId, 0);
                await _saldoRepository.Adicionar(saldo);
            }

            saldo.Adicionar(valor);

            _saldoRepository.Atualizar(saldo);

            var transacao = new TransacaoFinanceira(
                saldo.Id,
                tipoTransacao,
                valor,
                descricao);

            await _transacaoFinanceiraRepository.Adicionar(transacao);

            // CORRIGIDO: Usar o Commit() da BaseService para salvar todas as alterações
            var saved = await Commit();

            if (saved)
            {
                Notificar("Sucesso", $"Crédito de {valor:C} realizado com sucesso. Saldo atual: {saldo.Valor:C}");
            }
            else
            {
                Notificar("Erro", "Não foi possível registrar o crédito. Tente novamente.");
            }

            return saved;
        }
    }
}
