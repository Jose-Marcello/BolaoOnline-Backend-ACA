using ApostasApp.Core.Domain.Models.Apostadores;
using ApostasApp.Core.Domain.Models.Interfaces;

namespace ApostasApp.Core.Domain.Interfaces.Apostadores
{
    public interface IApostadorRepository : IRepository<Apostador>
    {
        Task<Apostador> ObterApostador(Guid id);
        Task<Apostador> ObterApostadorPorUsuarioId(string Id);

        //Só pode haver um campeonato ativo (Validar isso)
        //Task<Apostador> ObterApostadorAtivo();

    }
}