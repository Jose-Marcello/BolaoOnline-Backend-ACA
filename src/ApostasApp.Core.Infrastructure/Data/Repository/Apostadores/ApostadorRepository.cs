using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.InfraStructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApostasApp.Core.InfraStructure.Data.Repository.Apostadores
{
    public class ApostadorRepository : Repository<Apostador>, IApostadorRepository
    {
        private readonly ILogger<ApostadorRepository> _logger;

        public ApostadorRepository(MeuDbContext context,
                                   ILogger<ApostadorRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<Apostador> ObterApostador(Guid id)
        {
            return await Db.Apostadores.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }


        /// <summary>
        /// Obtém um apostador pelo ID do usuário do Identity associado (UsuarioId).
        /// </summary>
        /// <param name="userId">O ID do usuário do Identity (string).</param>
        /// <returns>A entidade Apostador encontrada, ou null se não existir.</returns>
        public async Task<Apostador?> ObterApostadorPorUsuarioId(string userId) // Renomeei o parâmetro para userId para clareza
        {
            // <<-- AQUI ESTÁ A CORREÇÃO: Converter userId (string) para Guid ANTES da consulta -->>
            //if (!Guid.TryParse(userId, out Guid userIdAsGuid))
            //{
                // Se a string não for um Guid válido, não há como encontrar o apostador.
                // Você pode logar um erro aqui se quiser.
            //    return null;
            //}

            return await Db.Apostadores // Use _context.Apostadores diretamente para a consulta
                                 .AsNoTracking() // Geralmente bom para consultas de leitura
                                 .Include(a=>a.ApostadoresCampeonatos)
                                 .Include(a => a.Usuario)                                 
                                 .Include(a => a.Saldo)
                                 .FirstOrDefaultAsync(a => a.UsuarioId == userId); // <<-- AGORA COMPARA GUID COM GUID
                                 //.FirstOrDefaultAsync(a => a.UsuarioId == userIdAsGuid); // <<-- AGORA COMPARA GUID COM GUID
        }
    
    //public async Task<IEnumerable<Apostador>> ObterApostadorAtivo()
    /* public async Task<Apostador> ObterApostadorAtivo()

     {
         //throw new NotImplementedException();

         return await Db.Apostadores.AsNoTracking()
                      //.Where(c => c.Ativo == true)
                      .FirstOrDefaultAsync(c => c.Ativo == true);
                      //.ToListAsync();

     }*/



}

}