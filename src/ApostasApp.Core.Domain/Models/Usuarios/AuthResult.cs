using ApostasApp.Core.Domain.Models.Notificacoes;
using System.Collections.Generic;

namespace ApostasApp.Core.Domain.Models.Usuarios
{
    public class AuthResult
    {
        public bool Success { get; private set; }
        public string UserId { get; private set; }
        public string Email { get; private set; }
        public string UserName { get; private set; }
        public IReadOnlyList<Notificacao> Notifications { get; private set; }

        private AuthResult(bool success, string userId, string email, string userName, List<Notificacao> notifications = null)
        {
            Success = success;
            UserId = userId;
            Email = email;
            UserName = userName;
            Notifications = notifications ?? new List<Notificacao>();
        }

        // Renomeado de Success para Succeeded para evitar conflito com a propriedade
        public static AuthResult Succeeded(string userId, string email, string userName)
        {
            return new AuthResult(true, userId, email, userName);
        }

        public static AuthResult Failure(List<Notificacao> notifications)
        {
            return new AuthResult(false, null, null, null, notifications);
        }
    }
}
