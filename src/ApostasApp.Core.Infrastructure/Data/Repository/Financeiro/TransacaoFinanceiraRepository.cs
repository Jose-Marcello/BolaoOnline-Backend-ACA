// Em ApostasApp.Core.InfraStructure.Data.Repository.Financeiro/TransacaoFinanceiraRepository.cs

using ApostasApp.Core.Domain.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Context; // Assumindo o caminho do seu DbContext
using ApostasApp.Core.InfraStructure.Data.Repository; // Assumindo o caminho da sua classe base Repository<TEntity>

using System.Threading.Tasks;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Financeiro // Ajuste o namespace
{
    // TransacaoFinanceiraRepository herda da sua base abstrata Repository<TransacaoFinanceira>
    public class TransacaoFinanceiraRepository : Repository<TransacaoFinanceira>, ITransacaoFinanceiraRepository
    {
        // O construtor de TransacaoFinanceiraRepository chama o construtor da classe base
        public TransacaoFinanceiraRepository(MeuDbContext context) : base(context) // <-- Chama a base
        {
            // Métodos como Adicionar(), Atualizar(), Remover() já são herdados do GenericRepository.
            // Se TransacaoFinanceira precisar de métodos de consulta específicos que não estão no genérico,
            // você os adicionaria aqui. Por enquanto, os métodos do IRepository genérico são suficientes.
        }

        // Exemplo: Se você precisasse de um método para obter todas as transações de um usuário específico
        // public async Task<IEnumerable<TransacaoFinanceira>> ObterTransacoesPorUsuario(Guid usuarioId)
        // {
        //     return await _dbSet.Where(t => t.ApostadorId == usuarioId).ToListAsync();
        // }
    }
}