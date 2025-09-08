// RegisterRequestDto.cs
// DTO para a requisição de registro de usuário

using ApostasApp.Core.Application.Utils;
using System.ComponentModel.DataAnnotations;

namespace ApostasApp.Core.Application.DTOs.Usuarios
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo {1} caracteres.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "O apelido é obrigatório.")]
        public string Apelido { get; set; }

        [Required(ErrorMessage = "O NOME COMPLETO é obrigatório.")]
        public string NomeCompleto { get; set; }

        private string _cpf;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public string Cpf
        {
            get => _cpf;
            set => _cpf = value.CleanNumbers(); // <<-- APLICA A LIMPEZA AQUI
        }

        [Required(ErrorMessage = "O celular é obrigatório.")]
        public string Celular { get; set; }

        public string FotoPerfil { get; set; } = string.Empty; // Adicionado para exibição no dashboard

        public bool TermsAccepted { get; set; }

        // NOVOS CAMPOS: Para passar scheme e host para o serviço de e-mail de confirmação.
        public string Scheme { get; set; }
        public string Host { get; set; }
        public bool SendConfirmationEmail { get; set; } = true; // Por padrão, enviar e-mail de confirmação


        



    }
}
