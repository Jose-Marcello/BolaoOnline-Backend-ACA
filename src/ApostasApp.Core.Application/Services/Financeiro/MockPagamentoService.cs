using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ApostasApp.Core.Infrastructure.Services.Financeiro
{
    public class MockPagamentoService : IFinanceiroService
    {
        public static ConcurrentDictionary<string, PagamentoDto> PagamentosSimulados { get; } = new();

        public Task<ApiResponse<SaldoDto>> ObterSaldoAtualAsync(Guid apostadorId)
        {
            return Task.FromResult(new ApiResponse<SaldoDto> { Data = new SaldoDto { Valor = 100.00M, DataUltimaAtualizacao = DateTime.Now }, Success = true });
        }

        public Task<ApiResponse<bool>> DebitarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao)
        {
            return Task.FromResult(new ApiResponse<bool> { Data = true, Success = true });
        }

        public Task<ApiResponse<bool>> CreditarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao)
        {
            return Task.FromResult(new ApiResponse<bool> { Data = true, Success = true });
        }

        
        // CORREÇÃO: O TIPO DE RETORNO FOI AJUSTADO AQUI
        public Task<ApiResponse<SimulaPixResponseDto>> GerarPixSimuladoParaDepositoAsync(DepositarRequestDto request)
        {
            var idTransacao = Guid.NewGuid().ToString();

            PagamentosSimulados[idTransacao] = new PagamentoDto
            {
                IdTransacao = idTransacao,
                Valor = request.Valor,
                Status = "AGUARDANDO_PAGAMENTO"
            };

            var pixResponse = new SimulaPixResponseDto
            {
                Sucesso = true,
                Mensagem = "PIX simulado gerado com sucesso.",
                IdTransacao = idTransacao,
                QrCodeBase64 = "Mocked_QRCode_Base64",
                QrCodeString = $"Mocked_QRCode_String_Id_{idTransacao}"
            };

            return Task.FromResult(new ApiResponse<SimulaPixResponseDto> { Data = pixResponse, Success = true });
        }

        public Task<ApiResponse<bool>> CreditarSaldoViaWebhookAsync(string externalReference, decimal valor)
        {
            // O serviço real trataria isso, no nosso mock é só um método de "fachada"
            return Task.FromResult(new ApiResponse<bool> { Data = true, Success = true });
        }

        // Métodos de utilidade para o Mock (que não estão na interface)
        public Task<SimulacaoPagamentoResultadoDto> SimularWebhookPixAsync(string idTransacao, string novoStatus)
        {
            // Lógica de simulação de webhook
            if (PagamentosSimulados.TryGetValue(idTransacao, out var pagamento))
            {
                pagamento.Status = novoStatus;
                return Task.FromResult(new SimulacaoPagamentoResultadoDto
                {
                    Sucesso = true,
                    Mensagem = $"Status do pagamento {idTransacao} atualizado para {novoStatus}.",
                    IdTransacao = idTransacao
                });
            }

            return Task.FromResult(new SimulacaoPagamentoResultadoDto
            {
                Sucesso = false,
                Mensagem = $"Transação {idTransacao} não encontrada para atualização.",
                IdTransacao = idTransacao
            });
        }

        public static void LimparPagamentos()
        {
            PagamentosSimulados.Clear();
        }
               

        Task<ApiResponse<PixResponseDto>> IFinanceiroService.GerarPixParaDepositoAsync(DepositarRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}