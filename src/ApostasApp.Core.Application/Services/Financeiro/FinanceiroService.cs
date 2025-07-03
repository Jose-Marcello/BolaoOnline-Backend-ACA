// ApostasApp.Core.Application.Services.Financeiro/FinanceiroService.cs
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Base;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostadores; // Usando IApostadorRepository
using ApostasApp.Core.Domain.Interfaces.Financeiro; // Usando ISaldoRepository, ITransacaoFinanceiraRepository
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Models.Financeiro;
using AutoMapper; // Adicionado para IMapper

namespace ApostasApp.Core.Application.Services.Financeiro
{
    public class FinanceiroService : BaseService, IFinanceiroService
    {
        private readonly ISaldoRepository _saldoRepository;
        private readonly ITransacaoFinanceiraRepository _transacaoFinanceiraRepository;
        private readonly IApostadorRepository _apostadorRepository; // Mantido do seu original
        private readonly IMapper _mapper; // Adicionado para mapeamento de DTOs

        public FinanceiroService(
            ISaldoRepository saldoRepository,
            ITransacaoFinanceiraRepository transacaoFinanceiraRepository,
            IApostadorRepository apostadorRepository, // Injetado
            IUnitOfWork uow,
            INotificador notificador,
            IMapper mapper) : base(notificador, uow) // Passa notificador e uow para a BaseService
        {
            _saldoRepository = saldoRepository;
            _transacaoFinanceiraRepository = transacaoFinanceiraRepository;
            _apostadorRepository = apostadorRepository; // Atribuído
            _mapper = mapper; // Atribuído
        }

        /// <summary>
        /// Obtém o saldo atual de um apostador.
        /// </summary>
        /// <param name="apostadorId">O ID do apostador.</param>
        /// <returns>Um ApiResponse com o DTO do saldo atual.</returns>
        public async Task<ApiResponse<SaldoDto>> ObterSaldoAtualAsync(Guid apostadorId)
        {
            var apiResponse = new ApiResponse<SaldoDto>(false, null);
            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                Notificar("SALDO_NAO_ENCONTRADO", "Alerta", "Apostador não possui saldo registrado. Saldo inicializado como zero.");
                apiResponse.Success = true; // Considera sucesso, pois o saldo é "zero" e não um erro de sistema
                apiResponse.Data = new SaldoDto { ApostadorId = apostadorId, Valor = 0, DataUltimaAtualizacao = DateTime.Now };
            }
            else
            {
                apiResponse.Success = true;
                apiResponse.Data = new SaldoDto
                {
                    ApostadorId = saldo.ApostadorId,
                    Valor = saldo.Valor,
                    DataUltimaAtualizacao = saldo.DataUltimaAtualizacao
                };
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList(); // Inclui notificações
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
            var apiResponse = new ApiResponse<bool>(false, false);

            if (valor <= 0)
            {
                Notificar("VALOR_DEBITO_INVALIDO", "Erro", "O valor do débito deve ser maior que zero.");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                Notificar("SALDO_INEXISTENTE_DEBITO", "Erro", "Apostador não possui saldo para debitar. Por favor, deposite um valor antes.", "Saldo");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            var debitoEfetuado = saldo.Debitar(valor);

            if (!debitoEfetuado)
            {
                Notificar("SALDO_INSUFICIENTE", "Alerta", "Saldo insuficiente para realizar o débito.");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            _saldoRepository.Atualizar(saldo);

            var transacao = new TransacaoFinanceira(
                apostadorId, // O construtor de TransacaoFinanceira precisa do ApostadorId
                tipoTransacao,
                -valor, // Valor negativo para representar o débito
                descricao
            );
            transacao.SaldoId = saldo.Id; // <<-- CORREÇÃO CRÍTICA: Vincula a transação ao SaldoId

            _transacaoFinanceiraRepository.Adicionar(transacao);

            if (await CommitAsync()) // <<-- ADICIONADO: CommitAsync para persistir as alterações
            {
                apiResponse.Success = true;
                apiResponse.Data = true;
                Notificar("DEBITO_SUCESSO", "Sucesso", $"Débito de {valor:C} realizado com sucesso. Saldo atual: {saldo.Valor:C}");
            }
            else
            {
                Notificar("DEBITO_FALHA_PERSISTENCIA", "Erro", "Não foi possível persistir o débito.");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
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
            var apiResponse = new ApiResponse<bool>(false, false);

            if (valor <= 0)
            {
                Notificar("VALOR_CREDITO_INVALIDO", "Erro", "O valor do crédito deve ser maior que zero.");
                apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
                return apiResponse;
            }

            var saldo = await _saldoRepository.ObterSaldoPorApostadorId(apostadorId);

            if (saldo == null)
            {
                // Se não houver saldo, cria um novo saldo antes de adicionar
                saldo = new Saldo(apostadorId, 0); // Cria um novo saldo com valor inicial zero
                _saldoRepository.Adicionar(saldo); // Adiciona o novo saldo ao repositório
            }

            saldo.Adicionar(valor);
            _saldoRepository.Atualizar(saldo);

            var transacao = new TransacaoFinanceira(
                apostadorId, // O construtor de TransacaoFinanceira precisa do ApostadorId
                tipoTransacao,
                valor,
                descricao
            );
            transacao.SaldoId = saldo.Id; // <<-- CORREÇÃO CRÍTICA: Vincula a transação ao SaldoId

            _transacaoFinanceiraRepository.Adicionar(transacao);

            if (await CommitAsync()) // <<-- ADICIONADO: CommitAsync para persistir as alterações
            {
                apiResponse.Success = true;
                apiResponse.Data = true;
                Notificar("CREDITO_SUCESSO", "Sucesso", $"Crédito de {valor:C} realizado com sucesso. Saldo atual: {saldo.Valor:C}");
            }
            else
            {
                Notificar("CREDITO_FALHA_PERSISTENCIA", "Erro", "Não foi possível persistir o crédito.");
            }
            apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
            return apiResponse;
        }

        // Se você precisa do ObterExtratoFinanceiro, ele deve ser adicionado aqui.
        // Ele deve retornar ApiResponse<IEnumerable<TransacaoFinanceiraDto>>
        // public async Task<ApiResponse<IEnumerable<TransacaoFinanceiraDto>>> ObterExtratoFinanceiro(Guid apostadorId)
        // {
        //     var apiResponse = new ApiResponse<IEnumerable<TransacaoFinanceiraDto>>(false, null);
        //     var extrato = await _transacaoFinanceiraRepository.ObterExtratoPorApostadorIdAsync(apostadorId);
        //     if (extrato == null || !extrato.Any())
        //     {
        //         Notificar("EXTRATO_NAO_ENCONTRADO", "Alerta", "Nenhuma transação encontrada para este apostador.");
        //         apiResponse.Success = true; // Considera sucesso, apenas sem dados
        //         apiResponse.Data = Enumerable.Empty<TransacaoFinanceiraDto>();
        //     }
        //     else
        //     {
        //         apiResponse.Success = true;
        //         apiResponse.Data = _mapper.Map<IEnumerable<TransacaoFinanceiraDto>>(extrato);
        //     }
        //     apiResponse.Notifications = ObterNotificacoesParaResposta().ToList();
        //     return apiResponse;
        // }
    }
}
