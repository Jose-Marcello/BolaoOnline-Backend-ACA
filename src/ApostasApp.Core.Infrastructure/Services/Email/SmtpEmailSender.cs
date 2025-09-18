// Localização: ApostasApp.Core.Infrastructure.Services.Email/SmtpEmailSender.cs

using ApostasApp.Core.Application.Services.Interfaces.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ApostasApp.Core.Infrastructure.Services.Email
{
    // A classe agora tem um nome que reflete o que ela faz
    public class SmtpEmailSender : IBolaoEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailSender> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // O construtor não precisa mais do cliente SendGrid
        public SmtpEmailSender(IConfiguration configuration,
                              ILogger<SmtpEmailSender> logger,
                              IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var logoUrl = "https://placehold.co/150x150/png"; //provisoriamente

                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = int.Parse(emailSettings["SmtpPort"]);
                var smtpUsername = emailSettings["SmtpUsername"];
                var smtpPassword = emailSettings["SmtpPassword"];
                var fromEmail = emailSettings["FromEmail"];
                var fromName = "BOLÃO ONLINE";

                var mailMessage = new MailMessage();
                mailMessage.To.Add(email);
                mailMessage.From = new MailAddress(fromEmail, fromName);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;

                // Opção 2: Use um link direto para o logo
                // Certifique-se de que "App:BaseUrl" está configurado no seu appsettings.json
                //var baseUrl = _configuration["App:BaseUrl"];
                //var logoUrl = $"{baseUrl}/images/logo-bolao.png";

                //var logoUrl = "https://placehold.co/150x150/png"; //provisoriamente

                // Adicione a tag <img> com o URL do logo no início do corpo do HTML
                var htmlBodyWithLogo = $"<div style='text-align: center; margin-bottom: 20px;'><img src='{logoUrl}' alt='Logo Bolão Online' style='width: 150px; max-width: 100%; height: auto;' /></div>" + htmlMessage;

                mailMessage.Body = htmlBodyWithLogo;

                using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await smtpClient.SendMailAsync(mailMessage);

                    _logger.LogInformation($"Email enviado com sucesso via SMTP para {email}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exceção ao enviar e-mail via SMTP para {email}.");
            }
        }
    }
}