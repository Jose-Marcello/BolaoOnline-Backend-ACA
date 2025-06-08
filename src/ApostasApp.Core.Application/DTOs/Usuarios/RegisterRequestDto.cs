using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    /// <summary>
    /// DTO para a requisição de registro de um novo usuário.
    /// Contém os campos necessários para o registro via API.
    /// </summary>
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O Email está em formato inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "A Senha e a Confirmação de Senha não conferem.")]
        public string ConfirmPassword { get; set; } // Campo para confirmação de senha

        [Required(ErrorMessage = "O Apelido é obrigatório.")]
        [StringLength(50, ErrorMessage = "O {0} deve ter no máximo {1} caracteres.")]
        public string Apelido { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "O CPF está em formato inválido.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O Celular é obrigatório.")]
        [Phone(ErrorMessage = "O Celular está em formato inválido.")]
        public string Celular { get; set; }
    }
}
