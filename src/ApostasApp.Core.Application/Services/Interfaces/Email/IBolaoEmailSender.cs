// Localização: ApostasApp.Core.Application.Services.Interfaces.Financeiro/IFinanceiroService.cs
using ApostasApp.Core.Application.DTOs.Financeiro;
// Localização: ApostasApp.Core.Application.Services.Interfaces.Email/IEmailSender.cs

using System.Threading.Tasks;

namespace ApostasApp.Core.Application.Services.Interfaces.Email
{
    public interface IBolaoEmailSender
    {
        // <<-- ASSINATURA CORRIGIDA PARA 4 ARGUMENTOS -->>
        //Task SendEmailAsync(string toEmail, string subject, string htmlMessage, string plainTextContent);
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}
