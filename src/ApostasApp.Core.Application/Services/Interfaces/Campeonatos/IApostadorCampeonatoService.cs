using ApostasApp.Core.Application.DTOs.ApostadorCampeonatos;
using ApostasApp.Core.Application.Models; // <<-- ADICIONADO: Para ApiResponse -->>
using ApostasApp.Core.Domain.Models.Campeonatos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Campeonatos
{
    public interface IApostadorCampeonatoService
    {
        Task<ApiResponse<ApostadorCampeonato>> ObterPorId(Guid id);
        Task<ApiResponse> Adicionar(ApostadorCampeonato apostadorCampeonato);
        Task<ApiResponse> Remover(ApostadorCampeonato apostadorCampeonato);
        Task<ApiResponse<ApostadorCampeonato>> ObterApostadorCampeonatoPorApostadorECampeonato(Guid apostadorId, Guid campeonatoId);
        Task<ApiResponse<IEnumerable<ApostadorCampeonatoDto>>> ObterApostadoresDoCampeonato(Guid campeonatoId);
    }
}
