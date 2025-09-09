// WebhookController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Cryptography;
using ApostasApp.Core.Domain.Models.Configuracoes;

[Route("api/webhooks")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly MercadoPagoSettings _mercadoPagoSettings;

    public WebhookController(IOptions<MercadoPagoSettings> settings)
    {
        _mercadoPagoSettings = settings.Value;
    }

    [HttpPost("pix")]
    public async Task<IActionResult> ReceberNotificacaoPix()
    {
        // 1. Obtenha o corpo da requisição e o cabeçalho de assinatura
        var body = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
        var signature = Request.Headers["X-Signature"].FirstOrDefault();

        // 2. Valide a assinatura
        if (!IsValidSignature(body, signature, _mercadoPagoSettings.WebhookSecret))
        {
            return Unauthorized(); // Rejeita a requisição se a assinatura for inválida
        }

        // 3. Opcional: Processar o payload aqui (próximo passo)
        Console.WriteLine("Notificação de PIX recebida e validada!");
        Console.WriteLine(body);

        return Ok();
    }

    private bool IsValidSignature(string body, string signature, string secret)
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secret))
        {
            return false;
        }

        var parts = signature.Split(',');
        if (parts.Length < 2)
        {
            return false;
        }

        var timestamp = parts[0].Split('=')[1];
        var hash = parts[1].Split('=')[1];

        // Gere o hash localmente
        var data = $"{timestamp}.{body}";
        var hashBytes = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(data));
        var localHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        return hash == localHash;
    }
}