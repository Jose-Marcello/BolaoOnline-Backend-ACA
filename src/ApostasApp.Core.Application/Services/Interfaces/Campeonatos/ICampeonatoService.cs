// ApostasApp.Core.Application.Services.Interfaces.Campeonatos/ICampeonatoService.cs
using ApostasApp.Core.Application.DTOs.Campeonatos;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Domain.Models.Campeonatos; // Para Campeonato (se necessário para o método Remover)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Campeonatos
{
    public interface ICampeonatoService
    {
        // Métodos que retornam bool ou DTOs diretamente, conforme seu CampeonatoService
        Task<bool> Adicionar(CampeonatoDto campeonatoDto);
        Task<bool> Atualizar(CampeonatoDto campeonatoDto);
        Task<bool> Remover(Campeonato campeonato); // Ou Task<bool> Remover(Guid id); dependendo da sua implementação
        Task<CampeonatoDto> ObterPorId(Guid id);
        Task<IEnumerable<CampeonatoDto>> ObterTodos();

        // Métodos que retornam ApiResponse, conforme seu CampeonatoService
        Task<ApiResponse<IEnumerable<CampeonatoDto>>> GetAvailableCampeonatos(string? userId);
        Task<ApiResponse<bool>> AderirCampeonatoAsync(Guid apostadorId, Guid campeonatoId);
        Task<ApiResponse<CampeonatoDto?>> GetDetalhesCampeonato(Guid id);
    }
}
