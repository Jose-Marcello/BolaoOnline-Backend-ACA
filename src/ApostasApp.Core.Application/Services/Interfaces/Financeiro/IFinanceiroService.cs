using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Domain.Models.Financeiro;

namespace ApostasApp.Core.Application.Services.Interfaces.Financeiro // Ajuste o namespace se necessário
{
    public interface IFinanceiroService
    {
        Task<SaldoDto> ObterSaldoAtualAsync(Guid apostadorId);

        // Adicionar TipoTransacao como parâmetro
        Task<bool> DebitarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao);

        Task<bool> CreditarSaldoAsync(Guid apostadorId, decimal valor, TipoTransacao tipoTransacao, string descricao);
    }
}
    


