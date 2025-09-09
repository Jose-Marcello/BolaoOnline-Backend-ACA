using ApostasApp.Core.Application.DTOs.Apostas;
using ApostasApp.Core.Application.DTOs.Ranking;
using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Interfaces.Apostas;
using ApostasApp.Core.Domain.Models.Apostas;
using ApostasApp.Core.Domain.Models.Campeonatos;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Context;
using ApostasApp.Core.InfraStructure.Data.Models;
using ApostasApp.Core.InfraStructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

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


    public async Task<ApostaRodada> ObterStatusApostaRodada(Guid rodadaId, Guid apostadorCampeonatoId)
    {
        return await Db.ApostasRodada // Agora busca na coleção de ApostaRodada
                       .Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId && ar.RodadaId == rodadaId)
                       .FirstOrDefaultAsync();

    }

    public async Task<ApostaRodada> ObterApostaRodadaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId)
    {
        return await Db.ApostasRodada // Agora busca na coleção de ApostaRodada
                       .Include(ar => ar.Palpites) // Se precisar dos palpites para algo mais (não para o status de envio aqui)
                       .AsNoTracking()
                       .Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId && ar.RodadaId == rodadaId)
                       .FirstOrDefaultAsync();
    }


    public async Task<IEnumerable<ApostaRodada>> ObterApostasRodadaApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId)
    {
        return await Db.ApostasRodada // Agora busca na coleção de ApostaRodada
                                      //.Include(ar => ar.Palpites) // Se precisar dos palpites para algo mais (não para o status de envio aqui)
                       .AsNoTracking()
                       .Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId && ar.RodadaId == rodadaId).ToListAsync();

    }
    // Localização: ApostasApp.Core.Infrastructure.Data.Repositories.Apostas/ApostaRodadaRepository.cs

    public async Task<IEnumerable<IRankingResult>> ObterDadosRankingCampeonatoAsync(Guid campeonatoId)
    {
        var rankingQuery = await Db.ApostasRodada
            .AsNoTracking()
            .Include(a => a.ApostadorCampeonato)
                .ThenInclude(ac => ac.Apostador)
                .ThenInclude(a => a.Usuario)
            .Where(a => a.Rodada.CampeonatoId == campeonatoId)
            .GroupBy(a => a.ApostadorCampeonatoId)
            .Select(g => new RankingDataModel
            {
                ApostadorId = g.First().ApostadorCampeonato.ApostadorId,
                UsuarioId = g.First().ApostadorCampeonato.Apostador.Usuario.Id, // <<-- CAMPO ADICIONADO AQUI
                Pontuacao = g.Sum(a => a.PontuacaoTotalRodada),
                NomeApostador = g.First().ApostadorCampeonato.Apostador.NomeCompleto,
                Apelido = g.First().ApostadorCampeonato.Apostador.Usuario.Apelido,
                FotoPerfil = g.First().ApostadorCampeonato.Apostador.Usuario.FotoPerfil
            })
            .OrderByDescending(r => r.Pontuacao)
            .ToListAsync();

        return rankingQuery;
    }

    // Em ApostasApp.Core.Infrastructure.Data.Repositories.Apostas/ApostaRodadaRepository.cs

    // ... (seus outros métodos)

    /// <summary>
    /// Obtém o ranking total do campeonato, somando a pontuação de todas as rodadas.
    /// </summary>
    /// <param name="campeonatoId">O ID do campeonato.</param>
    /// <returns>Uma lista de objetos com os dados do ranking.</returns>
    public async Task<IEnumerable<IRankingResult>> ObterRankingCampeonatoAsync(Guid campeonatoId)
    {
        var rankingQuery = await Db.ApostasRodada
            .AsNoTracking()
            // Inclui os dados do apostador e do usuário para a projeção final
            .Include(ar => ar.ApostadorCampeonato)
                .ThenInclude(ac => ac.Apostador)
                    .ThenInclude(ap => ap.Usuario)
            // Garante que estamos pegando apenas as apostas do campeonato correto
            .Where(ar => ar.Rodada.CampeonatoId == campeonatoId)
            // Agrupa por apostador para somar os pontos de todas as rodadas
            .GroupBy(ar => new
            {
                ApostadorId = ar.ApostadorCampeonato.ApostadorId,
                Apelido = ar.ApostadorCampeonato.Apostador.Usuario.Apelido,
                FotoPerfil = ar.ApostadorCampeonato.Apostador.Usuario.FotoPerfil
            })
            // Projeta os dados para um DTO, somando a pontuação
            .Select(g => new RankingDataModel
            {
                ApostadorId = g.Key.ApostadorId,
                Apelido = g.Key.Apelido,
                FotoPerfil = g.Key.FotoPerfil,
                Pontuacao = g.Sum(ar => ar.PontuacaoTotalRodada)
            })
            .OrderByDescending(r => r.Pontuacao)
            .ToListAsync();

        // A lógica de posição pode ser adicionada aqui ou no serviço
        //var rankingComPosicao = rankingQuery.Select((item, index) => {
         //   item.Posicao = index + 1;
         //   return item;
        //}).ToList();

        return rankingQuery;
    }

    // ...



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
        //return await DbSet.Where(ar => ar.ApostadorCampeonatoId == apostadorCampeonatoId &&
        //                                ar.RodadaId == rodadaId &&
         //                               ar.DataHoraSubmissao.HasValue)
         //                 .OrderByDescending(ar => ar.DataHoraSubmissao)
         //                 .FirstOrDefaultAsync();
    }



    public async Task<ApostasTotais> ObterTotaisApostasAvulsas(Guid rodadaId)
    {
        var totais = await Db.ApostasRodada.AsNoTracking()
            // Ajuste na condição: usar o campo booleano EhApostaIsolada
            .Where(a => a.RodadaId == rodadaId && a.EhApostaIsolada)
            .GroupBy(a => a.RodadaId)
            .Select(g => new ApostasTotais
            {
                NumeroDeApostas = g.Count(),
                ValorTotal = g.Sum(a => a.CustoPagoApostaRodada) ?? 0m   //Usado porque o CustoPagoApostaRodada é ?
            })
            .FirstOrDefaultAsync();

        return totais ?? new ApostasTotais();
    }

    // A query deve somar os valores de todas as rodadas do campeonato
    public async Task<CampeonatoTotais> ObterTotaisCampeonato(Guid campeonatoId)
    {
        var totais = await Db.ApostadoresCampeonatos.AsNoTracking()
            .Where(ac => ac.CampeonatoId == campeonatoId)
            .GroupBy(ac => ac.CampeonatoId)
            .Select(g => new CampeonatoTotais
            {
                NumeroDeApostadores = g.Count(),
                // Corrigido: Para obter o custo de adesão do campeonato, você precisa
                // pegar o valor de um dos registros (já que é o mesmo para todos)
                // e multiplicar pelo número de apostadores.
                ValorTotalArrecadado = (g.FirstOrDefault().Campeonato.CustoAdesao) * g.Count() ?? 0m
            })
            .FirstOrDefaultAsync();

        return totais ?? new CampeonatoTotais();
    }
}
    






    

