using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.InfraStructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ApostaRodadaRepository : Repository<ApostaRodada>, IApostaRodadaRepository
{
    private readonly ILogger<ApostaRodadaRepository> _logger;

    // Sobrecarga de construtor sem ILogger, se ainda usar. Idealmente, ter apenas um.
    public ApostaRodadaRepository(MeuDbContext context, ILogger<ApostaRodadaRepository> logger) : base(context)
    {
        _logger = logger;
    }
            

    // Se este construtor é usado em algum lugar (ex: testes simples), mantenha.
    // Se não, remova-o e injete o logger sempre.
    public ApostaRodadaRepository(MeuDbContext db) : base(db)
    {
        // _logger = null; // Cuidado com NullReferenceException se usar _logger
    }

    public async Task<ApostaRodada> ObterApostaRodadaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId)
    {
        return await Db.ApostasRodada // Agora busca na coleção de ApostaRodada
                       .Include(ar => ar.Palpites) // Se precisar dos palpites para algo mais (não para o status de envio aqui)
                       .AsNoTracking()
                       .Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId && ar.RodadaId == rodadaId)
                       .FirstOrDefaultAsync();
    }

    // Exemplo de implementação no ApostaRodadaRepository
    public async Task<ApostaRodada> ObterUltimaApostaRodadaDoApostadorNaRodada(Guid apostadorCampeonatoId, Guid rodadaId)
    {
        // Esta lógica depende de como você define "última" ou "ativa".
        // Opção 1: A aposta que ainda está em edição (DataHoraSubmissao é null)
        var apostaEmEdicao = await Db.ApostasRodada.Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId &&
                                                      ar.RodadaId == rodadaId &&
                                                      !ar.DataHoraSubmissao.HasValue)
                                        .FirstOrDefaultAsync();

        //if (apostaEmEdicao != null)
        //{
            return apostaEmEdicao;
        //}

        // Opção 2: Se não há aposta em edição, pega a última submetida
        // (Isso será relevante quando o usuário puder ter múltiplas ApostaRodadas submetidas)
        //return await DbSet.Where(ar => ar.ApostadorCampeamentoId == apostadorCampeamentoId &&
        //                                ar.RodadaId == rodadaId &&
         //                               ar.DataHoraSubmissao.HasValue)
         //                 .OrderByDescending(ar => ar.DataHoraSubmissao)
         //                 .FirstOrDefaultAsync();
    }

}
