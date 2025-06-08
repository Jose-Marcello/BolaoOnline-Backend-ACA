using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using ApostasApp.Core.Domain.Models.Configuracoes;

namespace ApostasApp.Core.Infrastructure.Services.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly SendGridSettings _sendGridSettings;

        public SendGridEmailSender(ISendGridClient sendGridClient, IOptions<SendGridSettings> sendGridSettingsAccessor)
        {
            _sendGridClient = sendGridClient;
            _sendGridSettings = sendGridSettingsAccessor.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromEmail = _sendGridSettings.FromEmail;
            var fromName = _sendGridSettings.FromName;
            var apiKey = _sendGridSettings.ApiKey;

            if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromName) || string.IsNullOrEmpty(apiKey))
            {
                System.Console.WriteLine("SendGrid settings are not fully configured (FromEmail, FromName, or ApiKey missing). Cannot send email.");
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

            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted &&
                response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                System.Console.WriteLine($"Erro ao enviar email via SendGrid. Status: {response.StatusCode}, Body: {responseBody}");
            }
            else
            {
                System.Console.WriteLine($"Email enviado com sucesso para {email}.");
            }
        }
    }
}
