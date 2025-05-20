using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Presentation.ViewModels

{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "O EMAIL é obrigatório")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A SENHA é obrigatória")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }
    }

}
