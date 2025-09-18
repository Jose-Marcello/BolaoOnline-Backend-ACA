// Localização: ApostasApp.Core.Infrastructure.Services.Email/SendGridEmailSender.cs

using Microsoft.AspNetCore.Identity.UI.Services; // Usando a interface padrão do Identity UI
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace ApostasApp.Core.Infrastructure.Services.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration; // Injetar IConfiguration para ler settings
        private readonly ILogger<SendGridEmailSender> _logger; // Injetar ILogger

        public SendGridEmailSender(ISendGridClient sendGridClient, IConfiguration configuration, ILogger<SendGridEmailSender> logger)
        {
            _sendGridClient = sendGridClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];
            var apiKey = _configuration["SendGrid:ApiKey"]; // Embora não usado para instanciar client aqui, é bom ter acesso para logs/verificação

            if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromName) || string.IsNullOrEmpty(apiKey))
            {
                var errorMessage = "SendGrid settings are not fully configured (FromEmail, FromName, or ApiKey missing). Cannot send email.";
                _logger.LogError(errorMessage);
                System.Console.WriteLine(errorMessage); // Para visualização rápida no console de debug
                return;
            }

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = subject,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            try
            {
                // <<-- COLOQUE UM BREAKPOINT NESSA LINHA ABAIXO -->>
                var response = await _sendGridClient.SendEmailAsync(msg);

                // <<-- NOVO: LER O CORPO DA RESPOSTA AQUI -->>
                var responseBody = await response.Body.ReadAsStringAsync(); // Esta variável conterá o corpo da resposta

                if (response.StatusCode != System.Net.HttpStatusCode.Accepted &&
                    response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError($"Falha ao enviar email via SendGrid. Status: {response.StatusCode}, Body: {responseBody}");
                    System.Console.WriteLine($"Erro ao enviar email via SendGrid. Status: {response.StatusCode}, Body: {responseBody}");
                }
                else
                {
                    _logger.LogInformation($"Email enviado com sucesso para {email}. Status: {response.StatusCode}, Body: {responseBody}");
                    System.Console.WriteLine($"Email enviado com sucesso para {email}. Status: {response.StatusCode}, Body: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"EXCEÇÃO NO ENVIO DE E-MAIL VIA SENDGRID para {email}.");
                System.Console.WriteLine($"EXCEÇÃO NO ENVIO DE E-MAIL VIA SENDGRID para {email}: {ex.Message}");
            }
        }
    }
}
