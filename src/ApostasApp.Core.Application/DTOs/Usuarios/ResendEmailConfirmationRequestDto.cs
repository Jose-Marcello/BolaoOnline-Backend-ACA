using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class ResendEmailConfirmationRequestDto
    {
        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O Email está em formato inválido.")]
        public string Email { get; set; }
    }
}
