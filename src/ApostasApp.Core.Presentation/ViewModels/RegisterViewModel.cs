using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Por favor, insira um endereço de e-mail válido.")]
        [EmailAddress(ErrorMessage = "Por favor, insira um endereço de e-mail válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Por favor, insira uma senha.")]
        //[StringLength(100, ErrorMessage = "A senha deve ter pelo menos 8 caracteres, uma letra maiúscula, um símbolo e um número.", MinimumLength = 8)]       
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Por favor, insira a confirmação da senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "A senha e a senha de confirmação não correspondem.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Por favor, insira um CPF VÁLIDO (Utilize apenas NÚMEROS).")]
        //[MinLength(11, ErrorMessage = "O CPF deve ter 11 dígitos. O formato 999.999.99-99 é automático")]
        //[MaxLength(11, ErrorMessage = "O CPF deve ter 11 dígitos. O formato 999.999.99-99 é automático")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Por favor, insira um NÚMERO DE CELULAR com DDD !!")]
        [MaxLength(20, ErrorMessage = "O celular deve ter no máximo 20 caracteres.")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "Por favor, insira um APELIDO válido. O APELIDO será sua identificação na tela de Apostas !!")]
        [MaxLength(50, ErrorMessage = "O apelido deve ter no máximo 50 caracteres.")]
        public string Apelido { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } //= DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? LastLoginDate { get; set; } //= DateTime.Now;
    }
}







