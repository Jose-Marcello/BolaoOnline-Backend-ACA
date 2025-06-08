using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos;
using ApostasApp.Core.Application.Services.Campeonatos;
using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Application.Services.Interfaces.Campeonatos
{
    public interface IApostadorCampeonatoService 
    {       
        Task<ApostadorCampeonato> ObterPorId(Guid id); 
        Task Adicionar(ApostadorCampeonato apostadorCampeonato);

        //task Remover(ApostadorCampeonato apostadorCampeonato);
        Task Remover(ApostadorCampeonato apostadorCampeonato);
        Task<ApostadorCampeonato> ObterApostadorCampeonatoPorApostadorECampeonato(string usuarioId, Guid campeonatoId);

        Task<IEnumerable<ApostadorCampeonatoDto>> ObterApostadoresDoCampeonato(Guid campeonatoId);

    }
}
