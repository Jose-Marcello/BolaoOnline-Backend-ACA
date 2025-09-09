// Exemplo: ApostasApp.Core.Application.DTOs.Usuarios\ChangePasswordRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "A senha atual é obrigatória.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A nova senha deve ter no mínimo {1} caracteres.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "A confirmação da nova senha é obrigatória.")]
        [Compare("NewPassword", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmNewPassword { get; set; }
    }
}
