// Localização: ApostasApp.Core.Domain/Models/Identity/AuthResult.cs

using ApostasApp.Core.Domain.Models.Notificacoes; // AGORA USAR Notificacao
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
        public List<Notificacao> Notifications { get; set; } // Alterado para Notificacao

        private AuthResult(bool success, string userId, string email, string username, List<Notificacao> notifications) // Alterado para Notificacao
        {
            Success = success;
            UserId = userId;
            Email = email;
            Username = username;
            Notifications = notifications ?? new List<Notificacao>();
        }

        public static AuthResult Succeeded(string userId, string email, string username)
        {
            return new AuthResult(true, userId, email, username, null);
        }

        public static AuthResult Failure(List<Notificacao> notifications) // Alterado para Notificacao
        {
            return new AuthResult(false, null, null, null, notifications);
        }

        public static AuthResult Failure(string message, string type = "Erro", string code = null, string fieldName = null)
        {
            // Alterado para criar uma instância de Notificacao
            return new AuthResult(false, null, null, null, new List<Notificacao> { new Notificacao(code, type, message, fieldName) });
        }
    }
}