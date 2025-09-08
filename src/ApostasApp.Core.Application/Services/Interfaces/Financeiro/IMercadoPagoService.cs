using ApostasApp.Core.Domain.Models.Financeiro;

namespace ApostasApp.Core.Application.Services.Interfaces
{
    public interface IMercadoPagoService
    {
        Task<PixResponseDto> CriarPagamentoPixAsync(decimal valor, string descricao, string idExterno);
    }
}