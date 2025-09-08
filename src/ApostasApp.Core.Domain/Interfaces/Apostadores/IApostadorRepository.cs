using ApostasApp.Core.Domain.Interfaces;
using ApostasApp.Core.Domain.Models.Apostadores;
using System.Threading.Tasks;


namespace ApostasApp.Core.Domain.Interfaces.Apostadores
{
    public interface IApostadorRepository : IRepository<Apostador>
    {
        Task<Apostador> ObterApostador(Guid id);
        Task<Apostador> ObterApostadorPorUsuarioId(string Id);

        Task<Apostador> ObterPorIdComSaldo(Guid id);

        //Só pode haver um campeonato ativo (Validar isso)
        //Task<Apostador> ObterApostadorAtivo();

    }
}