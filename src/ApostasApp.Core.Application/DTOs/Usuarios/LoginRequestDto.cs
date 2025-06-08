using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O Email está em formato inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Lembrar-me?")]
        public bool IsPersistent { get; set; } = false; // "Remember me"

        public bool LockoutOnFailure { get; set; } = true; // Bloquear a conta em caso de falhas
    }
}
