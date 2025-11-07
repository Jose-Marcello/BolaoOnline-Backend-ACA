using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
  public class ResetPasswordRequestDto
  {
    [Required(ErrorMessage = "O ID do Usuário é obrigatório.")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "O Token de redefinição é obrigatório.")]
    public string Token { get; set; }

    [Required(ErrorMessage = "O E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O E-mail é inválido.")]
    public string Email { get; set; } // Adicionado para robustez do Backend

    [Required(ErrorMessage = "A Nova Senha é obrigatória.")]
    [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "A Confirmação de Senha é obrigatória.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "A Nova Senha e a Confirmação de Senha não conferem.")]
    public string ConfirmNewPassword { get; set; }
  }
}
