using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Domain.Models.Campeonatos;

namespace ApostasApp.Core.Application.Services.Interfaces.Campeonatos
{
    public interface ICampeonatoService 
    {
        Task<IEnumerable<CampeonatoListItemDto>> GetAvailableCampeonatosAsync();
        Task<bool> AdherirCampeonatoAsync(Guid apostadorId, Guid campeonatoId);

    }
}
