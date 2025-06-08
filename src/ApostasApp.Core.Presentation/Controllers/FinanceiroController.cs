// Exemplo de como seu FinanceiroController ficaria
using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using ApostasApp.Core.Application.Services.Interfaces; // Para INotificador (se não for injetado de outra forma)
using ApostasApp.Core.Domain.Models.Financeiro; // Para TipoTransacao
using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;


namespace ApostasApp.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceiroController : BaseController // <<< Agora herda de BaseController
    {
        private readonly IFinanceiroService _financeiroService;

        public FinanceiroController(
            IFinanceiroService financeiroService,
            INotificador notificador) : base(notificador) // <<< Passa o notificador para o construtor da base
        {
            _financeiroService = financeiroService;
        }

        [HttpGet("saldo/{apostadorId}")]
        public async Task<ActionResult> GetSaldo(Guid apostadorId) // Retorna ActionResult, pois CustomResponse já encapsula Ok
        {
            var saldo = await _financeiroService.ObterSaldoAtualAsync(apostadorId);
            // Você pode adicionar uma notificação se o saldo for nulo ou não encontrado, se for uma regra de negócio.
            // Ex: if (saldo == null) Notificar("Saldo não encontrado.");
            return CustomResponse(saldo); // <<< Usa CustomResponse
        }

        [HttpPost("depositar/{apostadorId}")]
        public async Task<ActionResult> Depositar(Guid apostadorId, [FromBody] DepositRequestDto request)
        {
            // ModelState.IsValid é uma validação de modelo do ASP.NET Core, que ainda é útil aqui
            if (!ModelState.IsValid)
            {
                // Você pode opcionalmente converter os erros do ModelState para notificações
                // e usar CustomResponse, ou manter o BadRequest(ModelState) que é padrão da API.
                // Por simplicidade aqui, mantemos BadRequest(ModelState).
                return BadRequest(ModelState);
            }

            var sucesso = await _financeiroService.CreditarSaldoAsync(apostadorId, request.Amount, TipoTransacao.CreditoManual, "Depósito via API");

            if (sucesso)
            {
                return CustomResponse(new { message = "Depósito realizado com sucesso!" }); // <<< Usa CustomResponse
            }
            else
            {
                // Se o FinanceiroService já notifica os erros (ex: "valor inválido"),
                // o CustomResponse() já vai pegar essas notificações.
                return CustomResponse(); // <<< Usa CustomResponse para pegar as notificações do serviço
            }
        }
    }
}