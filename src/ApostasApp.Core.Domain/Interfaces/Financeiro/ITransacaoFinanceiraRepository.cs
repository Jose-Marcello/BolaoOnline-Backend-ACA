using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Domain.Interfaces;

namespace ApostasApp.Core.Domain.Interfaces.Financeiro
{ 
    public interface ITransacaoFinanceiraRepository : IRepository<TransacaoFinanceira>
    {
        // Por enquanto, o método 'Adicionar' do IRepository genérico já é suficiente para TransacaoFinanceira.
        // Você pode adicionar métodos específicos aqui no futuro, se precisar (ex: obter transações por período, por tipo, etc.).
        // NOVO MÉTODO PARA DEPÓSITO

        Task<TransacaoFinanceira> ObterPorReferenciaExterna(string externalReference);

    }
}