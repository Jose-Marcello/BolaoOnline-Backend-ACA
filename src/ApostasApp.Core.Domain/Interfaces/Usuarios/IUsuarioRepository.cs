using ApostasApp.Core.Domain.Interfaces; // Para IRepository
using ApostasApp.Core.Domain.Interfaces.Relatorios;
using ApostasApp.Core.Domain.Models.Jogos; // Para Jogo (se Jogo for uma entidade separada)
using ApostasApp.Core.Domain.Models.Rodadas; // Para Rodada
using ApostasApp.Core.Domain.Models.Usuarios;
using System;
using System.Collections.Generic; // Para IEnumerable
using System.Threading.Tasks;

namespace ApostasApp.Core.Domain.Models.Interfaces.Rodadas
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<bool> CpfExiste(string cpf);
        Task<bool> ApelidoExiste(string apelido);

    }
}
