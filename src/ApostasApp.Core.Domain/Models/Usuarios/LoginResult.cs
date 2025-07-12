// Localização: ApostasApp.Core.Domain/Models/Identity/LoginResult.cs

using ApostasApp.Core.Domain.Models.Notificacoes; // Usar NotificationDto
using System;
using System.Collections.Generic;

namespace ApostasApp.Core.Domain.Models.Identity
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsNotAllowed { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public List<NotificationDto> Notifications { get; set; } // Alterado para NotificationDto

        private LoginResult(bool success, bool requiresTwoFactor, bool isLockedOut, bool isNotAllowed, string token, string refreshToken, DateTime expiration, List<NotificationDto> notifications)
        {
            Success = success;
            RequiresTwoFactor = requiresTwoFactor;
            IsLockedOut = isLockedOut;
            IsNotAllowed = isNotAllowed;
            Token = token;
            RefreshToken = refreshToken;
            Expiration = expiration;
            Notifications = notifications ?? new List<NotificationDto>();
        }

        public static LoginResult Succeeded(string token, string refreshToken, DateTime expiration)
        {
            return new LoginResult(true, false, false, false, token, refreshToken, expiration, null);
        }

        public static LoginResult TwoFactorRequired()
        {
            return new LoginResult(false, true, false, false, null, null, DateTime.MinValue, null);
        }

        public static LoginResult LockedOut()
        {
            return new LoginResult(false, false, true, false, null, null, DateTime.MinValue, null);
        }

        public static LoginResult NotAllowed()
        {
            return new LoginResult(false, false, false, true, null, null, DateTime.MinValue, null);
        }

        public static LoginResult Failed(List<NotificationDto> notifications) // Alterado para NotificationDto
        {
            return new LoginResult(false, false, false, false, null, null, DateTime.MinValue, notifications);
        }

        public static LoginResult Failed(string message, string type = "Erro", string code = null, string fieldName = null)
        {
            return new LoginResult(false, false, false, false, null, null, DateTime.MinValue, new List<NotificationDto> { new NotificationDto { Codigo = code, Tipo = type, Mensagem = message, NomeCampo = fieldName } });
        }
    }
}
