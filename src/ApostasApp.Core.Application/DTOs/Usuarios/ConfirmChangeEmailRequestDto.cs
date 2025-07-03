// Exemplo: ApostasApp.Core.Application.DTOs.Usuarios\ConfirmChangeEmailRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class ConfirmChangeEmailRequestDto
    {
        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "O novo e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "O código de confirmação é obrigatório.")]
        public string Code { get; set; }
    }
}