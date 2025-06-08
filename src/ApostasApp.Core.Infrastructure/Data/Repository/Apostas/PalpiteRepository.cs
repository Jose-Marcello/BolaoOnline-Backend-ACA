using ApostasApp.Core.Domain.Interfaces.Apostas; // Para IPalpiteRepository
using ApostasApp.Core.Domain.Models.Apostas; // Para Palpite, ApostaRodada
using ApostasApp.Core.InfraStructure.Data.Context; // Para MeuDbContext
using ApostasApp.Core.Infrastructure.Data.Repository; // Para Repository<T>
using Microsoft.EntityFrameworkCore; // Para Include, Where, ExecuteDeleteAsync, AsNoTracking, ToListAsync
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; // Para Where, Any()
using System.Threading.Tasks;
// REMOVER ESTA LINHA SE AINDA EXISTIR: using System.Data.Entity;


namespace ApostasApp.Core.Infrastructure.Data.Repository.Apostas
{
    public class PalpiteRepository : Repository<Palpite>, IPalpiteRepository
    {
        private readonly ILogger<PalpiteRepository> _logger;
        public PalpiteRepository(MeuDbContext context, ILogger<PalpiteRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Palpite>> ObterPalpitesDaRodada(Guid rodadaId)
        {
            return await DbSet.AsNoTracking()
                              .Include(p => p.Jogo) // Incluir dados do Jogo
                                  .ThenInclude(j => j.EquipeCasa) // j.EquipeCasa já é a entidade Equipe.
                                      .ThenInclude(ec => ec.Equipe) // Remover esta linha se EquipeCasa já é a entidade Equipe. Manter se EquipeCasa é uma "join entity".
                              .Include(p => p.Jogo)
                                  .ThenInclude(j => j.EquipeVisitante) // j.EquipeVisitante já é a entidade Equipe.
                                      .ThenInclude(ev => ev.Equipe) // Remover esta linha se EquipeVisitante já é a entidade Equipe. Manter se EquipeVisitante é uma "join entity".
                                                                    // CORREÇÃO AQUI: Navega de Palpite -> ApostaRodada -> Apostador (que é o ApostadorCampeonato na ApostaRodada) -> Usuario
                              .Include(p => p.ApostaRodada) // 'p' é Palpite, tem ApostaRodada
                                  .ThenInclude(ar => ar.ApostadorCampeonato) // 'ar' é ApostaRodada, tem ApostadorCampeonato (que é do TIPO Apostador)
                                      .ThenInclude(apostador => apostador.Usuario) // AGORA CORRETO: 'apostador' é do tipo Apostador, que tem a propriedade Usuario.
                              .Where(p => p.Jogo.RodadaId == rodadaId)
                              .ToListAsync();
        }

        /// <summary>
        /// Remove todos os palpites associados a uma rodada específica de forma eficiente.
        /// </summary>
        /// <param name="rodadaId">O ID da rodada cujos palpites serão removidos.</param>
        /// <returns>True se a operação for bem-sucedida, false caso contrário.</returns>
        public async Task<bool> RemoverTodosPalpitesDaRodada(Guid rodadaId)
        {
            try
            {
                var affectedRows = await DbSet.Where(p => p.Jogo.RodadaId == rodadaId)
                                              .ExecuteDeleteAsync(); // Método eficiente para deleção em massa

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover palpites da rodada {RodadaId}", rodadaId);
                return false;
            }
        }
    }
}
