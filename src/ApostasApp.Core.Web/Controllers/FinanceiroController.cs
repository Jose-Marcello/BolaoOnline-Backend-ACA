// Localização: ApostasApp.Core.Web/Controllers/FinanceiroController.cs

using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Models; // Para ApiResponse
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Domain.Models.Financeiro; // Para TipoTransacao
using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Domain.Interfaces; // Para IUnitOfWork (se ainda for necessário para DI, mas não para BaseController)
using System; // Para Guid e Task
using System.Threading.Tasks; // Para Task
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using ApostasApp.Core.Web.Controllers; // Para BaseController

namespace ApostasApp.Core.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Adicionado para proteger os endpoints por padrão
    public class FinanceiroController : BaseController
    {
        private readonly IFinanceiroService _financeiroService;

        public FinanceiroController(
            IFinanceiroService financeiroService,
            INotificador notificador,
            // REMOVIDO: IUnitOfWork uow, pois BaseController não o recebe mais no construtor
            IUnitOfWork uow) : base(notificador) // Passa apenas o notificador para a BaseController
        {
            _financeiroService = financeiroService;
        }

        [HttpGet("saldo/{apostadorId}")]
        public async Task<IActionResult> GetSaldo(Guid apostadorId)
        {
            var response = await _financeiroService.ObterSaldoAtualAsync(apostadorId);
            return CustomResponse(response); // Usa CustomResponse
        }

        [HttpPost("depositar/{apostadorId}")]
        public async Task<IActionResult> Depositar(Guid apostadorId, [FromBody] DepositRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return CustomValidationProblem(ModelState); // Usa CustomValidationProblem
            }

            var response = await _financeiroService.CreditarSaldoAsync(apostadorId, request.Amount, TipoTransacao.CreditoManual, "Depósito via API");
            return CustomResponse(response); // Usa CustomResponse
        }
        /*
        [HttpPost("debitar/{apostadorId}")]
        public async Task<IActionResult> Debitar(Guid apostadorId, [FromBody] DebitRequestDto request) // Supondo um DebitRequestDto
        {
            if (!ModelState.IsValid)
            {
                return CustomValidationProblem(ModelState);
            }

            var response = await _financeiroService.DebitarSaldoAsync(apostadorId, request.Amount, TipoTransacao.DebitoManual, "Débito via API");
            return CustomResponse(response);
        }
        */
    }
}
