// Localização: ApostasApp.Core.Domain/Models/Identity/AuthResult.cs

using ApostasApp.Core.Domain.Models.Notificacoes; // Usar NotificationDto
using System.Collections.Generic;
using System.Linq;

namespace ApostasApp.Core.Domain.Models.Identity
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public List<NotificationDto> Notifications { get; set; } // Alterado para NotificationDto

        private AuthResult(bool success, string userId, string email, string username, List<NotificationDto> notifications)
        {
            Success = success;
            UserId = userId;
            Email = email;
            Username = username;
            Notifications = notifications ?? new List<NotificationDto>();
        }

        public static AuthResult Succeeded(string userId, string email, string username)
        {
            return new AuthResult(true, userId, email, username, null);
        }

        public static AuthResult Failure(List<NotificationDto> notifications) // Alterado para NotificationDto
        {
            return new AuthResult(false, null, null, null, notifications);
        }

        public static AuthResult Failure(string message, string type = "Erro", string code = null, string fieldName = null)
        {
            return new AuthResult(false, null, null, null, new List<NotificationDto> { new NotificationDto { Codigo = code, Tipo = type, Mensagem = message, NomeCampo = fieldName } });
        }
    }
}
