using ApostasApp.Core.Application.Services.Interfaces.Email;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApostasApp.Core.Infrastructure.Services.Email
{
    public class MockEmailSender : IBolaoEmailSender
    {
        // Adicionamos um dicionário estático para armazenar os e-mails
        // A chave é o e-mail do destinatário, e o valor é uma lista de objetos EmailMock
        public static ConcurrentDictionary<string, List<EmailMock>> EmailsEnviados { get; } = new();

        private readonly ILogger<MockEmailSender> _logger;

        public MockEmailSender(ILogger<MockEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            _logger.LogWarning("--- MODO DE SIMULAÇÃO DE E-MAIL ATIVO ---");

            // Crie uma nova instância da lista se a chave não existir
            EmailsEnviados.TryAdd(toEmail, new List<EmailMock>());

            // Extrai o link de confirmação do HTML, se houver
            string confirmationLink = null;
            var match = Regex.Match(htmlMessage, @"href='(.*?)'");
            if (match.Success)
            {
                confirmationLink = match.Groups[1].Value;
                _logger.LogWarning($"LINK DE CONFIRMAÇÃO ENCONTRADO (PARA TESTE): {confirmationLink}");
            }

            // Adiciona o e-mail simulado à lista do destinatário
            EmailsEnviados[toEmail].Add(new EmailMock
            {
                To = toEmail,
                Subject = subject,
                Body = htmlMessage,
                ConfirmationLink = confirmationLink,
                SentAt = DateTime.Now
            });

            _logger.LogInformation($"E-mail simulado armazenado para {toEmail} com o assunto '{subject}'.");

            return Task.CompletedTask;
        }

        // --- MÉTODOS ESTÁTICOS PARA ACESSAR OS DADOS ---
        public static List<EmailMock> ObterEmails(string email)
        {
            EmailsEnviados.TryGetValue(email, out var emails);
            return emails;
        }

        public static void LimparEmails()
        {
            EmailsEnviados.Clear();
        }
    }

    public class EmailMock
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ConfirmationLink { get; set; }
        public DateTime SentAt { get; set; }
    }
}