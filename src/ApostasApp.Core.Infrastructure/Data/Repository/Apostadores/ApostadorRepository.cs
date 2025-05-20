using ApostasApp.Core.Domain.Interfaces.Apostadores;
using ApostasApp.Core.Domain.Models.Apostadores;
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


        public async Task<Apostador> ObterApostadorPorUsuarioId(string Id)
        {
            return await Db.Apostadores.AsNoTracking()
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.UsuarioId == Id);

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