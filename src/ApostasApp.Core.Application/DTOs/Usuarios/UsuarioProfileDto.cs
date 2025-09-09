using System;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    /// <summary>
    /// DTO para retornar informações de perfil do usuário logado.
    /// </summary>
    public class UsuarioProfileDto
    {
        public string Id { get; set; } // ID do IdentityUser
        public string Email { get; set; }
        public string Apelido { get; set; }
        public string CPF { get; set; }
        public string Celular { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool EmailConfirmed { get; set; }
        // Não inclua informações sensíveis como PasswordHash
    }
}
