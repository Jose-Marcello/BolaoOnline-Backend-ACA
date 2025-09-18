using ApostasApp.Core.Domain.Models.Interfaces.Rodadas;
using ApostasApp.Core.Domain.Models.Usuarios;
using ApostasApp.Core.Infrastructure.Data.Repository;
using ApostasApp.Core.Infrastructure.Data.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore; // Adicionado para o método AnyAsync
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

// Namespace deve ser ajustado para a localização real do arquivo
namespace ApostasApp.Core.Infrastructure.Data.Repository.Usuarios
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly ILogger<UsuarioRepository> _logger; // Tipo do logger ajustado para a classe atual
        private readonly IMapper _mapper;

        public UsuarioRepository(MeuDbContext context, ILogger<UsuarioRepository> logger, IMapper mapper) : base(context)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> CpfExiste(string cpf)
        {
            // O acesso ao DbContext é feito através do `Db`, que é herdado da classe base Repository
            return await Db.Usuarios.AnyAsync(u => u.CPF == cpf);
        }

        public async Task<bool> ApelidoExiste(string apelido)
        {
            // O acesso ao DbContext é feito através do `Db`, que é herdado da classe base Repository
            return await Db.Usuarios.AnyAsync(u => u.Apelido == apelido);
        }
    }
}
