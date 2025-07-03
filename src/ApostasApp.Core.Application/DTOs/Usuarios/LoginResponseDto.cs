// Exemplo: ApostasApp.Core.Application.DTOs.Usuarios\LoginResponseDto.cs
using System;
using System.Collections.Generic;
using ApostasApp.Core.Domain.Models.Notificacoes; // Ajustado para o namespace correto de NotificationDto

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class LoginResponseDto
    {
        public bool LoginSucesso { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string Apelido { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; } // <<-- ADICIONADO: Propriedade UserId
        public List<NotificationDto> Erros { get; set; } // Lista de erros/notificações
    }
}
