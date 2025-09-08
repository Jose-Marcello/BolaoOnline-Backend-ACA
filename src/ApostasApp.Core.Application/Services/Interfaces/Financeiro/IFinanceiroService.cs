using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Models;
using ApostasApp.Core.Domain.Models.Financeiro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Financeiro
{
    public interface IFinanceiroService
    {
        Task<ApiResponse<SaldoDto>> ObterSaldoAtualAsync(Guid apostadorId);
        Task<ApiResponse<bool>> DebitarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao);

        Task<ApiResponse<bool>> CreditarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao);

        Task<ApiResponse<SimulaPixResponseDto>> GerarPixSimuladoParaDepositoAsync(DepositarRequestDto request);

        Task<ApiResponse<PixResponseDto>> GerarPixParaDepositoAsync(DepositarRequestDto request);

        Task<ApiResponse<bool>> CreditarSaldoViaWebhookAsync(string externalReference, decimal valor);
    }
}
    
 