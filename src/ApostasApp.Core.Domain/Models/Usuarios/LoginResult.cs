using ApostasApp.Core.Domain.Models.Notificacoes; // Garante que Notificacao é reconhecido
using System.Collections.Generic;
using System; // Para DateTime

namespace ApostasApp.Core.Domain.Models // OU ApostasApp.Core.Domain.Results, dependendo do seu namespace real
{
    /// <summary>
    /// Esta classe encapsula o resultado de uma operação de login no DOMÍNIO.
    /// Ela inclui informações do token de autenticação e notificações.
    /// </summary>
    public class LoginResult
    {
        public bool Success { get; private set; }
        public string Token { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime Expiration { get; private set; } // CRÍTICO: Propriedade Expiration
        public bool RequiresTwoFactor { get; private set; }
        public bool IsLockedOut { get; private set; }
        public List<Notificacao> Notifications { get; private set; } // Lista de notificações

        // Construtor privado para garantir que a criação de LoginResult seja feita pelos métodos estáticos
        private LoginResult(bool success, string token, string refreshToken, DateTime expiration, bool requiresTwoFactor, bool isLockedOut, List<Notificacao> notifications)
        {
            Success = success;
            Token = token;
            RefreshToken = refreshToken;
            Expiration = expiration;
            RequiresTwoFactor = requiresTwoFactor;
            IsLockedOut = isLockedOut;
            Notifications = notifications ?? new List<Notificacao>();
        }

        // Construtor auxiliar para falhas que não envolvem tokens
        private LoginResult(bool success, bool requiresTwoFactor, bool isLockedOut, List<Notificacao> notifications)
        {
            Success = success;
            RequiresTwoFactor = requiresTwoFactor;
            IsLockedOut = isLockedOut;
            Notifications = notifications ?? new List<Notificacao>();
            // Em caso de falha, tokens e expiração serão nulos/DateTime.MinValue
            Token = null;
            RefreshToken = null;
            Expiration = DateTime.MinValue;
        }

        // --- Métodos Estáticos para Criar Instâncias de LoginResult ---

        /// <summary>
        /// Cria uma instância de LoginResult para um login bem-sucedido.
        /// </summary>
        public static LoginResult Succeeded(string token, string refreshToken, DateTime expiration) // CRÍTICO: Método Succeeded
        {
            return new LoginResult(true, token, refreshToken, expiration, false, false, null);
        }

        /// <summary>
        /// Cria uma instância de LoginResult para um login falho com notificações.
        /// </summary>
        public static LoginResult Failed(List<Notificacao> notifications = null) // CRÍTICO: Método Failed
        {
            return new LoginResult(false, false, false, notifications);
        }

        /// <summary>
        /// Cria uma instância de LoginResult indicando que a autenticação de dois fatores é necessária.
        /// </summary>
        public static LoginResult TwoFactorRequired()
        {
            return new LoginResult(false, true, false, null);
        }

        /// <summary>
        /// Cria uma instância de LoginResult indicando que a conta do usuário está bloqueada.
        /// </summary>
        public static LoginResult LockedOut()
        {
            return new LoginResult(false, false, true, null);
        }
    }
}
