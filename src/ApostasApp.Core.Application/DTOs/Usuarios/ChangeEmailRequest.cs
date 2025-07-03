// Exemplo: ApostasApp.Core.Application.DTOs.Usuarios\ChangeEmailRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class ChangeEmailRequestDto
    {
        [Required(ErrorMessage = "O novo e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string NewEmail { get; set; }
    }
}
