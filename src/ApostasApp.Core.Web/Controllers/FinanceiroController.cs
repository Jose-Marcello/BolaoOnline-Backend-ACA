// FinanceiroController.cs

using ApostasApp.Core.Application.DTOs.Financeiro;
using ApostasApp.Core.Domain.Models.Financeiro;
using Microsoft.AspNetCore.Mvc;
using ApostasApp.Core.Domain.Interfaces.Notificacoes;
using ApostasApp.Core.Application.Services.Interfaces.Financeiro;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ApostasApp.Core.Web.Controllers;
using System;
using Microsoft.Extensions.Logging;

// A rota deve ser exatamente a URL que o front-end está enviando
[Route("api/TransacaoFinanceira")]
[ApiController]
[Authorize]
public class FinanceiroController : BaseController
{
    private readonly IFinanceiroService _financeiroService;
    private readonly ILogger<FinanceiroController> _logger;

    public FinanceiroController(
        IFinanceiroService financeiroService,
        INotificador notificador,
        ILogger<FinanceiroController> logger) : base(notificador)
    {
        _financeiroService = financeiroService;
        _logger = logger;
    }

    // Endpoint de Depósito
    [HttpPost("Depositar")]
    public async Task<IActionResult> Depositar([FromBody] DepositarRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            NotificarErro("Dados de depósito inválidos.");
            return CustomResponse();
        }

        _logger.LogInformation("Recebida requisição de depósito para o apostador {ApostadorId} no valor de {Valor}", request.ApostadorId, request.Valor);


        // A alteração está aqui: agora chamamos o método que gera o PIX
        var pixResponse = await _financeiroService.GerarPixParaDepositoAsync(request);

        // Retornamos a resposta do serviço, que contém os dados do PIX
        return CustomResponse(pixResponse);

        /*
        var result = await _financeiroService.CreditarSaldoAsync(
            request.ApostadorId,
            request.Valor,
            TipoTransacao.CreditoManual,
            "Depósito via API"
        );

        return CustomResponse(result);*/

    }

    // Opcional: Adicionar os outros endpoints aqui
    [HttpGet("saldo/{apostadorId}")]
    public async Task<IActionResult> GetSaldo(Guid apostadorId)
    {
        var response = await _financeiroService.ObterSaldoAtualAsync(apostadorId);
        return CustomResponse(response);
    }

    /*
    [HttpPost("SimularWebhookPix")]
    public async Task<IActionResult> SimularWebhookPix([FromBody] SimularWebhookRequestDto request)
    {
        // Crie um DTO simples para esta requisição, como:
        // public class SimularWebhookRequestDto
        // {
        //    public string ExternalReference { get; set; }
        //    public decimal Valor { get; set; }
        // }

        // Chame a lógica de negócio do seu serviço para creditar o saldo.
        var response = await _financeiroService.CreditarSaldoViaWebhookAsync(request.ExternalReference, request.Valor);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    */

}