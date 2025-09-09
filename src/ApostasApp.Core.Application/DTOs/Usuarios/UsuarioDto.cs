// Localização sugerida: ApostasApp.Core.Application.DTOs.Usuarios

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    /// <summary>
    /// DTO para representar informações básicas de um usuário.
    /// Utilizado para enviar dados do usuário ao frontend ou em outras operações.
    /// </summary>
    public class UsuarioDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty; // Mantenha se o username for um identificador de login diferente do email
        public string Email { get; set; } = string.Empty;
        public string Apelido { get; set; } = string.Empty; // Adicionado para exibição no dashboard
        public string FotoPerfil { get; set; } = string.Empty; // Adicionado para exibição no dashboard
        public bool TermsAccepted { get; set; }

        // Adicione outras propriedades do usuário que você deseja expor ao frontend.
        // Por exemplo:
        // public string NomeCompleto { get; set; } = string.Empty;
        // public bool Ativo { get; set; }
        // public List<string> Roles { get; set; } = new List<string>(); // Se você usar roles
    }
}
