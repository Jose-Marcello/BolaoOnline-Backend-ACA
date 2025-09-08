// Em ApostasApp.Core.InfraStructure.Data.Repository.Financeiro/TransacaoFinanceiraRepository.cs

using ApostasApp.Core.Domain.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Models.Financeiro;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Context; // Assumindo o caminho do seu DbContext
using ApostasApp.Core.InfraStructure.Data.Repository; // Assumindo o caminho da sua classe base Repository<TEntity>
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


using System.Threading.Tasks;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Financeiro // Ajuste o namespace
{
    // TransacaoFinanceiraRepository herda da sua base abstrata Repository<TransacaoFinanceira>
    public class TransacaoFinanceiraRepository : Repository<TransacaoFinanceira>, ITransacaoFinanceiraRepository
    {

        //private readonly ILogger<TransacaoFinanceira> _logger;

        public TransacaoFinanceiraRepository(MeuDbContext context) : base(context)
        //ILogger<TransacaoFinanceiraRepository> logger) : base(context)
        {

            //_logger = logger;
        }


        public async Task<TransacaoFinanceira> ObterPorReferenciaExterna(string externalReference)
        {  
            return await Db.TransacoesFinanceiras.AsNoTracking()
                                 .FirstOrDefaultAsync(t => t.ExternalReference == externalReference);

        }
    }
}


    