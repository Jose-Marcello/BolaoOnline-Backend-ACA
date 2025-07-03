using ApostasApp.Core.Domain.Models.Notificacoes;
using System;
using System.Collections.Generic;


namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class UsuarioLoginResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; } // Propriedade para RefreshToken
        public DateTime? Expiration { get; set; } // Propriedade para Expiração do Token
        public bool RequiresTwoFactor { get; set; } // Se a conta requer 2FA
        public bool IsLockedOut { get; set; } // Se a conta está bloqueada
        public List<NotificationDto> Notifications { get; set; } // Lista de notificações

        public UsuarioLoginResult()
        {
            Notifications = new List<NotificationDto>();
        }
    }
}
