// Localização: ApostasApp.Core.Application.Services.Interfaces.Financeiro/IFinanceiroService.cs
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
        // Se você tiver um método para extrato, adicione-o aqui, por exemplo:
        // Task<ApiResponse<IEnumerable<TransacaoFinanceiraDto>>> ObterExtratoFinanceiro(Guid apostadorId);
    }
}
