// LoginRequestDto.cs
// DTO para a requisição de login
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo {1} caracteres.")] // Ajuste conforme sua política de senha
        public string Password { get; set; }

        public bool IsPersistent { get; set; } // Para "Lembrar-me"
    }
}
