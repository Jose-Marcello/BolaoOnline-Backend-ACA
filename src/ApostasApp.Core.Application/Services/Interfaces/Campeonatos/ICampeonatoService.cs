// Localização: ApostasApp.Core.Application.Services.Interfaces/Campeonatos/ICampeonatoService.cs
using ApostasApp.Core.Application.DTOs.Campeonatos; // Para CampeonatoDto
using ApostasApp.Core.Application.Models; // Para ApiResponse
// Removido: using ApostasApp.Core.Domain.Models.Notificacoes; // NotificationDto não é necessário aqui
using ApostasApp.Core.Application.Services.Interfaces; // <<-- NOVO: Para INotifiableService -->>
using ApostasApp.Core.Domain.Models.Campeonatos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Campeonatos
{
    public interface ICampeonatoService : INotifiableService // <<-- CORRIGIDO: Herda de INotifiableService
    {
        // Retorna ApiResponse<IEnumerable<CampeonatoDto>>
        Task<ApiResponse<IEnumerable<CampeonatoDto>>> GetAvailableCampeonatos(string? userId);

        // O seu método de adesão
        Task<ApiResponse<bool>> AderirCampeonatoAsync(Guid apostadorId, Guid campeonatoId);

        // Método para obter detalhes de um Campeonato por ID
        Task<ApiResponse<CampeonatoDto?>> GetDetalhesCampeonato(Guid id); // Retorna ApiResponse<CampeonatoDto?>

        // <<-- REMOVIDOS: Estes métodos são herdados de INotifiableService -->>
        // bool TemNotificacao();
        // IEnumerable<NotificationDto> ObterNotificacoesParaResposta();

        // Métodos básicos que foram adicionados para a interface
        Task<bool> Adicionar(CampeonatoDto campeonatoDto);
        Task<bool> Atualizar(CampeonatoDto campeonatoDto);
        Task<bool> Remover(Campeonato campeoanato); //Guid id);
        Task<CampeonatoDto> ObterPorId(Guid id); // Mantido o retorno de DTO para consistência com a implementação anterior
        Task<IEnumerable<CampeonatoDto>> ObterTodos(); // Mantido o retorno de DTO para consistência com a implementação anterior
    }
}
